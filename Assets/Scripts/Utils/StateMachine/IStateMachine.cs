using System;

namespace Utils.StateMachine
{
    public interface IStateMachine
    {
        IState CurrentState { get; }
        event EventHandler<StateEventArgs> OnStateChanged;
        void Tick(TimeSpan deltaTime);
    }
}
