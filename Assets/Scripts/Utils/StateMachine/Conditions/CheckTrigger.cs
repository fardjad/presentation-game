using Utils.StateMachine.Blackboard;

namespace Utils.StateMachine.Conditions
{
    public class CheckTrigger : BlackboardAwareCondition
    {
        private readonly string _parameterName;

        public CheckTrigger(IBlackboard blackboard, string parameterName) : base(blackboard)
        {
            _parameterName = parameterName;
        }

        public override bool Check()
        {
            var trigger = Blackboard.Parameters[_parameterName] as StateMachineTrigger;
            if (trigger == null || !trigger.IsSet())
                return false;
            trigger.Reset();
            return true;
        }
    }
}