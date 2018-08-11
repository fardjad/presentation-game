using Utils.StateMachine.Conditions;

namespace Utils.StateMachine
{
    public class Transition : ITransition
    {
        public Transition(ICondition condition, IState to)
        {
            Condition = condition;
            To = to;
        }

        public ICondition Condition { get; private set; }
        public IState To { get; private set; }
    }
}
