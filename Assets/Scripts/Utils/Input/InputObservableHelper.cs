using UniRx;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Utils.Input
{
    public abstract class InputObservableHelper
    {
        private readonly IObservable<Unit> _updateObservable;

        public struct Hw
        {
            public float Horizontal;
            public float Vertical;
        }

        public struct MouseXy
        {
            public float MouseX;
            public float MouseY;
        }

        protected InputObservableHelper(IObservable<Unit> updateObservable)
        {
            _updateObservable = updateObservable;
        }

        public IObservable<bool> GetKeyDownObservable(KeyCode keyCode)
        {
            return _updateObservable
                .Select(_ => UnityEngine.Input.GetKeyDown(keyCode))
                .Where(isDown => isDown);
        }


        public IObservable<Hw> GetHwObservable()
        {
            return Observable.CombineLatest(
                GetHorizontalObservable(),
                GetVerticalObservable(),
                (horizontal, vertical) => new Hw
                {
                    Horizontal = horizontal,
                    Vertical = vertical
                });
        }

        private IObservable<float> GetHorizontalObservable()
        {
            return _updateObservable.Select(_ => CrossPlatformInputManager.GetAxis("Horizontal"));
        }

        private IObservable<float> GetVerticalObservable()
        {
            return _updateObservable.Select(_ => CrossPlatformInputManager.GetAxis("Vertical"));
        }

        private IObservable<float> GetMouseXObservable()
        {
            return _updateObservable.Select(_ => CrossPlatformInputManager.GetAxis("Mouse X"));
        }

        private IObservable<float> GetMouseYObservable()
        {
            return _updateObservable.Select(_ => CrossPlatformInputManager.GetAxis("Mouse Y"));
        }

        public IObservable<MouseXy> GetMouseXyObservable()
        {
            return Observable.CombineLatest(
                GetMouseXObservable(),
                GetMouseYObservable(),
                (mouseX, mouseY) => new MouseXy
                {
                    MouseX = mouseX,
                    MouseY = mouseY
                }
            );
        }
    }
}
