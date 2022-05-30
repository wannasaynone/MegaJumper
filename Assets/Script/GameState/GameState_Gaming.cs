using Zenject;

namespace MegaJumper.GameState
{
    public class GameState_Gaming : GameStateBase
    {
        private readonly BlockManager m_blockManager;
        private readonly GameProperties m_gameProperties;

        private JumperSetting m_jumperSetting;
        private bool m_isFever = false;

        private bool m_isApplyFirstFever = true;

        public GameState_Gaming(
            BlockManager blockManager,
            GameProperties gameProperties,
            SignalBus signalBus, 
            JumperSetting jumperSetting,
            bool applyFirstFever) : base(signalBus)
        {
            m_gameProperties = gameProperties;
            m_blockManager = blockManager;
            m_jumperSetting = jumperSetting;
            m_isApplyFirstFever = !applyFirstFever;
        }

        private void OnJumpEnded(Event.InGameEvent.OnJumpEnded obj)
        {
            if (m_isFever)
            {
                return;
            }

            if (obj.IsSuccess)
            {
                if (m_isApplyFirstFever)
                {
                    m_blockManager.CreateNew();
                }
                else
                {
                    m_isApplyFirstFever = true;
                    SignalBus.Fire<Event.InGameEvent.OnStartFever>();
                }
            }
        }

        public override void Start()
        {
            SignalBus.Subscribe<Event.InGameEvent.OnJumpEnded>(OnJumpEnded);
            SignalBus.Subscribe<Event.InGameEvent.OnStartFever>(OnStartFever);
            SignalBus.Subscribe<Event.InGameEvent.OnFeverEnded>(OnFeverEnded);
            SignalBus.Subscribe<Event.InGameEvent.OnJumperSettingSet>(OnJumperSettingSet);
        }

        private void OnStartFever()
        {
            m_isFever = true;
            float _eachTime = m_gameProperties.FEVER_ANIMATION_TIME / (float)m_jumperSetting.FeverAddScore;
            for (int i = 0; i < m_jumperSetting.FeverAddScore; i++)
            {
                KahaGameCore.Common.TimerManager.Schedule(_eachTime * i, m_blockManager.CreateNew);
            }
        }

        private void OnFeverEnded()
        {
            m_isFever = false;
            m_blockManager.CreateNew();
        }

        private void OnJumperSettingSet(Event.InGameEvent.OnJumperSettingSet obj)
        {
            m_jumperSetting = obj.JumperSetting;
        }

        public override void Tick()
        {
        }

        public override void Stop()
        {
            SignalBus.Unsubscribe<Event.InGameEvent.OnJumpEnded>(OnJumpEnded);
            SignalBus.Unsubscribe<Event.InGameEvent.OnStartFever>(OnStartFever);
            SignalBus.Unsubscribe<Event.InGameEvent.OnFeverEnded>(OnFeverEnded);
            SignalBus.Unsubscribe<Event.InGameEvent.OnJumperSettingSet>(OnJumperSettingSet);
        }
    }
}