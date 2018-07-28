using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Utils.BehaviorTree
{
    public class CompareSharedTransformPosition : Conditional
    {
        public SharedTransform variable;
        public SharedTransform compareTo;

        public override TaskStatus OnUpdate()
        {
            return variable.Value.position.Equals(compareTo.Value.position) ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}