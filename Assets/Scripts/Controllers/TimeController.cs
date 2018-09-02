using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using JetBrains.Annotations;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace Controllers
{
    public class TimeController : MonoBehaviour
    {
        private TimeSpan _remainingTime;
        private TextMeshProUGUI _textMeshPro;
        private NpcManager _npcManager;
        private IDisposable _timerDisposable;
        private bool _timerStarted = false;
        private ChairManager _chairManager;
        private SlideController _slideController;
        private IDictionary<int, TimeSpan> _slideTime;
        private float _attentionSum = 0f;
        private TimeSpan _elapsedTime;

        [SerializeField] public RawImage SlideImage;
        private ScoreManager _scoreManager;

        [UsedImplicitly]
        [Inject]
        private void Construct(NpcManager npcManager, ChairManager chairManager, ScoreManager scoreManager)
        {
            _scoreManager = scoreManager;
            _chairManager = chairManager;
            _npcManager = npcManager;
        }

        private void Start()
        {
            _textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
            _slideController = SlideImage.GetComponent<SlideController>();

            _remainingTime = TimeSpan.FromMinutes(20);
            _elapsedTime = TimeSpan.Zero;

            _scoreManager.Finished = false;
            _slideTime = _scoreManager.SlideTime;
            _slideTime.Clear();
            for (var i = 0; i < _slideController.SlidesCount; i += 1)
            {
                _slideTime[i] = TimeSpan.Zero;
            }
        }

        private void StartTimer()
        {
            _timerStarted = true;
            _timerDisposable = this.UpdateAsObservable()
                .Sample(TimeSpan.FromSeconds(1))
                .TimeInterval()
                .Subscribe(ti =>
                {
                    if (_remainingTime <= TimeSpan.Zero)
                    {
                        _scoreManager.Finished = true;
                        return;
                    };

                    var currentSlide = _slideController.CurrentSlide;
                    _slideTime[currentSlide] += ti.Interval;
                    _remainingTime -= ti.Interval;
                    _elapsedTime += ti.Interval;

                    _npcManager.NpcControllerDictionary.Values.ToList()
                        .Select(npcController => (float) npcController.gameObject.GetComponent<NpcAttentionController>()
                            .Blackboard.Parameters["Attention"])
                        .ToList()
                        .ForEach(
                            attention => { _attentionSum += attention; });

                    _scoreManager.AverageAttention = _attentionSum /
                                                     _npcManager.NpcControllerDictionary.Count /
                                                     _elapsedTime.TotalSeconds;
                });
        }

        private void StopTimer()
        {
            if (!_timerStarted) return;
            _timerStarted = false;
            _timerDisposable.Dispose();
        }

        private void Update()
        {
            var text = string.Format("{0:00}:{1:00}", _remainingTime.Minutes, _remainingTime.Seconds);
            _textMeshPro.text = text;

            if (_npcManager.NpcControllerDictionary.Count != _chairManager.GetChairsCount() || _timerStarted) return;

            var allSeated = true;

            _npcManager.NpcControllerDictionary.Values.ToList()
                .ForEach(controller =>
                {
                    var stateMachine = controller.StateMachine;
                    if (!stateMachine.CurrentState.Name.Equals("SittingOnTheChair"))
                    {
                        allSeated = false;
                    }
                });

            if (allSeated)
            {
                StartTimer();
            }
        }

        private void OnDestroy()
        {
            StopTimer();
        }
    }
}
