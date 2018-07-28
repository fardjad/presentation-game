using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

namespace Utils.BehaviorTree
{
    public class GetNavMeshAgentStoppingDistance : Action
    {
        public SharedGameObject NPC;
        public SharedFloat TargetValue;

        private NavMeshAgent _agent;

        public override void OnStart()
        {
            _agent = NPC.Value.GetComponent<NavMeshAgent>();
        }

        public override TaskStatus OnUpdate()
        {
            TargetValue.SetValue(_agent.stoppingDistance);
            return TaskStatus.Success;
        }
    }
}