﻿using UniRx;
using UnityEngine;

public static class InputObservables
{
    public struct HW
    {
        public float Horizontal;
        public float Vertical;
    }

    public struct MouseXY
    {
        public float MouseX;
        public float MouseY;
    }

    public static IObservable<bool> GetKeyDownObservable(IObservable<Unit> updateObservable, KeyCode keyCode)
    {
        return updateObservable
            .Select(_ => Input.GetKeyDown(keyCode))
            .Where(isDown => isDown);
    }


    public static IObservable<HW> GetHwObservable(IObservable<Unit> updateObservable)
    {
        return Observable.CombineLatest(
            getHorizontalObservable(updateObservable),
            getVerticalObservable(updateObservable),
            (horizontal, vertical) => new HW
            {
                Horizontal = horizontal,
                Vertical = vertical
            });
    }

    private static IObservable<float> getHorizontalObservable(IObservable<Unit> updateObservable)
    {
        return updateObservable.Select(_ => Input.GetAxis("Horizontal"));
    }

    private static IObservable<float> getVerticalObservable(IObservable<Unit> updateObservable)
    {
        return updateObservable.Select(_ => Input.GetAxis("Vertical"));
    }

    private static IObservable<float> getMouseXObservable(IObservable<Unit> updateObservable)
    {
        return updateObservable.Select(_ => Input.GetAxis("Mouse X"));
    }

    private static IObservable<float> getMouseYObservable(IObservable<Unit> updateObservable)
    {
        return updateObservable.Select(_ => Input.GetAxis("Mouse Y"));
    }

    public static IObservable<MouseXY> GetMouseXyObservable(IObservable<Unit> updateObservable)
    {
        return Observable.CombineLatest(
            getMouseXObservable(updateObservable),
            getMouseYObservable(updateObservable),
            (mouseX, mouseY) => new MouseXY
            {
                MouseX = mouseX,
                MouseY = mouseY
            }
        );
    }
}