using System;
using System.Collections.Generic;

namespace Utils.StateMachine
{
    public interface IState
    {
        string Name { get; }
        TimeSpan Length { get; }
        List<ITransition> Transitions { get; }
    }
}
