using UnityEngine;
using Zenject;
using TMPro;
using DG.Tweening;

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
        [SerializeField] private TextMeshProUGUI m_historyHighscoreText;
        [SerializeField] private TextMeshProUGUI m_todayHighscoreText;
        [SerializeField] private GameObject m_restoreButton;
        [SerializeField] private float m_startX;
        [SerializeField] private float m_endX;

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
            m_scoreText.text = "";
            m_comboText.text = "";

            m_localSaveManager = localSaveManager;
        }

        private void OnTutorialStart()
        {
            m_tutorialMode = true;
            m_scoreText.gameObject.SetActive(false);
            m_coinPanelRoot.SetActive(false);
            m_historyHighscoreText.gameObject.SetActive(false);
            m_todayHighscoreText.gameObject.SetActive(false);
            m_restoreButton.SetActive(false);
        }

        private void OnTutorialEnded()
        {
            m_tutorialMode = false;
            m_scoreText.gameObject.SetActive(true);
            m_scoreText.text = "0";
            m_restoreButton.SetActive(false);
        }

        private void OnGameStarted()
        {
            if (m_tutorialMode)
                return;

            m_scoreText.gameObject.SetActive(true);
            m_coinPanelRoot.SetActive(false);
            m_historyHighscoreText.gameObject.SetActive(false);
            m_todayHighscoreText.gameObject.SetActive(false);
            m_restoreButton.SetActive(false);
        }

        private void OnJumperSettingSet(Event.InGameEvent.OnJumperSettingSet obj)
        {
            m_currentJumpSetting = obj.JumperSetting;
            OnJumpEnded(new Event.InGameEvent.OnJumpEnded(Vector3.zero, false, false, m_currentJumpSetting.Life));
        }

        private void OnScoreReset()
        {
            m_scoreText.text = "";
            m_coinText.text = m_localSaveManager.SaveDataInstance.Coin.ToString("N0");
            m_coinPanelRoot.SetActive(m_localSaveManager.SaveDataInstance.IsTutorialEnded);
            m_historyHighscoreText.gameObject.SetActive(m_localSaveManager.SaveDataInstance.IsTutorialEnded);
            m_todayHighscoreText.gameObject.SetActive(m_localSaveManager.SaveDataInstance.IsTutorialEnded);
            m_historyHighscoreText.text = "History Highscore\n" + m_localSaveManager.SaveDataInstance.Highscore_All;
            m_todayHighscoreText.text = "Today Highscore\n" + m_localSaveManager.SaveDataInstance.Highscore_Day;
            m_restoreButton.SetActive(true);
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

        public void ShowCoinPanel(bool withBounce = false)
        {
            m_coinPanelRoot.SetActive(true);
            if (withBounce)
            {
                KahaGameCore.Common.GeneralCoroutineRunner.Instance.StartCoroutine(IEBounceCoinPanel());   
            }
        }

        public void HideCoinPanel(bool withBounce = false)
        {
            if (withBounce)
            {
                KahaGameCore.Common.GeneralCoroutineRunner.Instance.StartCoroutine(IEBounceCoinPanel_Hide());
            }
            else
            {
                m_coinPanelRoot.SetActive(false);
            }
        }

        private System.Collections.IEnumerator IEBounceCoinPanel()
        {
            m_coinPanelRoot.transform.localScale = Vector3.zero;
            m_coinPanelRoot.transform.DOScale(new Vector3(1.1f, 1.1f, 1f), 0.15f);
            yield return new WaitForSeconds(0.15f);
            m_coinPanelRoot.transform.DOScale(Vector3.one, 0.3f);
        }

        private System.Collections.IEnumerator IEBounceCoinPanel_Hide()
        {
            m_coinPanelRoot.transform.DOScale(new Vector3(1.1f, 1.1f, 1f), 0.15f);
            yield return new WaitForSeconds(0.15f);
            m_coinPanelRoot.transform.DOScale(Vector3.zero, 0.3f);
            yield return new WaitForSeconds(0.15f);
            m_coinPanelRoot.SetActive(false);
        }

        public void UpdateCoinText(int value)
        {
            m_coinText.text = value.ToString("N0");
        }

        private bool m_shaking;
        public void ShakeCoinPanel()
        {
            if (m_shaking)
                return;

            KahaGameCore.Common.GeneralCoroutineRunner.Instance.StartCoroutine(IEShakeCoinPanel());
        }

        private System.Collections.IEnumerator IEShakeCoinPanel()
        {
            m_shaking = true;
            m_coinPanelRoot.transform.DOScale(Vector3.one * 1.5f, 0.5f);
            m_coinPanelRoot.transform.DOShakePosition(0.5f, 100f, 20);
            yield return new WaitForSeconds(0.5f);
            m_coinPanelRoot.transform.DOScale(Vector3.one, 0.25f);
            yield return new WaitForSeconds(0.3f);
            m_shaking = false;
        }
    }
}