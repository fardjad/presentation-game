using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using Utils;
using Zenject;

namespace Controllers
{
    public class NpcSpawnController : MonoBehaviour
    {
        private NpcController.Factory _npcControllerFactory;
        private NpcManager _npcManager;
        private HeadColliderController.Factory _headColliderControllerFactory;

        private List<IDisposable> _disposables;
        private ChairManager _chairManager;

        [Inject]
        [UsedImplicitly]
        public void Construct(NpcController.Factory npcControllerFactory,
            HeadColliderController.Factory headColliderControllerFactory,
            NpcManager npcManager, ChairManager chairManager)
        {
            _chairManager = chairManager;
            _npcControllerFactory = npcControllerFactory;
            _headColliderControllerFactory = headColliderControllerFactory;
            _npcManager = npcManager;
        }

        // Use this for initialization
        private void Start()
        {
            _disposables = new List<IDisposable>();

            var intervalDisposable = Observable.Interval(TimeSpan.FromSeconds(1))
                .Delay(TimeSpan.FromSeconds(1))
                .Take(_chairManager.GetChairsCount())
                .Subscribe(_ =>
                {
                    var npcController = _npcControllerFactory.Create();
                    var headColliderController = _headColliderControllerFactory.Create();
                    headColliderController.Npc = npcController.gameObject;

                    npcController.gameObject.transform.position = transform.position;

                    var id = Guid.NewGuid().ToString();

                    npcController.name = string.Format("NPC {0}", id);
                    headColliderController.name = string.Format("HeadColliderHolder {0}", id);

                    _npcManager.RegisterNpcController(id, npcController);
                });

            _disposables.Add(intervalDisposable);
        }

        private void OnDestroy()
        {
            _disposables.ToList().ForEach(d => d.Dispose());
        }
    }
}
