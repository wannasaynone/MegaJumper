using Zenject;

namespace MegaJumper
{
    public class MegaJumperInstaller : MonoInstaller
    {
        [UnityEngine.SerializeField] private Monetization.AdmobKeyStorer m_admobStorer;

        [Inject] private BlockContainer m_blockContainer;

        public override void InstallBindings()
        {
            Container.Bind(typeof(ITickable)).To<InputDetecter>().AsSingle();
            Container.Bind<ScoreManager>().AsSingle();
            Container.Bind<BlockManager>().AsSingle();
            Container.BindFactory<Block, Block.Factory>().FromComponentInNewPrefab(m_blockContainer.blockPrefab);
            Container.Bind(typeof(IInitializable), typeof(ITickable)).To<GameManager>().AsSingle();
            Container.Bind<ComboManager>().AsSingle().NonLazy();
            Container.Bind<LocalSaveManager>().AsSingle().NonLazy();

            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<Event.InGameEvent.OnBlockSpawned>();
            Container.DeclareSignal<Event.InGameEvent.OnFeverEnded>();
            Container.DeclareSignal<Event.InGameEvent.OnJumpEnded>();
            Container.DeclareSignal<Event.InGameEvent.OnPointDown>();
            Container.DeclareSignal<Event.InGameEvent.OnPointUp>();
            Container.DeclareSignal<Event.InGameEvent.OnStartFever>();
            Container.DeclareSignal<Event.InGameEvent.OnStartJump>();
            Container.DeclareSignal<Event.InGameEvent.OnGameStarted>();
            Container.DeclareSignal<Event.InGameEvent.OnGameResetCalled>();
            Container.DeclareSignal<Event.InGameEvent.OnScoreAdded>();
            Container.DeclareSignal<Event.InGameEvent.OnScoreReset>();
            Container.DeclareSignal<Event.InGameEvent.OnJumperSettingSet>();
            Container.DeclareSignal<Event.InGameEvent.OnComboAdded>();
            Container.DeclareSignal<Event.InGameEvent.OnComboReset>();
            Container.DeclareSignal<Event.InGameEvent.OnTutorialStart>();
            Container.DeclareSignal<Event.InGameEvent.OnTutorialEnded>();

            InitSDK();
        }

        private void InitSDK()
        {
            GoogleMobileAds.Api.MobileAds.Initialize(OnAdInited);
        }

        private void OnAdInited(GoogleMobileAds.Api.InitializationStatus status)
        {
            STORIAMonetization.MonetizeCenter.Instance.AdManager.SetAdUnit(
                new Monetization.AdmobAdUnit(m_admobStorer.RewardAdID, m_admobStorer.InterstitialAdID));
        }

        private void Update()
        {
            UnityEngine.Application.targetFrameRate = 60;
        }
    }
}
