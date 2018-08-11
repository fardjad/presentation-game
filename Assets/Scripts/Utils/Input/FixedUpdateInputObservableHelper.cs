using JetBrains.Annotations;
using UniRx;

namespace Utils.Input
{
    [UsedImplicitly]
    public sealed class FixedUpdateInputObservableHelper : InputObservableHelper
    {
        public FixedUpdateInputObservableHelper(IObservable<Unit> updateObservable) : base(updateObservable)
        {
        }
    }
}
