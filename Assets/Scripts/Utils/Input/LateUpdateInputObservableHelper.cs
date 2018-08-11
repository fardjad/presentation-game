using JetBrains.Annotations;
using UniRx;

namespace Utils.Input
{
    [UsedImplicitly]
    public sealed class LateUpdateInputObservableHelper : InputObservableHelper
    {
        public LateUpdateInputObservableHelper(IObservable<Unit> updateObservable) : base(updateObservable)
        {
        }
    }
}
