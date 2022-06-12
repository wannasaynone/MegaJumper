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
        [SerializeField] private TextMeshProUGUI m_lifeText;
        [SerializeField] private GameObject m_coinPanelRoot;
        [SerializeField] private TextMeshProUGUI m_coinText;

        private JumperSetting m_currentJumpSetting;

        private bool m_tutorialMode = false;
        private LocalSaveManager m_localSaveManager;

        [Inject]
        public void Constructor(SignalBus signalBus, LocalSaveManager localSaveManager)
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
            signalBus.Subscribe<Event.InGameEvent.OnJumpEnded>(OnJumpEnded);
            signalBus.Subscribe<Event.InGameEvent.OnCoinAdded>(OnCoinAdded);
            m_scoreText.text = "";
            m_comboText.text = "";

            m_localSaveManager = localSaveManager;
        }

        private void OnTutorialStart()
        {
            m_tutorialMode = true;
            m_scoreText.gameObject.SetActive(false);
            m_coinPanelRoot.SetActive(false);
        }

        private void OnTutorialEnded()
        {
            m_tutorialMode = false;
            m_scoreText.gameObject.SetActive(true);
            m_scoreText.text = "0";
        }

        private void OnGameStarted()
        {
            if (m_tutorialMode)
                return;

            m_scoreText.gameObject.SetActive(true);
            m_coinPanelRoot.SetActive(false);
        }

        private void OnJumperSettingSet(Event.InGameEvent.OnJumperSettingSet obj)
        {
            m_currentJumpSetting = obj.JumperSetting;
            OnJumpEnded(new Event.InGameEvent.OnJumpEnded(Vector3.zero, false, false, m_currentJumpSetting.Life));
        }

        private void OnScoreReset()
        {
            m_scoreText.text = "";
            m_coinPanelRoot.SetActive(m_localSaveManager.SaveDataInstance.IsTutorialEnded);
            OnJumpEnded(new Event.InGameEvent.OnJumpEnded(Vector3.zero, false, false, m_currentJumpSetting.Life));
        }

        private void OnScoreAdded(Event.InGameEvent.OnScoreAdded obj)
        {
            m_scoreText.text = obj.Current.ToString();
        }

        private void OnComboAdded(Event.InGameEvent.OnComboAdded obj)
        {
            if (obj.Current > 0)
                m_comboText.text = "Combo x" + obj.Current;
            else
                m_comboText.text = "";

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

        private void OnJumpEnded(Event.InGameEvent.OnJumpEnded obj)
        {
            if (m_currentJumpSetting.Life >= 2)
            {
                m_lifeText.text = "Life: " + obj.RemainingLife;
            }
            else
            {
                m_lifeText.text = "";
            }
        }

        private void OnCoinAdded(Event.InGameEvent.OnCoinAdded obj)
        {
            m_coinText.text = obj.Current.ToString("N0");
        }
    }
}