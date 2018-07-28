using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils.Input;

namespace Controllers
{
    public class CameraLookController : MonoBehaviour
    {
        [SerializeField] public float Sensitivity = 2f;
        [SerializeField] public float Smoothing = 2f;

        private void Start()
        {
            var sensitivityObservable = this.UpdateAsObservable().Select(_ => Sensitivity).Distinct();
            var smoothingObservable = this.UpdateAsObservable().Select(_ => Smoothing).Distinct();
            var characterControllerObservable = Observable.Return(GetComponentInParent<CharacterController>());
            var mouseXYObservable = InputObservables.GetMouseXyObservable(this.UpdateAsObservable());
            var mouseDeltaObservable = Observable.CombineLatest(
                sensitivityObservable,
                smoothingObservable,
                mouseXYObservable,
                (sensitivity, smoothing, mouseXY) =>
                    new Vector3(mouseXY.MouseX, mouseXY.MouseY) * sensitivity * smoothing
            );
            var mouseDeltaAndSmoothingObservable = Observable.CombineLatest(
                mouseDeltaObservable,
                smoothingObservable,
                (mouseDelta, smoothing) => new {mouseDelta, smoothing}
            );
            var smoothMouseDeltaObservable = mouseDeltaAndSmoothingObservable.Scan((acc, mouseDeltaAndSmoothing) =>
                {
                    var deltaX = Mathf.Lerp(acc.mouseDelta.x, mouseDeltaAndSmoothing.mouseDelta.x,
                        1f / mouseDeltaAndSmoothing.smoothing);

                    var deltaY = Mathf.Lerp(acc.mouseDelta.y, mouseDeltaAndSmoothing.mouseDelta.y,
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

            Observable.CombineLatest(
                    characterControllerObservable.Select(characterController => characterController.transform.up),
                    mouseLookObservable.Select(mouseLook => mouseLook.x),
                    (up, mouseLookX) =>
                        Quaternion.AngleAxis(mouseLookX, up)
                )
                .CombineLatest(characterControllerObservable,
                    (localRotation, characterController) => new {localRotation, characterController})
                .Subscribe(o => o.characterController.transform.localRotation = o.localRotation);

            mouseLookObservable.Select(mouseLook => Quaternion.AngleAxis(-mouseLook.y, Vector3.right))
                .Subscribe(localRotation => transform.localRotation = localRotation);
        }
    }
}
