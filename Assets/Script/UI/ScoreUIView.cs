using UnityEngine;
using Zenject;
using TMPro;

namespace MegaJumper.UI
{
    public class ScoreUIView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_scoreText;
        [SerializeField] private TextMeshProUGUI m_comboText;
        [SerializeField] private MoreMountains.Tools.MMProgressBar m_comboProgressBar;
        [SerializeField] private GameObject m_progressBarRoot;
        [SerializeField] private GameObject m_feverJumpText;

        private int m_currentHighest;
        private JumperSetting m_currentJumpSetting;

        private bool m_tutorialMode = false;

        [Inject]
        public void Constructor(SignalBus signalBus, GameProperties gameProperties)
        {
            signalBus.Subscribe<Event.InGameEvent.OnScoreAdded>(OnScoreAdded);
            signalBus.Subscribe<Event.InGameEvent.OnScoreReset>(OnScoreReset);
            signalBus.Subscribe<Event.InGameEvent.OnComboAdded>(OnComboAdded);
            signalBus.Subscribe<Event.InGameEvent.OnComboReset>(OnComboReset);
            signalBus.Subscribe<Event.InGameEvent.OnJumperSettingSet>(OnJumperSettingSet);
            signalBus.Subscribe<Event.InGameEvent.OnFeverEnded>(OnFeverEnded);
            signalBus.Subscribe<Event.InGameEvent.OnTutorialStart>(OnTutorialStart);
            signalBus.Subscribe<Event.InGameEvent.OnTutorialEnded>(OnTutorialEnded);
            signalBus.Subscribe<Event.InGameEvent.OnGameStarted>(OnGameStarted);
            m_scoreText.text = "0";
            m_comboText.text = "";
        }

        private void OnTutorialStart()
        {
            m_tutorialMode = true;
            m_scoreText.gameObject.SetActive(false);
        }

        private void OnTutorialEnded()
        {
            m_currentHighest = 0;
            m_tutorialMode = false;
            m_scoreText.gameObject.SetActive(true);
        }

        private void OnGameStarted()
        {
            if (m_tutorialMode)
                return;

            m_scoreText.gameObject.SetActive(true);
        }

        private void OnJumperSettingSet(Event.InGameEvent.OnJumperSettingSet obj)
        {
            m_currentJumpSetting = obj.JumperSetting;
        }

        private void OnScoreReset()
        {
            m_scoreText.text = "0";
        }

        private void OnScoreAdded(Event.InGameEvent.OnScoreAdded obj)
        {
            if (obj.Current >= m_currentHighest)
            {
                m_scoreText.text = obj.Current.ToString();
                m_currentHighest = obj.Current;
            }
            else
            {
                m_scoreText.text = obj.Current.ToString() + " / " + m_currentHighest;
            }
        }

        private void OnComboAdded(Event.InGameEvent.OnComboAdded obj)
        {
            m_comboText.text = "Combo x" + obj.Current;

            if (m_progressBarRoot.activeSelf)
            {
                m_comboProgressBar.UpdateBar01((float)obj.FeverCombo / (float)m_currentJumpSetting.FeverRequireCombo);
            }
            else
            {
                m_progressBarRoot.SetActive(true);
                m_comboProgressBar.SetBar01((float)obj.FeverCombo / (float)m_currentJumpSetting.FeverRequireCombo);
            }

            m_feverJumpText.SetActive(obj.FeverCombo >= m_currentJumpSetting.FeverRequireCombo);
        }

        private void OnComboReset()
        {
            m_comboText.text = "";
            m_comboProgressBar.SetBar01(0f);
            m_progressBarRoot.SetActive(false);
        }

        private void OnFeverEnded()
        {
            m_comboProgressBar.SetBar01(0f);
            m_progressBarRoot.SetActive(false);
            m_feverJumpText.SetActive(false);
        }
    }
}