using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Controllers
{
    public class HeadColliderController : MonoBehaviour
    {
        private Transform _npcHead;
        public GameObject Npc;

        private void Update()
        {
            if (Npc == null) return;
            if (_npcHead == null)
                _npcHead = Npc.transform.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.CompareTag("Head"));
            else
                transform.position = _npcHead.position;
        }

        [UsedImplicitly]
        public class Factory : PlaceholderFactory<HeadColliderController>
        {
        }
    }
}