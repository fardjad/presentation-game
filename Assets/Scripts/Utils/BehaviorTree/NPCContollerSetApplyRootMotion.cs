using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Controllers;

namespace Utils.BehaviorTree
{
    public class NPCContollerSetApplyRootMotion : Action
    {
        public SharedGameObject npc;
        public SharedBool value;

        private NPCController _npcController;

        public override void OnStart()
        {
            _npcController = npc.Value.GetComponent<NPCController>();
        }
    
        public override TaskStatus OnUpdate()
        {
            _npcController.SetApplyRootMotion(value.Value);
            return TaskStatus.Success;
        }
    }
}