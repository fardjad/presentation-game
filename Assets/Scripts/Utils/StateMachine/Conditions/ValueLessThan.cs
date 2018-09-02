using Utils.StateMachine.Blackboard;

namespace Utils.StateMachine.Conditions
{
    public class ValueLessThan : BlackboardAwareCondition
    {
        private readonly string _parameterName;
        private readonly double _upperBound;

        public ValueLessThan(IBlackboard blackboard, string parameterName, float upperBound) : base(blackboard)
        {
            _parameterName = parameterName;
            _upperBound = upperBound;
        }

        public override bool Check()
        {
            return (float) Blackboard.Parameters[_parameterName] < _upperBound;
        }
    }
}
