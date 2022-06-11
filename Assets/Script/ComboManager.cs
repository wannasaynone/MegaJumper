using Zenject;

namespace MegaJumper
{
    public class ComboManager 
    {
        public int Combo { get; private set; }

        private readonly SignalBus m_signalBus;
        private readonly ScoreManager m_scoreManager;

        private JumperSetting m_currentSetting;
        private int m_feverCombo;
        private bool m_isFever;
        private bool m_tutorialMode;

        public ComboManager(SignalBus signalBus, ScoreManager scoreManager)
        {
            signalBus.Subscribe<Event.InGameEvent.OnJumpEnded>(OnJumpEnded);
            signalBus.Subscribe<Event.InGameEvent.OnJumperSettingSet>(OnJumperSettingSet);
            signalBus.Subscribe<Event.InGameEvent.OnFeverEnded>(OnFeverEnded);
            signalBus.Subscribe<Event.InGameEvent.OnStartFever>(OnFeverStart);
            signalBus.Subscribe<Event.InGameEvent.OnTutorialStart>(OnTutorialStart);
            signalBus.Subscribe<Event.InGameEvent.OnTutorialEnded>(OnTutorialEnded);
            signalBus.Subscribe<Event.InGameEvent.OnGameResetCalled>(OnGameResetCalled);

            m_signalBus = signalBus;
            m_scoreManager = scoreManager;
        }

        private void OnJumperSettingSet(Event.InGameEvent.OnJumperSettingSet obj)
        {
            m_currentSetting = obj.JumperSetting;
        }

        private void OnJumpEnded(Event.InGameEvent.OnJumpEnded obj)
        {
            if (m_isFever)
                return;

            if (obj.IsSuccess)
            {
                m_scoreManager.Add(1);
            }

            if (obj.IsPerfect || m_currentSetting.SkipPerfectCheck)
            {
                if (!m_currentSetting.SkipPerfectCheck)
                {
                    Combo++;
                }

                m_feverCombo++;
                m_signalBus.Fire(new Event.InGameEvent.OnComboAdded(Combo, m_feverCombo));
                if (m_currentSetting != null && m_feverCombo >= m_currentSetting.FeverRequireCombo)
                {
                    m_signalBus.Fire<Event.InGameEvent.OnStartFever>();
                }
            }
            else
            {
                m_feverCombo = 0;
                Combo = 0;
                m_signalBus.Fire(new Event.InGameEvent.OnComboReset());
            }
        }

        private void OnFeverStart()
        {
            m_feverCombo = 0;
            m_isFever = true;
        }

        private void OnFeverEnded()
        {
            m_isFever = false;
            if (!m_tutorialMode) m_scoreManager.Add(m_currentSetting.FeverAddScore * 2);
        }

        private void OnTutorialStart()
        {
            m_tutorialMode = true;
        }

        private void OnTutorialEnded()
        {
            m_tutorialMode = false;
        }

        private void OnGameResetCalled()
        {
            m_feverCombo = 0;
            Combo = 0;
            m_signalBus.Fire(new Event.InGameEvent.OnComboReset());
        }
    }
}