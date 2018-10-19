using System;
using System.Linq;
using Boo.Lang;
using JetBrains.Annotations;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils.Input;
using Zenject;

namespace Controllers
{
    public class CameraLookController : MonoBehaviour
    {
        private List<IDisposable> _disposables;
        private InputObservableHelper _inputObservableHelper;
        [SerializeField] public float Sensitivity = 2f;
        [SerializeField] public float Smoothing = 2f;

        [Inject]
        [UsedImplicitly]
        public void Construct(UpdateInputObservableHelper inputObservableHelper)
        {
            _inputObservableHelper = inputObservableHelper;
        }

        private void Start()
        {
            _disposables = new List<IDisposable>();

            var sensitivityObservable = this.UpdateAsObservable().Select(_ => Sensitivity).DistinctUntilChanged();
            var smoothingObservable = this.UpdateAsObservable().Select(_ => Smoothing).DistinctUntilChanged();
            var characterControllerObservable = Observable.Return(GetComponentInParent<CharacterController>());
            var mouseXyObservable = _inputObservableHelper.GetMouseXyObservable();
            var mouseDeltaObservable = sensitivityObservable.CombineLatest(smoothingObservable,
                mouseXyObservable,
                (sensitivity, smoothing, mouseXy) =>
                    new Vector3(mouseXy.MouseX, mouseXy.MouseY) * sensitivity * smoothing
            );
            var mouseDeltaAndSmoothingObservable = mouseDeltaObservable.CombineLatest(smoothingObservable,
                (mouseDelta, smoothing) => new {mouseDelta, smoothing}
            );
            var smoothMouseDeltaObservable = mouseDeltaAndSmoothingObservable.Scan((acc, mouseDeltaAndSmoothing) =>
                {
                    var deltaX = Mathf.Lerp(acc.mouseDelta.x,
                        mouseDeltaAndSmoothing.mouseDelta.x,
                        1f / mouseDeltaAndSmoothing.smoothing);

                    var deltaY = Mathf.Lerp(acc.mouseDelta.y,
                        mouseDeltaAndSmoothing.mouseDelta.y,
                        1f / mouseDeltaAndSmoothing.smoothing);

                    return new
                    {
                        mouseDelta = new Vector3(deltaX, deltaY),
                        mouseDeltaAndSmoothing.smoothing
                    };
                })
                .Select(o => o.mouseDelta);
            var mouseLookObservable = smoothMouseDeltaObservable.Scan((acc, smoothMouseDelta) =>
            {
                var result = acc + smoothMouseDelta;
                return new Vector3(result.x, Mathf.Clamp(result.y, -50 /* down */, 70 /* up */));
            });

            var characterControllerRotationDisposable = characterControllerObservable
                .Select(characterController => characterController.transform.up).CombineLatest(
                    mouseLookObservable.Select(mouseLook => mouseLook.x),
                    (up, mouseLookX) =>
                        Quaternion.AngleAxis(mouseLookX, up)
                )
                .CombineLatest(characterControllerObservable,
                    (localRotation, characterController) => new {localRotation, characterController})
                .Subscribe(o => o.characterController.transform.localRotation = o.localRotation);

            _disposables.Add(characterControllerRotationDisposable);

            var mouseLookDisposable = mouseLookObservable
                .Select(mouseLook => Quaternion.AngleAxis(-mouseLook.y, Vector3.right))
                .Subscribe(localRotation => transform.localRotation = localRotation);

            _disposables.Add(mouseLookDisposable);
        }

        private void OnDestroy()
        {
            _disposables.ToList().ForEach(d => d.Dispose());
        }
    }
}