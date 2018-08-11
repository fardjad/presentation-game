using System.Collections.Generic;
using Controllers;
using JetBrains.Annotations;
using UnityEngine;
using Utils.StateMachine.Blackboard;

namespace Utils
{
    [UsedImplicitly]
    public class NpcManager : INpcManager
    {
        private readonly ChairManager _chairManager;
        private readonly IDictionary<string, NpcController> _npcControllerDictionary;

        private int _row;
        private int _col;

        public NpcManager(ChairManager chairManager)
        {
            _chairManager = chairManager;
            _npcControllerDictionary = new Dictionary<string, NpcController>();
        }

        public void RegisterNpcController(string id, NpcController npcController)
        {
            _npcControllerDictionary[id] = npcController;

            npcController.Blackboard.Parameters["SitOnTheChair"] = new StateMachineTrigger();
            npcController.Blackboard.Parameters["StandUp"] = new StateMachineTrigger();
            npcController.Blackboard.Parameters["WalkToDestination"] = new StateMachineTrigger();

            npcController.Blackboard.Parameters["_chairWalkToPosition"] =
                _chairManager.GetChairWalkToPosition(_row, _col);
            npcController.Blackboard.Parameters["_chairStandUpPosition"] = VectorBuilder
                .FromVector(_chairManager.GetChairWalkToPosition(_row, _col))
                .SetZ(_chairManager.GetChairWalkToPosition(_row, _col).z + 0.4f)
                .ToVector();
            npcController.Blackboard.Parameters["_chairCenter"] = _chairManager.GetChairCenter(_row, _col);
            ((StateMachineTrigger) npcController.Blackboard.Parameters["SitOnTheChair"]).Set();

            npcController.Blackboard.Parameters["_destination"] = new Vector3(0f, 0f, 0f);

            _col += 1;
            if (_col == 4)
            {
                _col = 0;
                _row += 1;
            }

            if (_row == 2 && _col == 0)
            {
                _col += 1;
            }
        }
    }
}
