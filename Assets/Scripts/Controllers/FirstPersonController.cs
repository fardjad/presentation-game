using JetBrains.Annotations;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils.Input;
using Zenject;

namespace Controllers
{
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] public float Speed = 0.5f;

        private InputObservableHelper _inputObservableHelper;

        [Inject]
        [UsedImplicitly]
        public void Construct(UpdateInputObservableHelper inputObservableHelper)
        {
            _inputObservableHelper = inputObservableHelper;
        }

        private void Start()
        {
            var speedObservable = this.UpdateAsObservable().Select(_ => Speed).DistinctUntilChanged();
            var hwObservable = _inputObservableHelper.GetHwObservable();

            var characterController = GetComponentInChildren<CharacterController>();
            Observable.CombineLatest(
                    speedObservable,
                    hwObservable,
                    (speed, hw) => new
                    {
                        speed,
                        hw
                    })
                .Subscribe(o =>
                {
                    characterController.SimpleMove(transform.forward * o.hw.Vertical * o.speed +
                                                   transform.right * o.hw.Horizontal * o.speed);
                });

            var animator = GetComponentInChildren<Animator>();
            Observable.CombineLatest(
                    speedObservable,
                    hwObservable,
                    (speed, hw) => new
                    {
                        speed,
                        hw
                    })
                .Subscribe(o =>
                {
                    animator.SetFloat("Horizontal", o.hw.Horizontal, 0.1f, Time.deltaTime);
                    animator.SetFloat("Vertical", o.hw.Vertical, 0.1f, Time.deltaTime);
                    animator.speed =
                        o.speed * 2f; // if character movement speed is 0.5, then animation must play at full speed
                });
        }
    }
}
