using System;
using System.Collections.Generic;

namespace Utils.StateMachine
{
    public class State : IState
    {
        public State(string name, TimeSpan length)
        {
            Name = name;
            Length = length;
            Transitions = new List<ITransition>();
        }

        public string Name { get; private set; }
        public TimeSpan Length { get; private set; }
        public List<ITransition> Transitions { get; private set; }

        public override string ToString()
        {
            return Name;
        }
    }
}