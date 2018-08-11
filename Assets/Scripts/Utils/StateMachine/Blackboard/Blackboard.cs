using System.Collections.Generic;

namespace Utils.StateMachine.Blackboard
{
    public class Blackboard : IBlackboard
    {
        public Blackboard()
        {
            Parameters = new Dictionary<string, object>();
        }

        public IDictionary<string, object> Parameters { get; private set; }
    }
}