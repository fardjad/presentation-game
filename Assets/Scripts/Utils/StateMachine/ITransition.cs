using Utils.StateMachine.Conditions;

namespace Utils.StateMachine
{
    public interface ITransition
    {
        int Priority { get;  }
        bool MayInterrupt { get; }
        ICondition Condition { get; }
        IState To { get; }
    }
}
