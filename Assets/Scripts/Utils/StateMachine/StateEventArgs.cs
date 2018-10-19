using System;

namespace Utils.StateMachine
{
    public class StateEventArgs : EventArgs
    {
        public StateEventArgs(IState state)
        {
            State = state;
        }

        public IState State { get; private set; }
    }
}