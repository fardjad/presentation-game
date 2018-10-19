using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace Controllers
{
    public class StatsCanvasController : MonoBehaviour
    {
        private float _attentionSum;
        private ChairManager _chairManager;
        private CommunicationManager _communicationManager;
        private IDisposable _currentSlideDisposable;
        private TimeSpan _elapsedTime;
        private NpcManager _npcManager;
        private IDisposable _qaDisposable;
        private TimeSpan _qaSeconds = TimeSpan.Zero;
        private bool _qaTime;
        private TimeSpan _remainingTime;
        private ScoreManager _scoreManager;
        private SlideController _slideController;
        private TextMeshProUGUI _slideScoreTextMeshPro;
        private bool _slidesLoaded;
        private IDictionary<int, TimeSpan> _slideTime;
        private IDisposable _timerDisposable;
        private TextMeshProUGUI _timeTextMeshPro;

        [SerializeField] public RawImage SlideImage;
        public bool TimerStarted { get; private set; }

        [UsedImplicitly]
        [Inject]
        private void Construct(NpcManager npcManager,
            ChairManager chairManager,
            ScoreManager scoreManager,
            CommunicationManager communicationManager)
        {
            _scoreManager = scoreManager;
            _chairManager = chairManager;
            _npcManager = npcManager;
            _communicationManager = communicationManager;
        }

        private void Start()
        {
            TimerStarted = false;
            _qaSeconds = TimeSpan.Zero;

            _timeTextMeshPro = GetComponentsInChildren
                    <TextMeshProUGUI>()
                .First(component => component.name == "TimeTextMesh");

            _slideScoreTextMeshPro = GetComponentsInChildren
                    <TextMeshProUGUI>()
                .First(component => component.name == "SlideScoreTextMesh");

            _slideController = SlideImage.GetComponent<SlideController>();

            _remainingTime = TimeSpan.FromMinutes(20);
            _elapsedTime = TimeSpan.Zero;

            _scoreManager.Finished = false;
            _slideTime = _scoreManager.SlideTime;
            _slideTime.Clear();

            _currentSlideDisposable = _communicationManager.GetObservableForType("currentSlideScore")
                .Select(scoreObject =>
                {
                    var scoreDictionary = (Dictionary<string, object>) scoreObject;
                    return new
                    {
                        NumberOfKeywords = scoreDictionary["numberOfKeywords"],
                        NumberOfMentionedKeywords = scoreDictionary["numberOfMentionedKeywords"]
                    };
                })
                .Subscribe(score =>
                {
                    _slideScoreTextMeshPro.text = string.Format("{0}/{1}",
                        score.NumberOfMentionedKeywords,
                        score.NumberOfKeywords);
                });

            _qaDisposable = _communicationManager.GetObservableForType("qa")
                .Select(value => (bool) value)
                .Subscribe(qaTime => { _qaTime = qaTime; });
        }

        public void InitializeSlidesTime()
        {
            for (var i = 0; i < _slideController.Slides.Count; i += 1) _slideTime[i] = TimeSpan.Zero;

            _slidesLoaded = true;
        }

        private void StartTimer()
        {
            TimerStarted = true;
            _timerDisposable = this.UpdateAsObservable()
                .Sample(TimeSpan.FromSeconds(1))
                .TimeInterval()
                .Subscribe(ti =>
                {
                    if (_remainingTime <= TimeSpan.Zero)
                    {
                        _scoreManager.Finished = true;
                        return;
                    }

                    var currentSlide = _slideController.CurrentSlide;
                    _slideTime[currentSlide] += ti.Interval;
                    _remainingTime -= ti.Interval;
                    _elapsedTime += ti.Interval;

                    _npcManager.NpcControllerDictionary.Values.ToList()
                        .Select(npcController => (float) npcController.gameObject.GetComponent<NpcAttentionController>()
                            .Blackboard.Parameters["Attention"])
                        .ToList()
                        .ForEach(
                            attention => { _attentionSum += attention; });


                    _communicationManager.SendJson(new {type = "elapsedTime", value = _elapsedTime.TotalSeconds});

                    if (!_qaTime)
                        _scoreManager.AverageAttention = _attentionSum /
                                                         _npcManager.NpcControllerDictionary.Count /
                                                         (_elapsedTime.TotalSeconds - _qaSeconds.TotalSeconds);
                    else
                        _qaSeconds += ti.Interval;

                    _communicationManager.SendJson(new
                    {
                        type = "currentSlideScore"
                    });
                });
        }

        private void StopTimer()
        {
            if (!TimerStarted) return;
            TimerStarted = false;
            _timerDisposable.Dispose();
        }

        private void Update()
        {
            var text = string.Format("{0:00}:{1:00}", _remainingTime.Minutes, _remainingTime.Seconds);
            _timeTextMeshPro.text = text;

            if (_npcManager.NpcControllerDictionary.Count != _chairManager.GetChairsCount() || TimerStarted) return;

            var allSeated = true;

            _npcManager.NpcControllerDictionary.Values.ToList()
                .ForEach(controller =>
                {
                    var stateMachine = controller.StateMachine;
                    if (!stateMachine.CurrentState.Name.Equals("SittingOnTheChair")) allSeated = false;
                });

            if (allSeated && _slidesLoaded && !TimerStarted) StartTimer();
        }

        private void OnDestroy()
        {
            StopTimer();
            _currentSlideDisposable.Dispose();
            _qaDisposable.Dispose();
        }
    }
}