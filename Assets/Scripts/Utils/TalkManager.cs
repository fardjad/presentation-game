using System.Collections.Generic;
using System.Linq;
using Controllers;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using Zenject;

namespace Utils
{
    public class TalkManager : ITickable
    {
        private NpcManager _npcManager;
        private ChairManager _chairManager;

        private float _elapsedTime = 0;

        [UsedImplicitly]
        [Inject]
        private void Construct(NpcManager npcManager, ChairManager chairManager)
        {
            _chairManager = chairManager;
            _npcManager = npcManager;
        }

        public void Tick()
        {
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime < 5) return;
            _elapsedTime = 0;

            for (var i = 0; i < _chairManager.GetNumberOfRows(); i += 1)
            {
                var cannotTalkSet = new HashSet<int>();
                var canTalkList = new List<Tuple<int, int>>();

                for (var j = 0; j < _chairManager.GetNumberOfCols() - 1; j += 1)
                {
                    var npc1 = _npcManager.FindInPosition(i, j);
                    var npc2 = _npcManager.FindInPosition(i, j + 1);

                    if (npc1 == null || npc2 == null) continue;

                    cannotTalkSet.Add(j);
                    cannotTalkSet.Add(j + 1);

                    if (!CanTalk(npc1, npc2)) continue;

                    canTalkList.Add(new Tuple<int, int>(j, j + 1));
                    j += 1;
                }

                canTalkList.SelectMany(tuple => new List<int> {tuple.Item1, tuple.Item2})
                    .Distinct()
                    .ToList()
                    .ForEach(
                        col => { cannotTalkSet.Remove(col); });

                foreach (var rowCol in canTalkList)
                {
                    var col1 = rowCol.Item1;
                    var col2 = rowCol.Item2;

                    var npc1 = _npcManager.FindInPosition(i, col1);
                    var npc2 = _npcManager.FindInPosition(i, col2);
                    var talkController1 = npc1.gameObject.GetComponent<NpcTalkController>();
                    var talkController2 = npc2.gameObject.GetComponent<NpcTalkController>();

                    talkController1.Blackboard.Parameters["TalkToRight"] = "true";
                    talkController1.Blackboard.Parameters["TalkToLeft"] = "false";

                    talkController2.Blackboard.Parameters["TalkToLeft"] = "true";
                    talkController2.Blackboard.Parameters["TalkToRight"] = "false";
                }

                foreach (var col in cannotTalkSet)
                {
                    var npc = _npcManager.FindInPosition(i, col);
                    var talkController = npc.gameObject.GetComponent<NpcTalkController>();
                    talkController.Blackboard.Parameters["TalkToRight"] = "false";
                    talkController.Blackboard.Parameters["TalkToLeft"] = "false";
                }
            }
        }

        private static bool CanTalk(NpcController npc1, NpcController npc2)
        {
            var sm1 = npc1.StateMachine;
            var sm2 = npc2.StateMachine;

            var attention1 = (float) npc1.gameObject.GetComponent<NpcAttentionController>()
                .Blackboard.Parameters["Attention"];
            var attention2 = (float) npc2.gameObject.GetComponent<NpcAttentionController>()
                .Blackboard.Parameters["Attention"];

            return sm1.CurrentState.Name.Equals("SittingOnTheChair") &&
                   sm2.CurrentState.Name.Equals("SittingOnTheChair") &&
                   (attention1 < 0.2f && attention2 < 0.2f);
        }
    }
}
