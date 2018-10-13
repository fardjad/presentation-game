using System;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using Utils.Input;
using Utils.VR;
using Zenject;

namespace Controllers
{
    public class VrCameraController : MonoBehaviour
    {
        private UpdateInputObservableHelper _inputObservableHelper;
        public float Speed = 0.5f;
        private IDisposable _inputHDisposable;
        private IDisposable _inputVDisposable;

        [Inject]
        [UsedImplicitly]
        private void Construct(UpdateInputObservableHelper inputObservableHelper)
        {
            _inputObservableHelper = inputObservableHelper;
        }

        private void Start()
        {
            StartCoroutine(VrUtils.SwitchMode(true));

            var transformObservable = Observable.Return(transform);

            _inputHDisposable = _inputObservableHelper.GetHwObservable()
                .Select(o => o.Horizontal)
                .WithLatestFrom(transformObservable, (h, gameObjectTransform) => new {h, gameObjectTransform})
                .Subscribe(o => { o.gameObjectTransform.Translate(o.h * Speed * Time.deltaTime, 0, 0); });

            _inputVDisposable = _inputObservableHelper.GetHwObservable()
                .Select(o => o.Vertical)
                .WithLatestFrom(transformObservable,
                    (v, gameObjectTransform) => new
                    {
                        v,
                        gameObjectTransform
                    })
                .Subscribe(o => { o.gameObjectTransform.Translate(0, o.v * Speed * Time.deltaTime, 0); });
        }


        private void OnDestroy()
        {
            _inputHDisposable.Dispose();
            _inputVDisposable.Dispose();
        }
    }
}