using UniRx;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public float Speed = 0.5f;

    private void Start()
    {
        setParentConstraints();

        var animatorObservable = Observable.Return(GetComponentInChildren<Animator>());
        var speedObservable = Observable.Return(Speed);
        var inputObservables = Toolbox.Instance.GetComponent<InputObservables>();
        var hwObservable = inputObservables.HwObservable;
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
                (animator, animatorParams) => new {animator, animatorParams}
            )
            .Subscribe(o =>
            {
                o.animator.SetBool("IsWalking", o.animatorParams.isWalking);
                o.animator.SetFloat("Walk", o.animatorParams.walk);
                o.animator.SetBool("IsStrafing", o.animatorParams.isOnlyStrafing);
                o.animator.SetFloat("Strafe", o.animatorParams.strafe);
            });

        Observable.CombineLatest(
                speedObservable,
                animatorParamsObservable,
                (speed, animatorParams) =>
                    new Vector3(animatorParams.strafe, 0, animatorParams.walk) * speed * Time.deltaTime)
            .Subscribe(vector =>
                transform.Translate(vector));
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