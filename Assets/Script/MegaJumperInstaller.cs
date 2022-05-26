using Zenject;

namespace MegaJumper
{
    public class MegaJumperInstaller : MonoInstaller
    {
        [Inject] private BlockContainer m_blockContainer;

        public override void InstallBindings()
        {
            Container.Bind(typeof(ITickable)).To<InputDetecter>().AsSingle();
            Container.Bind<ScoreManager>().AsSingle();
            Container.Bind<BlockManager>().AsSingle();
            Container.BindFactory<Block, Block.Factory>().FromComponentInNewPrefab(m_blockContainer.blockPrefab);
            Container.Bind(typeof(IInitializable), typeof(ITickable)).To<GameManager>().AsSingle();

            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<Event.InGameEvent.OnBlockSpawned>();
            Container.DeclareSignal<Event.InGameEvent.OnFeverEnded>();
            Container.DeclareSignal<Event.InGameEvent.OnJumpEnded>();
            Container.DeclareSignal<Event.InGameEvent.OnPointDown>();
            Container.DeclareSignal<Event.InGameEvent.OnPointUp>();
            Container.DeclareSignal<Event.InGameEvent.OnStartFever>();
            Container.DeclareSignal<Event.InGameEvent.OnStartJump>();
            Container.DeclareSignal<Event.InGameEvent.OnGameStarted>();
        }
    }
}
