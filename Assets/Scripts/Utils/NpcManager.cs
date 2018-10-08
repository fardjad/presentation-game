using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using JetBrains.Annotations;
using UniRx;
using Utils.StateMachine.Blackboard;

namespace Utils
{
    [UsedImplicitly]
    public class NpcManager : IDisposable
    {
        private readonly ChairManager _chairManager;
        private readonly IDictionary<string, Tuple<int, int>> _npcChairDictionary;
        public IDictionary<string, NpcController> NpcControllerDictionary { get; private set; }

        private int _col = -1;
        private int _row = 0;
        private readonly ChairSpawnController.Settings _chairSpawnControllerSettings;

        public NpcManager(ChairManager chairManager, ChairSpawnController.Settings chairSpawnControllerSettings)
        {
            _chairSpawnControllerSettings = chairSpawnControllerSettings;
            _chairManager = chairManager;
            NpcControllerDictionary = new Dictionary<string, NpcController>();
            _npcChairDictionary = new Dictionary<string, Tuple<int, int>>();
        }

        public void RegisterNpcController(string id, NpcController npcController)
        {
            NextNumber();

            NpcControllerDictionary[id] = npcController;
            _npcChairDictionary[id] = new Tuple<int, int>(_row, _col);

            npcController.Blackboard.Parameters["_chairWalkToPosition"] =
                _chairManager.GetChairWalkToPosition(_row, _col);
            npcController.Blackboard.Parameters["_chairCenter"] = _chairManager.GetChairCenter(_row, _col);
            npcController.Blackboard.Parameters["_previousChairStandUpPosition"] = VectorBuilder
                .FromVector(_chairManager.GetChairWalkToPosition(_row, _col))
                .SetZ(_chairManager.GetChairWalkToPosition(_row, _col).z + 0.4f)
                .ToVector();
            ((StateMachineTrigger) npcController.Blackboard.Parameters["SitOnTheChair"]).Set();
        }

        public NpcController FindInPosition(int row, int col)
        {
            var tuple = new Tuple<int, int>(row, col);
            var reversedDictionary = _npcChairDictionary.ToDictionary(pair => pair.Value, pair => pair.Key);

            return !reversedDictionary.ContainsKey(tuple) ? null : NpcControllerDictionary[reversedDictionary[tuple]];
        }

        private void NextNumber()
        {
            _col += 1;
            UpdateRowCol();

            _chairSpawnControllerSettings.DisabledChairs.ForEach(rowCol =>
            {
                var r = rowCol.Row;
                var c = rowCol.Col;
                if (_row != r || _col != c) return;
                _col += 1;
                UpdateRowCol();
            });
        }

        private void UpdateRowCol()
        {
            if (_col == _chairManager.GetNumberOfCols())
            {
                _col = 0;
                _row += 1;
            }

            if (_row == _chairManager.GetNumberOfRows())
            {
                _row = 0;
            }
        }

        public void RandomlyRaiseHand()
        {
            PutAllHandsDown();
            var controllers = (from pair in NpcControllerDictionary select pair.Value).ToList();
            var rand = new Random();
            var toSkip = rand.Next(0, NpcControllerDictionary.Count);
            var randomNpcController = controllers.Skip(toSkip).Take(1).First();
            var npcTalkController = randomNpcController.gameObject.GetComponent<NpcTalkController>();
            npcTalkController.Blackboard.Parameters["RaiseHand"] = "true";
        }

        public void PutAllHandsDown()
        {
            var controllers = from pair in NpcControllerDictionary select pair.Value;
            controllers.Select(npcController => npcController.gameObject.GetComponent<NpcTalkController>().Blackboard)
                .ToList()
                .ForEach(
                    blackboard => { blackboard.Parameters["RaiseHand"] = "false"; });
        }

        public void Dispose()
        {
            _npcChairDictionary.Clear();
            NpcControllerDictionary.Clear();
        }
    }
}
