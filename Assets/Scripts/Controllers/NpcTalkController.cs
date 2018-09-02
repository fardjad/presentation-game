using System;
using UnityEngine;
using Utils.StateMachine;
using Utils.StateMachine.Blackboard;
using Utils.StateMachine.Parser;
using Random = System.Random;

namespace Controllers
{
    public class NpcTalkController : MonoBehaviour
    {
        private Animator _animator;
        public StateMachine StateMachine { get; private set; }
        public IBlackboard Blackboard { get; private set; }

        private void Awake()
        {
            InitializeStateMachine();
        }

        private void InitializeStateMachine()
        {
            Blackboard = new Blackboard();
            var stateMachineConfig = Resources.Load<TextAsset>("NpcTalkControllerStateMachine").text;
            var states = ConfigParser.GetStates(stateMachineConfig, Blackboard);
            StateMachine = new StateMachine(states);

            Blackboard.Parameters["IsRed"] = "false";
            Blackboard.Parameters["TalkToLeft"] = "false";
            Blackboard.Parameters["TalkToRight"] = "false";
        }

        private void Start()
        {
            _animator = GetComponent<Animator>();
            StateMachine.OnStateChanged += (sender, args) =>
            {
                var stateName = args.State.Name;

                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (stateName)
                {
                    case "Idle":
                        break;
                    case "PlayDistractedAnimation":
                        ResetAll();
                        var index = new Random().Next(1, 4);
                        var triggerName = string.Format("Red{0}", index);
                        _animator.SetTrigger(triggerName);
                        break;
                    case "PlaySittingAnimation":
                        ResetAll();
                        _animator.SetTrigger("ResetSit");
                        break;
                    case "TalkToLeft":
                        ResetAll();
                        _animator.SetTrigger("TalkToLeft");
                        break;
                    case "TalkToRight":
                        ResetAll();
                        _animator.SetTrigger("TalkToRight");
                        break;
                }
            };
        }

        private void Update()
        {
            StateMachine.Tick(TimeSpan.FromSeconds(Time.deltaTime));
        }

        private void ResetAll()
        {
            _animator.ResetTrigger("ResetSit");
            _animator.ResetTrigger("TalkToLeft");
            _animator.ResetTrigger("TalkToRight");
            _animator.ResetTrigger("Yellow1");
            _animator.ResetTrigger("Yellow2");
            _animator.ResetTrigger("Red1");
            _animator.ResetTrigger("Red2");
            _animator.ResetTrigger("Red3");
        }
    }
}
