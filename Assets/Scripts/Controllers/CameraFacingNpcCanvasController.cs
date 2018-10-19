using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Controllers
{
    public class CameraFacingNpcCanvasController : MonoBehaviour
    {
        private Image _attentionBarBg;
        private Camera _mainCamera;
        private NpcAttentionController _npcAttentionController;
        private Transform _npcHead;

        public void Start()
        {
            _mainCamera = FindObjectOfType<Camera>();
            GetComponent<Canvas>().worldCamera = _mainCamera;

            _npcHead = transform.parent.GetComponentsInChildren<Transform>()
                .FirstOrDefault(t => t.CompareTag("Head"));

            _npcAttentionController = transform.parent.GetComponent<NpcAttentionController>();

            _attentionBarBg = GetComponentInChildren<Image>();


            _attentionBarBg.gameObject.SetActive(false);
        }

        private void Update()
        {
            if ((string) _npcAttentionController.Blackboard.Parameters["QATime"] == "true" ||
                (string) _npcAttentionController.Blackboard.Parameters["SittingOnTheChair"] == "false")
            {
                _attentionBarBg.gameObject.SetActive(false);
                return;
            }

            _attentionBarBg.gameObject.SetActive(true);

            transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward,
                _mainCamera.transform.rotation * Vector3.up);

            if (_npcHead == null) return;
            var npcHeadPosition = _npcHead.position;

            transform.position = VectorBuilder.FromVector(transform.position).SetY(npcHeadPosition.y + 0.5f).ToVector();
        }
    }
}