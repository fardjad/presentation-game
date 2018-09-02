using System;
using System.Collections.Generic;
using System.Linq;

namespace Utils.StateMachine
{
    public class StateMachine : IStateMachine
    {
        private readonly List<IState> _states;

        private TimeSpan _elapsedTimeForCurrentState;

        public StateMachine(IEnumerable<IState> states)
        {
            var statesList = states.ToList();
            if (statesList.Count == 0) throw new ArgumentException("states should not be empty!");

            _states = statesList;
            CurrentState = statesList.First();

            _elapsedTimeForCurrentState = TimeSpan.Zero;
        }

        public IState CurrentState { get; private set; }
        public event EventHandler<StateEventArgs> OnStateChanged;

        public void Tick(TimeSpan deltaTime)
        {
            _elapsedTimeForCurrentState += deltaTime;
            var nextState = GetNextState(CurrentState.Transitions, _states, _elapsedTimeForCurrentState < CurrentState.Length);

            if (nextState == null) return;

            CurrentState = nextState;
            _elapsedTimeForCurrentState = TimeSpan.Zero;
            if (OnStateChanged != null) OnStateChanged.Invoke(this, new StateEventArgs(CurrentState));
        }

        private static IState GetNextState(IEnumerable<ITransition> transitions, ICollection<IState> states, bool excludeNonInterruptingTransitions)
        {
            return transitions
                .OrderBy(transition => transition.Priority)
                .Where(transition => !excludeNonInterruptingTransitions || transition.MayInterrupt)
                .Where(transition => transition.Condition.Check())
                .Select(transition => transition.To)
                .Where(states.Contains)
                .DefaultIfEmpty(null)
                .First();
        }
    }
}
