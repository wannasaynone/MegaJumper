using DG.Tweening;
using UnityEngine;
using Zenject;

namespace MegaJumper.UI
{
    public class GoldRewardView : MonoBehaviour
    {
        public event System.Action OnCoinGained;

        [SerializeField] private GameObject m_root;
        [SerializeField] private TMPro.TextMeshProUGUI m_acmountText;
        [SerializeField] private int m_rewardAmount;
        [SerializeField] private UnityEngine.UI.Image m_coinImage;
        [SerializeField] private RectTransform m_coinEndPos;
        [SerializeField] private GameObject[] m_buttonRoots;
        [SerializeField] private GameObject m_loadingPanel;

        private ScoreUIView m_scoreUI;
        private LocalSaveManager m_localSaveManager;

        [Inject]
        public void Constructor(ScoreUIView scoreUI, LocalSaveManager localSaveManager)
        {
            m_scoreUI = scoreUI;
            m_localSaveManager = localSaveManager;
        }

        public void Show()
        {
            m_acmountText.text = m_rewardAmount.ToString("N0");
            m_root.SetActive(true);
            for (int i = 0; i < m_buttonRoots.Length; i++)
            {
                m_buttonRoots[i].SetActive(true);
            }
        }

        public void Button_ShowAd()
        {
            m_loadingPanel.SetActive(true);
            GameAnalyticsSDK.GameAnalytics.NewAdEvent(GameAnalyticsSDK.GAAdAction.Show, GameAnalyticsSDK.GAAdType.RewardedVideo, "Admob", "Gold");
            STORIAMonetization.MonetizeCenter.Instance.AdManager.ShowRewardVideo(OnAdShown, OnAdShownFail);
        }

        private void OnAdShown()
        {
            ProjectBS.Network.UnityThread.Do(ProcessReward);
        }

        // for process unity API in main thread
        private void ProcessReward()
        {
            GameAnalyticsSDK.GameAnalytics.NewAdEvent(GameAnalyticsSDK.GAAdAction.RewardReceived, GameAnalyticsSDK.GAAdType.RewardedVideo, "Admob", "Gold");
            m_loadingPanel.SetActive(false);
            for (int i = 0; i < m_buttonRoots.Length; i++)
            {
                m_buttonRoots[i].SetActive(false);
            }

            int _orginMoneyNumber = m_localSaveManager.SaveDataInstance.Coin;
            int _playerFinalCoin = _orginMoneyNumber + m_rewardAmount;

            m_localSaveManager.SaveDataInstance.AddCoin(m_rewardAmount);
            m_localSaveManager.SaveAll();

            KahaGameCore.Common.GameUtility.RunNunber(m_rewardAmount, 0, 0.5f, OnRewardNumberUpdate, null);
            KahaGameCore.Common.GameUtility.RunNunber(_orginMoneyNumber, _playerFinalCoin, 0.5f, OnCoinNumberUpdate, OnShown);
            StartCoroutine(IEShowAddCoin());
            GameResultView.showAdChance = 0f;
            OnCoinGained?.Invoke();
        }

        private System.Collections.IEnumerator IEShowAddCoin()
        {
            float _flyTime = 0.2f;
            float _spawnGap = 0.02f;
            int _amount = System.Convert.ToInt32(0.4f / _spawnGap);
            for (int i = 0; i < _amount; i++)
            {
                CloneCoin(_flyTime);
                yield return new WaitForSeconds(_spawnGap);
            }
        }

        private void OnShown()
        {
            m_root.SetActive(false);
        }

        private void CloneCoin(float flyTime)
        {
            UnityEngine.UI.Image _cloneCoin = Instantiate(m_coinImage);
            _cloneCoin.transform.SetParent(transform);
            _cloneCoin.transform.position = m_coinImage.transform.position;
            _cloneCoin.transform.localScale = Vector3.one;

            _cloneCoin.transform.DOMove(m_coinEndPos.position, flyTime).SetEase(Ease.Linear);
            Destroy(_cloneCoin.gameObject, flyTime + 0.1f);
        }

        private void OnCoinNumberUpdate(float cur)
        {
            m_scoreUI.UpdateCoinText(System.Convert.ToInt32(cur));
        }

        private void OnRewardNumberUpdate(float cur)
        {
            m_acmountText.text = System.Convert.ToInt32(cur).ToString("N0");
        }

        private void OnAdShownFail(STORIAMonetization.Advertisement.AdvertisementManager.FailType failType)
        {
            GameAnalyticsSDK.GameAnalytics.NewAdEvent(GameAnalyticsSDK.GAAdAction.FailedShow, GameAnalyticsSDK.GAAdType.RewardedVideo, "Admob", "Gold");
            KahaGameCore.Common.TimerManager.Schedule(0.1f, Button_ShowAd);
        }
    }
}
