using Utils.StateMachine.Conditions;

namespace Utils.StateMachine
{
    public interface ITransition
    {
        ICondition Condition { get; }
        IState To { get; }
    }
}
