using System;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utils.StateMachine;
using Utils.StateMachine.Blackboard;
using Utils.StateMachine.Parser;
using Random = System.Random;

namespace Controllers
{
    public class NpcAttentionController : MonoBehaviour
    {
        private Image _image;
        public StateMachine StateMachine { get; private set; }
        public IBlackboard Blackboard { get; private set; }
        private NpcController _npcController;
        private NpcTalkController _npcTalkController;
        private Animator _animator;
        private ISubject<string> _currentStateSubject;
        private IDisposable _currentStateSubjectDisposable;

        private void Awake()
        {
            _npcController = GetComponent<NpcController>();
            _npcTalkController = GetComponent<NpcTalkController>();
            InitializeStateMachine();
        }

        private void InitializeStateMachine()
        {
            _animator = GetComponent<Animator>();
            Blackboard = new Blackboard();
            var stateMachineConfig = Resources.Load<TextAsset>("NpcAttentionControllerStateMachine").text;
            var states = ConfigParser.GetStates(stateMachineConfig, Blackboard);
            StateMachine = new StateMachine(states);

            Blackboard.Parameters["PlayerIsLookingAtNpc"] = "false";
            Blackboard.Parameters["Attention"] = 1f;
            Blackboard.Parameters["SittingOnTheChair"] = "false";
        }

        private void Start()
        {
            _currentStateSubject = new Subject<string>();

            var npcControllerStateMachine = _npcController.StateMachine;
            npcControllerStateMachine.OnStateChanged += (sender, args) =>
            {
                var state = args.State;
                if (state.Name.Equals("SittingOnTheChair"))
                {
                    Blackboard.Parameters["SittingOnTheChair"] = "true";
                    Blackboard.Parameters["Attention"] = 1f;
                }
                else if (state.Name.Equals("StandUp"))
                {
                    Blackboard.Parameters["SittingOnTheChair"] = "false";
                }
            };

            _currentStateSubjectDisposable = _currentStateSubject.Sample(TimeSpan.FromSeconds(5))
                .Subscribe(state =>
                {
                    // ReSharper disable once ConvertIfStatementToSwitchStatement
                    if (state == "Yellow")
                    {
                        ResetAll();
                        var index = new Random().Next(1, 3);
                        var triggerName = string.Format("Yellow{0}", index);
                        _animator.SetTrigger(triggerName);
                    }
                    else if (state == "Green")
                    {
                        ResetAll();
                    }
                });
        }

        // Update is called once per frame
        private void Update()
        {
            var npcTalkControllerBlackboard = _npcTalkController.Blackboard;

            StateMachine.Tick(TimeSpan.FromSeconds(Time.deltaTime));

            if (_image == null)
            {
                _image = GetComponentsInChildren
                        <Image>()
                    .FirstOrDefault(image => image.gameObject.CompareTag("AttentionBar"));
            }

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (StateMachine.CurrentState.Name)
            {
                case "CheckSittingStatus":
                {
                    break;
                }
                case "DecreaseAttention":
                {
                    Blackboard.Parameters["Attention"] =
                        Mathf.Clamp((float) Blackboard.Parameters["Attention"] - 0.01f, 0f, 1f);
                    break;
                }
                case "Green":
                {
                    if (_image != null) _image.color = Color.green;
                    npcTalkControllerBlackboard.Parameters["IsRed"] = "false";
                    _currentStateSubject.OnNext(StateMachine.CurrentState.Name);
                    break;
                }
                case "IncreaseAttention":
                {
                    Blackboard.Parameters["Attention"] =
                        Mathf.Clamp((float) Blackboard.Parameters["Attention"] + 0.1f, 0f, 1f);
                    break;
                }
                case "Red":
                {
                    if (_image != null) _image.color = Color.red;
                    npcTalkControllerBlackboard.Parameters["IsRed"] = "true";
                    _currentStateSubject.OnNext(StateMachine.CurrentState.Name);
                    break;
                }
                case "SittingOnTheChair":
                {
                    break;
                }
                case "UpdateIndicator":
                {
                    var attention = (float) Blackboard.Parameters["Attention"];
                    if (_image != null) _image.fillAmount = attention;
                    break;
                }
                case "Yellow":
                {
                    if (_image != null) _image.color = Color.yellow;
                    npcTalkControllerBlackboard.Parameters["IsRed"] = "false";
                    _currentStateSubject.OnNext(StateMachine.CurrentState.Name);
                    break;
                }
            }
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

        private void OnDestroy()
        {
            if (_currentStateSubjectDisposable != null) _currentStateSubjectDisposable.Dispose();
        }
    }
}
