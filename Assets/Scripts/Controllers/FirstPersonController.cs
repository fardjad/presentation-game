using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public float Speed = 0.5f;

    private void Start()
    {
        setParentConstraints();

        var characterControllerObservable = Observable.Return(GetComponent<CharacterController>());
        var animatorObservable = Observable.Return(GetComponentInChildren<Animator>());
        var speedObservable = Observable.Return(Speed);
        var hwObservable = InputObservables.GetHwObservable(this.UpdateAsObservable());
        var animatorParamsObservable = hwObservable.Select(hw => new
        {
            isWalking = hw.Vertical != 0,
            walk = hw.Vertical,
            isStrafing = hw.Horizontal != 0,
            isOnlyStrafing = hw.Vertical == 0 && hw.Horizontal != 0,
            strafe = hw.Horizontal
        });

        Observable.CombineLatest(
                animatorObservable,
                animatorParamsObservable,
                characterControllerObservable,
                (animator, animatorParams, characterController) => new {animator, animatorParams, characterController}
            )
            .Subscribe(o =>
            {
                // TODO: Calculate the threshold in a more theoretically justifiable manner
                var isStuck = o.characterController.velocity.magnitude < 0.1;

                o.animator.SetBool("IsWalking", !isStuck && o.animatorParams.isWalking);
                o.animator.SetFloat("Walk", o.animatorParams.walk);
                o.animator.SetBool("IsStrafing", !isStuck && o.animatorParams.isOnlyStrafing);
                o.animator.SetFloat("Strafe", o.animatorParams.strafe);
            });

        Observable.CombineLatest(
                speedObservable,
                animatorParamsObservable,
                characterControllerObservable,
                (speed, animatorParams, characterController) => new
                {
                    speedSide = animatorParams.strafe * speed,
                    speedForward = animatorParams.walk * speed,
                    characterController
                })
            .Subscribe(o =>
                o.characterController.SimpleMove(transform.forward * o.speedForward +
                                                 transform.right * o.speedSide));
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