using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class CursorLockController : MonoBehaviour
{
    private void Start()
    {
        var escapeObservable = InputObservables.GetKeyDownObservable(this.UpdateAsObservable(), KeyCode.Escape);

        gameObject.UpdateAsObservable()
            .Select(_ => CursorLockMode.Locked)
            .TakeUntil(escapeObservable)
            .Distinct()
            .Subscribe(lockState => Cursor.lockState = lockState);

        escapeObservable
            .Select(_ => CursorLockMode.None)
            .Subscribe(lockState => Cursor.lockState = lockState);
    }
}