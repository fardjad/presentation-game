using System.Linq;
using UniRx;
using UnityEngine;

namespace Controllers
{
    public class CameraRaycastController : MonoBehaviour
    {
        private Camera _camera;
        private NpcAttentionController _currentNpcAttentionController;
        private Subject<string> _logger;

        private void Start()
        {
            _camera = FindObjectsOfType<Camera>().FirstOrDefault(c => c.CompareTag("MainCamera"));
            _logger = new Subject<string>();
            _logger.DistinctUntilChanged().Subscribe(Debug.Log);
        }

        private void FixedUpdate()
        {
            var ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Debug.DrawRay(ray.origin, ray.direction, Color.yellow, Time.deltaTime, true);
            RaycastHit hit;
            if (Physics.Raycast(ray.origin, ray.direction, out hit, 500f, 1 << 10, QueryTriggerInteraction.Collide))
            {
                var headColliderController = hit.transform.GetComponent<HeadColliderController>();
                if (headColliderController == null) return;
                var npcController = headColliderController.Npc;
                var npcAttentionController = npcController.GetComponent<NpcAttentionController>();
                if (_currentNpcAttentionController != null && _currentNpcAttentionController != npcAttentionController)
                    _currentNpcAttentionController.Blackboard.Parameters["PlayerIsLookingAtNpc"] = "false";

                _currentNpcAttentionController = npcAttentionController;
                npcAttentionController.Blackboard.Parameters["PlayerIsLookingAtNpc"] = "true";
            }
            else
            {
                if (_currentNpcAttentionController != null)
                    _currentNpcAttentionController.Blackboard.Parameters["PlayerIsLookingAtNpc"] = "false";

                _currentNpcAttentionController = null;
            }
        }
    }
}