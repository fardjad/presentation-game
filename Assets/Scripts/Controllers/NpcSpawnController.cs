using System;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using Utils;
using Zenject;

namespace Controllers
{
    public class NpcSpawnController : MonoBehaviour
    {
        private NpcController.Factory _factory;
        private INpcManager _npcManager;

        [Inject]
        [UsedImplicitly]
        public void Construct(NpcController.Factory factory, INpcManager npcManager)
        {
            _factory = factory;
            _npcManager = npcManager;
        }

        // Use this for initialization
        private void Start()
        {
            Observable.Interval(TimeSpan.FromSeconds(1))
                .Delay(TimeSpan.FromSeconds(1))
                .Take(11)
                .Subscribe(_ =>
                {
                    var npcController = _factory.Create();
                    npcController.gameObject.transform.position = transform.position;

                    var id = Guid.NewGuid().ToString();
                    _npcManager.RegisterNpcController(id, npcController);
                });
        }
    }
}
