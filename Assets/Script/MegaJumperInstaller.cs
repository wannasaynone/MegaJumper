using Zenject;
using DG.Tweening;

namespace MegaJumper
{
    public class MegaJumperInstaller : MonoInstaller
    {
        [UnityEngine.SerializeField] private UnityEngine.AudioSource m_bgm;

        [Inject] private BlockContainer m_blockContainer;

        public override void InstallBindings()
        {
            Container.Bind<ScoreManager>().AsSingle();
            Container.Bind<BlockManager>().AsSingle();
            Container.BindFactory<Block, Block.Factory>().FromComponentInNewPrefab(m_blockContainer.blockPrefab).UnderTransform(transform.parent);
            Container.Bind(typeof(IInitializable), typeof(ITickable)).To<GameManager>().AsSingle();
            Container.Bind<ComboManager>().AsSingle().NonLazy();
            Container.Bind<LocalSaveManager>().AsSingle().NonLazy();
            Container.Bind<SettlemenManager>().AsSingle();

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
            Container.DeclareSignal<Event.InGameEvent.OnStartRevive>();

            DOTween.To(GetBGM, SetBGM, 1f, 1f);
        }

        private float GetBGM()
        {
            return m_bgm.volume;
        }

        private void SetBGM(float v)
        {
            m_bgm.volume = v;
        }

        private void Update()
        {
            UnityEngine.Application.targetFrameRate = 60;
        }
    }
}
