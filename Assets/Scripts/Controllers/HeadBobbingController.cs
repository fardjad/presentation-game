using System;
using JetBrains.Annotations;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils.Input;
using Zenject;

namespace Controllers
{
    public class HeadBobbingController : MonoBehaviour
    {
        private const float Tolerance = 10e-5f;

        [SerializeField] public float BobbingSpeed = 0.18f;
        [SerializeField] public float BobbingAmount = 0.2f;

        private InputObservableHelper _inputObservableHelper;


        [Inject]
        [UsedImplicitly]
        public void Construct(UpdateInputObservableHelper inputObservableHelper)
        {
            _inputObservableHelper = inputObservableHelper;
        }

        private void Start()
        {
            var initialLocalPositionObservable = Observable.Return(transform.localPosition);

            var isStandingStillObservable =
                _inputObservableHelper.GetHwObservable()
                    .Select(hw => Math.Abs(hw.Horizontal) < Tolerance && Math.Abs(hw.Vertical) < Tolerance);

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
                .CombineLatest(isStandingStillObservable, (timer, isStill) => isStill ? 0 : timer)
                .Select(Mathf.Sin)
                .Select(value => value * BobbingAmount);

            initialLocalPositionObservable
                .Sample(sineWaveObservable.Where(value => Math.Abs(value) < Tolerance))
                .Subscribe(initialLocalPosition => transform.localPosition = initialLocalPosition);

            var translateChangeObservable = sineWaveObservable.Select(value => value * BobbingAmount);

            var clampedSumOfAxesObservable = _inputObservableHelper.GetHwObservable()
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
}
