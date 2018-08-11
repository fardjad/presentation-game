using Utils.StateMachine.Blackboard;

namespace Utils.StateMachine.Conditions
{
    public class ValueLessThan : BlackboardAwareCondition
    {
        private readonly string _parameterName;
        private readonly double _upperBound;

        public ValueLessThan(IBlackboard blackboard, string parameterName, double upperBound) : base(blackboard)
        {
            _parameterName = parameterName;
            _upperBound = upperBound;
        }

        public override bool Check()
        {
            return (double) Blackboard.Parameters[_parameterName] < _upperBound;
        }
    }
}