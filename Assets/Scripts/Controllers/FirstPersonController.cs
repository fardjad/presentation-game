using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils.Input;

namespace Controllers
{
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] public float Speed = 0.5f;

        private void Start()
        {
            setParentConstraints();

            var characterControllerObservable = Observable.Return(GetComponent<CharacterController>());
            var animatorObservable = Observable.Return(GetComponentInChildren<Animator>());
            var speedObservable = this.UpdateAsObservable().Select(_ => Speed).Distinct();
            var hwObservable = InputObservables.GetHwObservable(this.UpdateAsObservable());
            var transformObservable = this.UpdateAsObservable().Select(_ => transform);

            Observable.CombineLatest(
                    characterControllerObservable,
                    speedObservable,
                    hwObservable,
                    transformObservable,
                    (characterController, speed, hw, transform) => new
                    {
                        characterController,
                        speed,
                        hw,
                        transform
                    })
                .Subscribe(o =>
                {
                    o.characterController.SimpleMove(o.transform.forward * o.hw.Vertical * o.speed +
                                                     o.transform.right * o.hw.Horizontal * o.speed);
                });

            Observable.CombineLatest(
                    animatorObservable,
                    speedObservable,
                    hwObservable,
                    (animator, speed, hw) => new
                    {
                        animator,
                        speed,
                        hw
                    })
                .Subscribe(o =>
                {
                    o.animator.SetFloat("Horizontal", o.hw.Horizontal, 0.1f, Time.deltaTime);
                    o.animator.SetFloat("Vertical", o.hw.Vertical, 0.1f, Time.deltaTime);
                    o.animator.speed =
                        o.speed * 2f; // if chacarcter movement speed is 0.5, then animation must play at full speed
                });
        }

        private void setParentConstraints()
        {
            var parentRigidBodyObservable = Observable.Return(gameObject.GetComponentInParent<Rigidbody>());
            parentRigidBodyObservable.SkipWhile(parentRigidBody => parentRigidBody == null)
                .Subscribe(parentRigidBody =>
                {
                    parentRigidBody.constraints =
                        RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationZ;
                });
        }
    }
}
