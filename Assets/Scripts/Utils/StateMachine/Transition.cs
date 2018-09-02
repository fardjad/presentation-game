using Utils.StateMachine.Conditions;

namespace Utils.StateMachine
{
    public class Transition : ITransition
    {
        public Transition(ICondition condition, IState to, int priority = 0, bool mayInterrupt = false)
        {
            Condition = condition;
            To = to;
            Priority = priority;
            MayInterrupt = mayInterrupt;
        }

        public int Priority { get; private set; }
        public bool MayInterrupt { get; private set; }
        public ICondition Condition { get; private set; }
        public IState To { get; private set; }
    }
}
