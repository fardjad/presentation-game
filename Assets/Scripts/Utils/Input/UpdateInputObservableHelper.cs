using JetBrains.Annotations;
using UniRx;

namespace Utils.Input
{
    [UsedImplicitly]
    public sealed class UpdateInputObservableHelper : InputObservableHelper
    {
        public UpdateInputObservableHelper(IObservable<Unit> updateObservable) : base(updateObservable)
        {
        }
    }
}
