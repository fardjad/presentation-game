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
    public class FirstPersonController : MonoBehaviour
    {
        private List<IDisposable> _disposables;
        private InputObservableHelper _inputObservableHelper;
        [SerializeField] public float Speed = 0.5f;

        [Inject]
        [UsedImplicitly]
        public void Construct(UpdateInputObservableHelper inputObservableHelper)
        {
            _inputObservableHelper = inputObservableHelper;
        }

        private void Start()
        {
            _disposables = new List<IDisposable>();

            var speedObservable = this.UpdateAsObservable().Select(_ => Speed).DistinctUntilChanged();
            var hwObservable = _inputObservableHelper.GetHwObservable();

            var characterController = GetComponentInChildren<CharacterController>();
            var moveDisposable = speedObservable.CombineLatest(hwObservable,
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
            _disposables.Add(moveDisposable);

            var animator = GetComponentInChildren<Animator>();
            var animatorDisposable = speedObservable.CombineLatest(hwObservable,
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
            _disposables.Add(animatorDisposable);
        }

        private void OnDestroy()
        {
            _disposables.ToList().ForEach(d => d.Dispose());
        }
    }
}