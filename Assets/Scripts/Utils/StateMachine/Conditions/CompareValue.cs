using Utils.StateMachine.Blackboard;

namespace Utils.StateMachine.Conditions
{
    public class CompareValue : BlackboardAwareCondition
    {
        private readonly string _expectedValue;
        private readonly string _parameterName;

        public CompareValue(IBlackboard blackboard, string parameterName, string expectedValue) : base(blackboard)
        {
            _parameterName = parameterName;
            _expectedValue = expectedValue;
        }

        public override bool Check()
        {
            return Blackboard.Parameters[_parameterName].Equals(_expectedValue);
        }
    }
}