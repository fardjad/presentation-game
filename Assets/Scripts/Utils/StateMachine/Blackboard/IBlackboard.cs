using System.Collections.Generic;

namespace Utils.StateMachine.Blackboard
{
    public interface IBlackboard
    {
        IDictionary<string, object> Parameters { get; }
    }
}