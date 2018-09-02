using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Utils.Input;
using Zenject;
using Object = UnityEngine.Object;

namespace Controllers
{
    public class SlideController : MonoBehaviour
    {
        private RawImage _image;
        private UpdateInputObservableHelper _inputObservableHelper;
        private List<IDisposable> _disposables;
        private Object[] _textures;

        public int CurrentSlide { get; private set; }
        public int SlidesCount { get; private set; }

        [Inject]
        [UsedImplicitly]
        private void Construct(UpdateInputObservableHelper inputObservableHelper)
        {
            _inputObservableHelper = inputObservableHelper;
        }

        private void Awake()
        {
            _textures = Resources.LoadAll("Slides");
            SlidesCount = _textures.Length;
        }

        private void Start()
        {
            CurrentSlide = 0;

            _disposables = new List<IDisposable>();

            _image = GetComponent<RawImage>();


            var leftClickObservable = _inputObservableHelper.GetMouseDownObservable(0)
                .Select(_ => 1);

            var rightClickObservable = _inputObservableHelper.GetMouseDownObservable(1)
                .Select(_ => -1);

            var clickObservable = leftClickObservable.Merge(rightClickObservable);

            var slideNumberObservable = clickObservable.Scan((acc, value) =>
            {
                if (acc + value < 0) return 0;
                if (acc + value >= SlidesCount) return SlidesCount - 1;
                return acc + value;
            });

            var slideNumberChangerDisposable = slideNumberObservable
                .Do(slideNumber => CurrentSlide = slideNumber)
                .Select(slideNumber => (Texture) _textures[slideNumber])
                .Subscribe(texture => { _image.texture = texture; });

            _disposables.Add(slideNumberChangerDisposable);
        }

        private void Update()
        {
            _image.SizeToParent();
        }

        private void OnDestroy()
        {
            _disposables.ToList().ForEach(d => d.Dispose());
        }
    }
}
