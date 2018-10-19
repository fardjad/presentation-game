using Controllers;
using UniRx.Triggers;
using UnityEngine;
using Utils.Input;
using Utils.Network;
using Zenject;

namespace Utils.DI
{
    public class GameInstaller : MonoInstaller<GameInstaller>
    {
        public GameObject ChairPrefab;
        public ChairSpawnController.Settings ChairSpawnControllerSettings;
        public GameObject HeadColliderHolderPrefab;
        public GameObject NpcPrefab;

        public override void InstallBindings()
        {
            Container.Bind<ISocketConfig>().To<SocketConfig>().AsCached();
            Container.BindFactory<ZmqIpcHelper, ZmqIpcHelper.Factory>();
            Container.BindInterfacesAndSelfTo<ZmqIpcManager>().AsCached().NonLazy();

            Container.Bind<UpdateInputObservableHelper>()
                .ToSelf()
                .AsCached()
                .WithArguments(this.UpdateAsObservable());

            Container.Bind<FixedUpdateInputObservableHelper>()
                .ToSelf()
                .AsCached()
                .WithArguments(this.FixedUpdateAsObservable());

            Container.Bind<LateUpdateInputObservableHelper>()
                .ToSelf()
                .AsCached()
                .WithArguments(this.LateUpdateAsObservable());

            Container.BindFactory<ChairController, ChairController.Factory>()
                .FromComponentInNewPrefab(ChairPrefab)
                .UnderTransformGroup("Chairs");

            Container.BindFactory<NpcController, NpcController.Factory>()
                .FromComponentInNewPrefab(NpcPrefab)
                .UnderTransformGroup("NPCs");

            Container.BindFactory<HeadColliderController, HeadColliderController.Factory>()
                .FromComponentInNewPrefab(HeadColliderHolderPrefab)
                .UnderTransformGroup("NPCs");

            Container.BindInstance(ChairSpawnControllerSettings).AsCached();
            Container.Bind<ChairManager>().ToSelf().AsCached();
            Container.BindInterfacesAndSelfTo<NpcManager>().AsCached();

            Container.BindInterfacesAndSelfTo<TalkManager>().AsCached();

            Container.Bind<ScoreManager>().ToSelf().AsSingle();

            Container.Bind<CommunicationManager>().ToSelf().AsSingle().NonLazy();
        }
    }
}