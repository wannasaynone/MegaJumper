using UnityEngine;
using Zenject;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;

namespace MegaJumper.UI
{
    public class GameResultView : MonoBehaviour
    {
        [SerializeField] private GameObject m_root;
        [SerializeField] private GameResultView_ScoreObject m_scoreObjectPrefab;
        [SerializeField] private RectTransform m_scoreObjectRoot;
        [SerializeField] private UnityEngine.UI.ScrollRect m_scoreObjectScroll;
        [SerializeField] private GameObject m_continueButtonRoot;
        [SerializeField] private float m_waitShowPanelTime;
        [SerializeField] private TMPro.TextMeshProUGUI m_totalCoinText;
        [SerializeField] private UnityEngine.UI.Image m_coinImage;
        [SerializeField] private RectTransform m_coinEndPos;
        [SerializeField] private RectTransform m_getCoinButtonRoot;

        private List<GameResultView_ScoreObject> m_cloneScoreObjects = new List<GameResultView_ScoreObject>();

        private SignalBus m_signalBus;
        private ScoreUIView m_scoreUI;
        private ProgressView m_progressView;
        private ScoreManager m_scoreManager;
        private LocalSaveManager m_localSaveManager;

        private int m_playerFinalCoin;

        [Inject]
        public void Constructor(SignalBus signalBus, ScoreUIView scoreUI, ProgressView progressView, ScoreManager scoreManager, LocalSaveManager localSaveManager)
        {
            m_signalBus = signalBus;
            m_scoreUI = scoreUI;
            m_progressView = progressView;
            m_scoreManager = scoreManager;
            m_localSaveManager = localSaveManager;
        }

        public void ShowWith(List<SettlementSetting> result, int orginMoneyNumber, System.Action onShown)
        {
            for (int i = 0; i < m_cloneScoreObjects.Count; i++)
            {
                m_cloneScoreObjects[i].gameObject.SetActive(false);
            }

            m_totalCoinText.text = "0";
            m_root.SetActive(true);
            m_continueButtonRoot.SetActive(false);
            m_getCoinButtonRoot.gameObject.SetActive(false);
            KahaGameCore.Common.TimerManager.Schedule(m_waitShowPanelTime, delegate { StartShowResult(result, orginMoneyNumber, onShown); });
        }

        private void StartShowResult(List<SettlementSetting> result, int orginMoneyNumber, System.Action onShown)
        {
            StartCoroutine(IEShowResult(result, orginMoneyNumber, onShown));
        }

        private IEnumerator IEShowResult(List<SettlementSetting> result, int orginMoneyNumber, System.Action onShown)
        {
            int _totalNow = 0;

            for (int i = 0; i < result.Count; i++)
            {
                if (i < m_cloneScoreObjects.Count)
                {
                    m_cloneScoreObjects[i].SetUp(result[i], m_scoreManager);
                    m_cloneScoreObjects[i].gameObject.SetActive(true);
                }
                else
                {
                    GameResultView_ScoreObject _clone = Instantiate(m_scoreObjectPrefab);
                    _clone.transform.SetParent(m_scoreObjectRoot);
                    _clone.transform.localScale = Vector3.one;
                    _clone.SetUp(result[i], m_scoreManager);

                    m_cloneScoreObjects.Add(_clone);
                }

                if (result[i].TimesScore)
                {
                    _totalNow += result[i].AddCoin * m_scoreManager.Score;
                }
                else
                {
                    _totalNow += result[i].AddCoin;
                }

                m_cloneScoreObjects[i].transform.localScale = Vector3.zero;
                m_cloneScoreObjects[i].transform.DOScale(new Vector3(1.1f, 1.1f, 1f), 0.15f);
                DOTween.To(GetScrollPos, SetScrollPos, new Vector2(0f, 0f), 0.15f);

                yield return new WaitForSeconds(0.15f);

                m_cloneScoreObjects[i].transform.DOScale(Vector3.one, 0.3f);
            }

            if (_totalNow <= 0)
            {
                ShowButton();
                onShown?.Invoke();
                yield break;
            }

            KahaGameCore.Common.GameUtility.RunNunber(0, _totalNow, 1f, OnTotalNumberUpdated, null);

            yield return new WaitForSeconds(1f);

            // VFX depends on score here
            m_totalCoinText.transform.DOScale(Vector3.one * 0.85f, 0.3f);

            yield return new WaitForSeconds(0.35f);

            // VFX depends on score here

            m_totalCoinText.transform.DOScale(Vector3.one, 0.15f);

            yield return new WaitForSeconds(0.5f);

            m_scoreUI.ShowCoinPanel(true);

            yield return new WaitForSeconds(0.5f);

            m_playerFinalCoin = orginMoneyNumber + _totalNow;

            KahaGameCore.Common.GameUtility.RunNunber(orginMoneyNumber, m_playerFinalCoin, 0.5f, OnCoinNumberUpdate, ShowButton);
            KahaGameCore.Common.GameUtility.RunNunber(_totalNow, 0, 0.5f, OnTotalNumberUpdated_noScale, onShown);

            float _flyTime = 0.2f;
            float _spawnGap = 0.02f;
            int _amount = System.Convert.ToInt32(0.25f / _spawnGap);
            for (int i = 0; i < _amount; i++)
            {
                CloneCoin(_flyTime);
                yield return new WaitForSeconds(_spawnGap);
            }
        }

        private void SetScrollPos(Vector2 newPos)
        {
            m_scoreObjectScroll.normalizedPosition = newPos;
        }

        private Vector2 GetScrollPos()
        {
            return m_scoreObjectScroll.normalizedPosition;
        }

        private void OnTotalNumberUpdated(float cur)
        {
            m_totalCoinText.text = System.Convert.ToInt32(cur).ToString();
            float _add = cur / 1000f;
            if (_add > 0.5f) _add = 0.5f;

            m_totalCoinText.transform.localScale = new Vector3(1f + _add, 1f + _add);
        }

        private void OnTotalNumberUpdated_noScale(float cur)
        {
            m_totalCoinText.text = System.Convert.ToInt32(cur).ToString();
        }

        private void OnCoinNumberUpdate(float cur)
        {
            m_scoreUI.UpdateCoinText(System.Convert.ToInt32(cur));
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

        private void ShowButton()
        {
            m_getCoinButtonRoot.gameObject.SetActive(true);
            KahaGameCore.Common.TimerManager.Schedule(1f, delegate
            {
                m_continueButtonRoot.SetActive(true);
            });
        }

        public static float showAdChance = -1f;
        public void Button_Next()
        {
            if (m_localSaveManager.SaveDataInstance.IsTutorial2Ended)
            {
                if (showAdChance < 0f)
                {
                    if (m_localSaveManager.SaveDataInstance.IsTutorial2Ended)
                    {
                        showAdChance = 100f;
                    }
                    else
                    {
                        showAdChance = 0f;
                    }
                }

                if (Random.Range(0f, 100f) <= showAdChance
                    || m_scoreManager.Score >= 100)
                {
                    m_continueButtonRoot.SetActive(false);
                    GameAnalyticsSDK.GameAnalytics.NewAdEvent(GameAnalyticsSDK.GAAdAction.Show, GameAnalyticsSDK.GAAdType.Interstitial, "Admob", "GameEnd");

                    STORIAMonetization.MonetizeCenter.Instance.AdManager.ShowInterstitial(OnAdShown, OnAdShownFail);
                }
                else
                {
                    showAdChance += 50f;
                    m_root.SetActive(false);
                    m_signalBus.Fire(new Event.InGameEvent.OnGameResetCalled(false));
                }
            }
            else
            {
                m_root.SetActive(false);
                m_progressView.Show();
            }
        }

        private void OnAdShown()
        {
            ProjectBS.Network.UnityThread.Do(ProcessReward);
        }

        // for process unity API in main thread
        private void ProcessReward()
        {
            GameAnalyticsSDK.GameAnalytics.NewAdEvent(GameAnalyticsSDK.GAAdAction.RewardReceived, GameAnalyticsSDK.GAAdType.Interstitial, "Admob", "GameEnd");

            showAdChance = 0f;
            m_root.SetActive(false);
            m_signalBus.Fire(new Event.InGameEvent.OnGameResetCalled(true));
        }

        private void OnAdShownFail(STORIAMonetization.Advertisement.AdvertisementManager.FailType failType)
        {
            GameAnalyticsSDK.GameAnalytics.NewAdEvent(GameAnalyticsSDK.GAAdAction.FailedShow, GameAnalyticsSDK.GAAdType.Interstitial, "Admob", "GameEnd");

            KahaGameCore.Common.TimerManager.Schedule(0.1f, RetryShowAd);
        }

        private void RetryShowAd()
        {
            STORIAMonetization.MonetizeCenter.Instance.AdManager.ShowInterstitial(OnAdShown, OnAdShownFail);
        }
    }
}