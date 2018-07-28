using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

namespace Utils.BehaviorTree
{
    public class MoveNPCToDestination : Action
    {
        public SharedGameObject NPC;
        public SharedTransform Target;

        private NavMeshAgent _agent;

        public override void OnStart()
        {
            _agent = NPC.Value.GetComponentInChildren<NavMeshAgent>();
        }

        public override TaskStatus OnUpdate()
        {
            _agent.SetDestination(Target.Value.position);
            return TaskStatus.Success;
        }
    }
}