﻿using Utils.StateMachine.Blackboard;

namespace Utils.StateMachine.Conditions
{
    public class ValueInRange : BlackboardAwareCondition
    {
        private readonly double _lowerBound;
        private readonly string _parameterName;
        private readonly double _upperBound;

        public ValueInRange(IBlackboard blackboard, string parameterName, double lowerBound, double upperBound) :
            base(blackboard)
        {
            _parameterName = parameterName;
            _lowerBound = lowerBound;
            _upperBound = upperBound;
        }

        public override bool Check()
        {
            return (double) Blackboard.Parameters[_parameterName] <= _upperBound &&
                   (double) Blackboard.Parameters[_parameterName] >= _lowerBound;
        }
    }
}