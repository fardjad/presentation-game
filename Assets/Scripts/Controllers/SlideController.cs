using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Utils.Input;
using Utils.VR;
using Valve.VR;
using Zenject;

namespace Controllers
{
    public struct SlideInfo
    {
        public string Url;
        public bool ShouldRaiseHand;
    }

    public class SlideController : MonoBehaviour
    {
        private CommunicationManager _communicationManager;

        private List<IDisposable> _disposables;
        private RawImage _image;
        private UpdateInputObservableHelper _inputObservableHelper;

        public SteamVR_Input_Sources _InputSources;
        private NpcManager _npcManager;

        public Canvas StatsCanvas;
        private ScoreManager _scoreManager;

        public int CurrentSlide { get; private set; }
        public List<SlideInfo> Slides { get; set; }

        [Inject]
        [UsedImplicitly]
        private void Construct(UpdateInputObservableHelper inputObservableHelper,
            NpcManager npcManager,
            CommunicationManager communicationManager,
            ScoreManager scoreManager)
        {
            _inputObservableHelper = inputObservableHelper;
            _communicationManager = communicationManager;
            _npcManager = npcManager;
            _scoreManager = scoreManager;
        }

        private void Start()
        {
            Texture oldTexture = null;
            CurrentSlide = 0;
            _disposables = new List<IDisposable>();
            _image = GetComponent<RawImage>();
            var timeController = StatsCanvas.GetComponent<StatsCanvasController>();

            _communicationManager.SendJson(new {type = "slides"});
            var slidesObservable = _communicationManager.GetObservableForType("slides")
                .Select(slidesDictionary =>
                    (from pair in slidesDictionary as IDictionary<string, object>
                        orderby int.Parse(pair.Key)
                        select pair.Value as IDictionary<string, object>).ToList())
                .Select(slidesList => slidesList.Select(slide => new SlideInfo
                {
                    Url = string.Format("http://localhost:8080{0}", slide["uri"]),
                    ShouldRaiseHand = (bool) slide["shouldRaiseHand"]
                }))
                .Take(1);

            slidesObservable.Subscribe(slides =>
            {
                Slides = slides.ToList();
                timeController.InitializeSlidesTime();
                _communicationManager.SendJson((new {type = "start"}));
            });

            var leftClickObservable = _inputObservableHelper.GetMouseDownObservable(0)
                .Select(_ => 1)
                .SkipUntil(slidesObservable);

            var rightClickObservable = _inputObservableHelper.GetMouseDownObservable(1)
                .Select(_ => -1)
                .SkipUntil(slidesObservable);

            var leftGripObservable = this.UpdateAsObservable()
                .Where(_ => VrUtils.IsInVr)
                .Select(_ => SteamVR_Input._default.inActions.GrabGrip.GetState(SteamVR_Input_Sources.RightHand))
                .DistinctUntilChanged()
                .Where(value => value)
                .Select(_ => 1)
                .SkipUntil(slidesObservable);

            var rightGripObservable = this.UpdateAsObservable()
                .Where(_ => VrUtils.IsInVr)
                .Select(_ => SteamVR_Input._default.inActions.GrabGrip.GetState(SteamVR_Input_Sources.LeftHand))
                .DistinctUntilChanged()
                .Where(value => value)
                .Select(_ => -1)
                .SkipUntil(slidesObservable);


            var clickObservable = slidesObservable
                .Select(_ => 0)
                .Merge(leftClickObservable.Merge(rightClickObservable)).Merge(leftGripObservable)
                .Merge(rightGripObservable);

            var slideNumberObservable = clickObservable.Scan((acc, value) =>
            {
                if (acc + value < 0) return 0;
                if (acc + value >= Slides.Count) return Slides.Count - 1;
                return acc + value;
            });

            var slideNumberChangerDisposable = slideNumberObservable
                .Do(slideNumber => CurrentSlide = slideNumber)
                .Do(slideNumber => _scoreManager.CurrentSlide = slideNumber)
                .Do(slideNumber => _communicationManager.SendJson(new {type = "slideIndex", value = slideNumber}))
                .SelectMany(slideNumber => ObservableWWW.GetAndGetBytes(Slides[slideNumber].Url))
                .Subscribe(contents =>
                {
                    if (oldTexture != null) DestroyImmediate(oldTexture);

                    var texture = new Texture2D(1, 1);
                    texture.LoadImage(contents);
                    oldTexture = texture;
                    _image.texture = texture;

                    if (!timeController.TimerStarted) return;

                    if (Slides[CurrentSlide].ShouldRaiseHand)
                        _npcManager.RandomlyRaiseHand();
                    else
                        _npcManager.PutAllHandsDown();
                });

            _disposables.Add(slideNumberChangerDisposable);
        }

        private void Update()
        {
            if (_image.texture != null) _image.SizeToParent();
        }

        private void OnDestroy()
        {
            _disposables.ToList().ForEach(d => d.Dispose());
        }
    }
}