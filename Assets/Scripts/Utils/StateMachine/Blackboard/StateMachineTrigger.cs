namespace Utils.StateMachine.Blackboard
{
    public class StateMachineTrigger
    {
        private bool _value;

        public void Set()
        {
            _value = true;
        }

        public void Reset()
        {
            _value = false;
        }

        public bool IsSet()
        {
            return _value;
        }
    }
}
