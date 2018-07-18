using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class HeadBobbingController : MonoBehaviour
{
    public float BobbingSpeed = 0.18f;
    public float BobbingAmount = 0.2f;

    private void Start()
    {
        var initialLocalPositionObservable = Observable.Return(transform.localPosition);

        var isStillObservable =
            InputObservables.GetHwObservable(this.UpdateAsObservable())
                .Select(hw => hw.Horizontal == 0 && hw.Vertical == 0);

        var sineWaveObservable = this.UpdateAsObservable()
            .TimeInterval()
            .Select(ti => BobbingSpeed * (float) ti.Interval.TotalSeconds)
            .Scan((acc, value) =>
            {
                if (acc + value > 2 * Mathf.PI)
                {
                    return acc + value - 2 * Mathf.PI;
                }

                return acc + value;
            })
            .CombineLatest(isStillObservable, (timer, isStill) => isStill ? 0 : timer)
            .Select(Mathf.Sin)
            .Select(value => value * BobbingAmount);

        initialLocalPositionObservable
            .Sample(sineWaveObservable.Where(value => value == 0))
            .Subscribe(initialLocalPosition => transform.localPosition = initialLocalPosition);

        var translateChangeObservable = sineWaveObservable.Select(value => value * BobbingAmount);

        var clampedSumOfAxesObservable = InputObservables.GetHwObservable(this.UpdateAsObservable())
            .Select(hw => Mathf.Abs(hw.Horizontal) + Mathf.Abs(hw.Vertical))
            .Select(sumOfAxes => Mathf.Clamp(sumOfAxes, 0f, 1f));

        Observable.CombineLatest(
                translateChangeObservable,
                clampedSumOfAxesObservable,
                initialLocalPositionObservable,
                (translateChange, clampedSumOfAxes, initialLocalPosition) =>
                    new Vector3(0, translateChange * clampedSumOfAxes, 0) + initialLocalPosition
            )
            .Subscribe(localPosition => transform.localPosition = localPosition);
    }
}