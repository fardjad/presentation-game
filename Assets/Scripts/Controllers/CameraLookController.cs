﻿using UniRx;
using UnityEngine;

public class CameraLookController : MonoBehaviour
{
    public float Sensitivity = 2f;
    public float Smoothing = 2f;

    private void Start()
    {
        var sensitivityObservable = Observable.Return(Sensitivity);
        var smoothingObservable = Observable.Return(Smoothing);
        var playerObservable = Observable.Return(GameObject.FindGameObjectWithTag("Player"));
        var inputObservables = Toolbox.Instance.GetComponent<InputObservables>();
        var mouseXYObservable = inputObservables.MouseXYObservable;
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
        var smoothMouseDeltaObservable = mouseDeltaAndSmoothingObservable.Scan((acc, mouseDeltaAndSmoothing) => new
            {
                mouseDelta = new Vector3(
                    Mathf.Lerp(acc.mouseDelta.x, mouseDeltaAndSmoothing.mouseDelta.x,
                        1f / mouseDeltaAndSmoothing.smoothing),
                    Mathf.Lerp(acc.mouseDelta.y, mouseDeltaAndSmoothing.mouseDelta.y,
                        1f / mouseDeltaAndSmoothing.smoothing)
                ),
                mouseDeltaAndSmoothing.smoothing
            })
            .Select(o => o.mouseDelta);
        var mouseLookObservable = smoothMouseDeltaObservable.Scan((acc, smoothMouseDelta) => acc + smoothMouseDelta);
        var clampedMouseLookObservable = mouseLookObservable.Select(mouseLook =>
            new Vector3(mouseLook.x, Mathf.Clamp(mouseLook.y, -50 /* down */, 70 /* up */))
        );

        Observable.CombineLatest(
                playerObservable.Select(player => player.transform.up),
                clampedMouseLookObservable.Select(mouseLook => mouseLook.x),
                (playerTransformUp, mouseLookX) =>
                    Quaternion.AngleAxis(mouseLookX, playerTransformUp)
            )
            .CombineLatest(playerObservable, (localRotation, player) => new {localRotation, player})
            .Subscribe(o => o.player.transform.localRotation = o.localRotation);

        clampedMouseLookObservable.Select(clampedMouseLook => Quaternion.AngleAxis(-clampedMouseLook.y, Vector3.right))
            .Subscribe(localRotation => transform.localRotation = localRotation);
    }
}