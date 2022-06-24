using UnityEngine;
using STORIAMonetization.Advertisement;
using System;
using GoogleMobileAds.Api;

namespace MegaJumper.Monetization
{
    public class AdmobAdUnit : AdUnitBase
    {
        public static string TestRewardVidID = "ca-app-pub-3940256099942544/5224354917";

        private readonly string m_rewardID;
        private readonly string m_interID;
        private readonly string m_bannerID;

        private BannerView m_bannerView;
        private RewardedAd m_rewardVid;
        private InterstitialAd m_interstitialAd;

        private Action m_currentOnAdEnded = null;
        private Action<AdvertisementManager.FailType> m_currentOnFailed = null;

        public AdmobAdUnit(string rewardID, string interstitialID, string bannerID)
        {
            m_rewardID = rewardID;
            m_interID = interstitialID;
            m_bannerID = bannerID;

            m_rewardVid = new RewardedAd(m_rewardID);
            m_bannerView = new BannerView(m_bannerID, AdSize.Banner, AdPosition.Bottom);
            m_interstitialAd = new InterstitialAd(m_interID);

            AdRequest _bannerRequest = new AdRequest.Builder().Build();
            m_bannerView.LoadAd(_bannerRequest);

            SendRewardRequest();
            SendInterstitialRequest();
        }

        private void InitRewardEvent()
        {
            // Called when an ad request has successfully loaded.
            m_rewardVid.OnAdLoaded += HandleRewardedAdLoaded;
            // Called when an ad request failed to load.
            m_rewardVid.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
            // Called when an ad is shown.
            m_rewardVid.OnAdOpening += HandleRewardedAdOpening;
            // Called when an ad request failed to show.
            m_rewardVid.OnAdFailedToShow += HandleRewardedAdFailedToShow;
            // Called when the user should be rewarded for interacting with the ad.
            m_rewardVid.OnUserEarnedReward += HandleUserEarnedReward;
            // Called when the ad is closed.
            m_rewardVid.OnAdClosed += HandleRewardedAdClosed;
        }

        private void InitInterstitialEvent()
        {
            // Called when an ad request has successfully loaded.
            m_interstitialAd.OnAdLoaded += HandleOnAdLoaded;
            // Called when an ad request failed to load.
            m_interstitialAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
            // Called when an ad is shown.
            m_interstitialAd.OnAdOpening += HandleOnAdOpening;
            // Called when the ad is closed.
            m_interstitialAd.OnAdClosed += HandleOnAdClosed;
        }

        private void HandleOnAdClosed(object sender, EventArgs e)
        {
            m_currentOnAdEnded?.Invoke();
            m_currentOnAdEnded = null;
            SendInterstitialRequest();
        }

        private void HandleOnAdOpening(object sender, EventArgs e)
        {
        }

        private void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            Debug.LogError(e.LoadAdError.GetCode() + ":" + e.LoadAdError.GetMessage());
            m_currentOnAdEnded = null;
            m_currentOnFailed?.Invoke(AdvertisementManager.FailType.SeeConsole);
        }

        private void HandleOnAdLoaded(object sender, EventArgs e)
        {
        }

        private bool m_getReward;

        private void HandleRewardedAdClosed(object sender, EventArgs e)
        {
            if (m_getReward)
            {
                m_currentOnAdEnded?.Invoke();
            }

            m_currentOnAdEnded = null;
            m_getReward = false;
            SendRewardRequest();
        }

        private void HandleUserEarnedReward(object sender, Reward e)
        {
            m_getReward = true;
        }

        private void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs e)
        {
            Debug.LogError(e.AdError.GetCode() + ":" + e.AdError.GetMessage());
            m_currentOnAdEnded = null;
            m_currentOnFailed?.Invoke(AdvertisementManager.FailType.SeeConsole);
        }

        private void HandleRewardedAdOpening(object sender, EventArgs e)
        {
        }

        private void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            Debug.LogError(e.LoadAdError.GetCode() + ":" + e.LoadAdError.GetMessage());
            m_currentOnAdEnded = null;
            m_currentOnFailed?.Invoke(AdvertisementManager.FailType.SeeConsole);
        }

        private void HandleRewardedAdLoaded(object sender, EventArgs e)
        {
        }

        public override void ShowInterstitial(Action onShown, Action<AdvertisementManager.FailType> onFailed)
        {
            if (m_currentOnAdEnded != null)
            {
                Debug.LogError("ShowInterstitial was not completed but it is tryint to rise another one");
                onFailed?.Invoke(AdvertisementManager.FailType.SeeConsole);
                return;
            }

#if UNITY_EDITOR
            onShown?.Invoke();
#else
            m_currentOnAdEnded = onShown;
            m_currentOnFailed = onFailed;

            if (m_interstitialAd.IsLoaded())
            {
                m_interstitialAd.Show();
            }
            else
            {
                m_currentOnAdEnded = null;
                onFailed?.Invoke(AdvertisementManager.FailType.NotReadyYet);
                SendInterstitialRequest();
            }
#endif
        }

        public override void ShowRewardVideo(Action onShown, Action<AdvertisementManager.FailType> onFailed)
        {
            if (m_currentOnAdEnded != null)
            {
                Debug.LogError("ShowRewardVideo was not completed but it is tryint to rise another one");
                onFailed?.Invoke(AdvertisementManager.FailType.SeeConsole);
                return;
            }

#if UNITY_EDITOR
            onShown?.Invoke();
#else
            m_currentOnAdEnded = onShown;
            m_currentOnFailed = onFailed;

            if (m_rewardVid.IsLoaded())
            {
                m_rewardVid.Show();
            }
            else
            {
                m_currentOnAdEnded = null;
                onFailed?.Invoke(AdvertisementManager.FailType.NotReadyYet);
                SendRewardRequest();
            }
#endif
        }

        private void SendRewardRequest()
        {
            AdRequest _request = new AdRequest.Builder().Build();
            m_rewardVid.LoadAd(_request);
        }

        private void SendInterstitialRequest()
        {
            AdRequest _request = new AdRequest.Builder().Build();
            m_interstitialAd.LoadAd(_request);
        }
    }
}