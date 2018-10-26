using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Controllers
{
    public class ClassDoorController : MonoBehaviour
    {
        private void Start()
        {
            var animator = transform.parent.GetComponentInChildren<Animator>();

            var onTriggerEnterObservable = this.OnTriggerEnterAsObservable()
                .Where(other => other.CompareTag("Player"));

            onTriggerEnterObservable
                .Subscribe(_ => { animator.SetBool("IsOpen", true); });

            var onTriggerExitObservable = this.OnTriggerExitAsObservable()
                .Where(other => other.CompareTag("Player"));

            var closeRequestsObservable = onTriggerExitObservable.Select(_ => -1);
            var openRequestsObservable = onTriggerEnterObservable.Select(_ => 1);
            var pendingOpenRequestsObservable =
                openRequestsObservable.Merge(closeRequestsObservable).Scan((acc, value) => acc + value);

            onTriggerExitObservable
                .Delay(TimeSpan.FromSeconds(1))
                .CombineLatest(pendingOpenRequestsObservable, (_, pendingOpenRequests) => pendingOpenRequests)
                .Where(pendingOpenRequests => pendingOpenRequests == 0)
                .Subscribe(_ =>
                {
                    if (animator != null) animator.SetBool("IsOpen", false);
                });
        }
    }
}