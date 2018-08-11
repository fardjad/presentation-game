using Utils.StateMachine.Blackboard;

namespace Utils.StateMachine.Conditions
{
    public abstract class BlackboardAwareCondition : ICondition
    {
        protected readonly IBlackboard Blackboard;

        protected BlackboardAwareCondition(IBlackboard blackboard)
        {
            Blackboard = blackboard;
        }

        public abstract bool Check();
    }
}