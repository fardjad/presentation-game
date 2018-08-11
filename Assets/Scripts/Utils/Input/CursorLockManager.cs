using JetBrains.Annotations;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace Utils.Input
{
    public class CursorLockManager : MonoBehaviour
    {
        private InputObservableHelper _inputObservableHelper;

        [Inject]
        [UsedImplicitly]
        public void Construct(UpdateInputObservableHelper inputObservableHelper)
        {
            _inputObservableHelper = inputObservableHelper;
        }

        private void Start()
        {
            var escapeObservable = _inputObservableHelper.GetKeyDownObservable(KeyCode.Escape);

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
}
