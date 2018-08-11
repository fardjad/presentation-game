namespace Utils.StateMachine.Conditions
{
    public class AlwaysTrueCondition : ICondition
    {
        public bool Check()
        {
            return true;
        }
    }
}