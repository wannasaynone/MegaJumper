using UnityEngine;
using Zenject;

namespace MegaJumper.UI
{
    public class ProgressView : MonoBehaviour
    {
        [SerializeField] private GameObject m_root;
        [SerializeField] private TMPro.TextMeshProUGUI m_hintText;
        [SerializeField] private string m_hint;
        [SerializeField] private string m_completeHint;
        [SerializeField] private TMPro.TextMeshProUGUI m_progressText;
        [SerializeField] private int m_target = 0;
        [SerializeField] private UnityEngine.UI.Image m_progressImage;
        [SerializeField] private GameObject m_nextButton;

        private SignalBus m_signalBus;
        private LocalSaveManager m_localSaveManager;
        private SelectJumperView m_selectJumpView;

        [Inject]
        public void Constructor(SignalBus signalBus, LocalSaveManager localSaveManager, SelectJumperView selectJumperView)
        {
            m_signalBus = signalBus;
            m_localSaveManager = localSaveManager;
            m_selectJumpView = selectJumperView;
            m_signalBus.Subscribe<Event.InGameEvent.OnScoreReset>(OnScoreReset);
        }

        private void OnScoreReset()
        {
            if (m_localSaveManager.SaveDataInstance.Coin >= m_target)
            {
                m_hintText.text = m_completeHint;
            }
            else
            {
                m_hintText.text = string.Format(m_hint, (m_target - m_localSaveManager.SaveDataInstance.Coin).ToString("N0"));
            }
            m_progressText.text = m_localSaveManager.SaveDataInstance.Coin.ToString("N0") + " / " + m_target.ToString("N0");
            m_progressImage.fillAmount = (float)m_localSaveManager.SaveDataInstance.Coin / (float)m_target;
        }

        public void Show(int currentCoin)
        {
            m_root.SetActive(true);
            StartCoroutine(IEShowProgress(currentCoin));
        }

        private System.Collections.IEnumerator IEShowProgress(int currentCoin)
        {
            if (m_localSaveManager.SaveDataInstance.Coin >= m_target)
            {
                m_hintText.text = m_completeHint;
            }
            else
            {
                m_hintText.text = string.Format(m_hint, (m_target - m_localSaveManager.SaveDataInstance.Coin).ToString("N0"));
            }
            yield return new WaitForSeconds(0.5f);

            float _targetFillAmount = (float)currentCoin / (float)m_target;

            KahaGameCore.Common.GameUtility.RunNunber(m_progressImage.fillAmount, _targetFillAmount, 1f, OnProgressUpdate, delegate { OnProgessEnded(currentCoin); });

            yield return new WaitForSeconds(1f);

            m_nextButton.SetActive(true);
        }

        private void OnProgressUpdate(float current)
        {
            float _cur = (float)m_target * current;
            int _curInt = System.Convert.ToInt32(_cur);
            m_progressImage.fillAmount = current;
            m_progressText.text = _curInt.ToString("N0") + " / " + m_target.ToString("N0");
        }

        private void OnProgessEnded(int currentCoin)
        {
            m_progressText.text = currentCoin.ToString("N0") + " / " + m_target.ToString("N0");
        }

        public void Button_Next()
        {
            m_root.SetActive(false);
            m_nextButton.SetActive(false);
            if (m_progressImage.fillAmount >= 1f)
            {
                m_selectJumpView.ShowChangeButtonUnlock();
            }
            else
            {
                m_signalBus.Fire(new Event.InGameEvent.OnGameResetCalled(false));
            }
        }
    }
}