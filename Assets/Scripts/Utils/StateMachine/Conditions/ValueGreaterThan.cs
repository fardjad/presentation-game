﻿using Utils.StateMachine.Blackboard;

namespace Utils.StateMachine.Conditions
{
    public class ValueGreaterThan : BlackboardAwareCondition
    {
        private readonly double _lowerBound;
        private readonly string _parameterName;

        public ValueGreaterThan(IBlackboard blackboard, string parameterName, double lowerBound) : base(blackboard)
        {
            _parameterName = parameterName;
            _lowerBound = lowerBound;
        }

        public override bool Check()
        {
            return (double) Blackboard.Parameters[_parameterName] > _lowerBound;
        }
    }
}