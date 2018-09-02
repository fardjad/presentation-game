using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Utils.StateMachine;

namespace Controllers
{
    public class CameraFacingNpcCanvasController : MonoBehaviour
    {
        private Camera _mainCamera;
        private Transform _npcHead;
        private NpcController _npcController;
        private IStateMachine _npcControllerStateMachine;
        private Image _attentionBarBg;

        public void Start()
        {
            _mainCamera = FindObjectOfType<Camera>();
            GetComponent<Canvas>().worldCamera = _mainCamera;

            _npcHead = transform.parent.GetComponentsInChildren<Transform>()
                .FirstOrDefault(t => t.CompareTag("Head"));

            _npcController = transform.parent.GetComponent<NpcController>();
            _npcControllerStateMachine = _npcController.StateMachine;

            _attentionBarBg = GetComponentInChildren<Image>();

            _npcControllerStateMachine.OnStateChanged += (sender, stateArgs) =>
            {
                var state = stateArgs.State;
                if (state.Name.Equals("SittingOnTheChair"))
                {
                    _attentionBarBg.gameObject.SetActive(true);
                }
                else if (state.Name.Equals("StandUp"))
                {
                    _attentionBarBg.gameObject.SetActive(false);
                }
            };

            _attentionBarBg.gameObject.SetActive(false);
        }

        private void Update()
        {
            transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward,
                _mainCamera.transform.rotation * Vector3.up);

            if (_npcHead == null) return;
            var npcHeadPosition = _npcHead.position;

            transform.position = VectorBuilder.FromVector(transform.position).SetY(npcHeadPosition.y + 0.5f).ToVector();
        }
    }
}
