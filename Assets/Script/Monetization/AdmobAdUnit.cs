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
        private readonly string m_inerID;

        private RewardedAd m_rewardVid;

        private Action m_currentOnRewardEnded = null;
        private Action<AdvertisementManager.FailType> m_currentOnFailed = null;

        public AdmobAdUnit(string rewardID, string interstitialID)
        {
            m_rewardID = rewardID;
            m_inerID = interstitialID;

            m_rewardVid = new RewardedAd(m_rewardID);

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

            SendRewardRequest();
        }

        private bool m_getReward;

        private void HandleRewardedAdClosed(object sender, EventArgs e)
        {
            if (m_getReward)
            {
                m_currentOnRewardEnded?.Invoke();
            }

            m_currentOnRewardEnded = null;
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
            m_currentOnRewardEnded = null;
            m_currentOnFailed?.Invoke(AdvertisementManager.FailType.SeeConsole);
        }

        private void HandleRewardedAdOpening(object sender, EventArgs e)
        {
        }

        private void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            Debug.LogError(e.LoadAdError.GetCode() + ":" + e.LoadAdError.GetMessage());
            m_currentOnRewardEnded = null;
            m_currentOnFailed?.Invoke(AdvertisementManager.FailType.SeeConsole);
        }

        private void HandleRewardedAdLoaded(object sender, EventArgs e)
        {
        }

        public override void ShowInterstitial(Action onShown, Action<AdvertisementManager.FailType> onFailed)
        {
            if (string.IsNullOrEmpty(m_inerID))
            {
                Debug.LogError("interstitial id is null or empty.");
                onFailed?.Invoke(AdvertisementManager.FailType.SeeConsole);
                return;
            }

            throw new NotImplementedException();
        }

        public override void ShowRewardVideo(Action onShown, Action<AdvertisementManager.FailType> onFailed)
        {
            if (m_currentOnRewardEnded != null)
            {
                Debug.LogError("ShowRewardVideo was not completed but it is tryint to rise another one");
                onFailed?.Invoke(AdvertisementManager.FailType.SeeConsole);
                return;
            }

            m_currentOnRewardEnded = onShown;
            m_currentOnFailed = onFailed;
            Debug.Log(m_rewardID);

            if (m_rewardVid.IsLoaded())
            {
                m_rewardVid.Show();
            }
            else
            {
                m_currentOnRewardEnded = null;
                onFailed?.Invoke(AdvertisementManager.FailType.NotReadyYet);
                SendRewardRequest();
            }
        }

        private void SendRewardRequest()
        {
            AdRequest _request = new AdRequest.Builder().Build();
            m_rewardVid.LoadAd(_request);
        }
    }
}