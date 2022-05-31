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

        public ComboManager(SignalBus signalBus, ScoreManager scoreManager)
        {
            signalBus.Subscribe<Event.InGameEvent.OnJumpEnded>(OnJumpEnded);
            signalBus.Subscribe<Event.InGameEvent.OnJumperSettingSet>(OnJumperSettingSet);
            signalBus.Subscribe<Event.InGameEvent.OnFeverEnded>(OnFeverEnded);
            signalBus.Subscribe<Event.InGameEvent.OnStartFever>(OnFeverStart);

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

            if (obj.IsPerfect)
            {
                Combo++;

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
            m_scoreManager.Add(m_currentSetting.FeverAddScore * 2);
        }
    }
}