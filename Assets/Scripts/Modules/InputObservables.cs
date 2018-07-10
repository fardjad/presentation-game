using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class InputObservables : MonoBehaviour
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

    public IObservable<bool> EscapeObservable { get; private set; }
    public IObservable<HW> HwObservable { get; private set; }
    public IObservable<MouseXY> MouseXYObservable { get; private set; }

    private void Awake()
    {
        var horizontalObservable = gameObject.UpdateAsObservable().Select(_ => Input.GetAxis("Horizontal"));
        var verticalObservable = gameObject.UpdateAsObservable().Select(_ => Input.GetAxis("Vertical"));
        HwObservable = Observable.CombineLatest(
            horizontalObservable,
            verticalObservable,
            (horizontal, vertical) => new HW
            {
                Horizontal = horizontal,
                Vertical = vertical
            });

        var mouseXObservable = gameObject.UpdateAsObservable().Select(_ => Input.GetAxis("Mouse X"));
        var mouseYObservable = gameObject.UpdateAsObservable().Select(_ => Input.GetAxis("Mouse Y"));
        MouseXYObservable = Observable.CombineLatest(
            mouseXObservable,
            mouseYObservable,
            (mouseX, mouseY) => new MouseXY
            {
                MouseX = mouseX,
                MouseY = mouseY
            }
        );

        EscapeObservable = gameObject.UpdateAsObservable()
            .Select(_ => Input.GetKeyDown(KeyCode.Escape))
            .Where(isDown => isDown);
    }
}