using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Utils.BehaviorTree
{
    public class SetNPCNavMeshAgentState : Action
    {
        public SharedGameObject NPC;
        public SharedBool shouldPause;

        private NavMeshAgent _agent;
        private CharacterController _controller;

        public override void OnStart()
        {
            _agent = NPC.Value.GetComponent<NavMeshAgent>();
        }

        public override TaskStatus OnUpdate()
        {
            _agent.isStopped = shouldPause.Value;

            return TaskStatus.Success;
        }
    }
}