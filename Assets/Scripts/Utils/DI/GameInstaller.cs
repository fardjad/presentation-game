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
        public GameObject NpcPrefab;

        public override void InstallBindings()
        {
            Container.Bind<ISocketConfig>().To<SocketConfig>().AsSingle();
            Container.BindFactory<ZmqIpcHelper, ZmqIpcHelper.Factory>();
            Container.BindInterfacesAndSelfTo<ZmqIpcManager>().AsSingle().NonLazy();

            Container.Bind<UpdateInputObservableHelper>()
                .ToSelf()
                .AsSingle()
                .WithArguments(this.UpdateAsObservable());

            Container.Bind<FixedUpdateInputObservableHelper>()
                .ToSelf()
                .AsSingle()
                .WithArguments(this.FixedUpdateAsObservable());

            Container.Bind<LateUpdateInputObservableHelper>()
                .ToSelf()
                .AsSingle()
                .WithArguments(this.LateUpdateAsObservable());

            Container.BindFactory<ChairController, ChairController.Factory>()
                .FromComponentInNewPrefab(ChairPrefab)
                .UnderTransformGroup("Chairs");

            Container.BindFactory<NpcController, NpcController.Factory>()
                .FromComponentInNewPrefab(NpcPrefab)
                .UnderTransformGroup("NPCs");

            Container.BindInstance(ChairSpawnControllerSettings).AsSingle();
            Container.Bind<ChairManager>().ToSelf().AsSingle();

            Container.Bind<INpcManager>().To<NpcManager>().AsSingle();
        }
    }
}