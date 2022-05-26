using UnityEngine;
using System;

namespace STORIAMonetization.Advertisement
{
    public class AdvertisementManager
    {
        public enum FailType
        {
            Disconnected,
            TimeOut,
            NotReadyYet,
            SeeConsole
        }

        private AdUnitBase m_currentAdUnit = null;

        public void SetAdUnit(AdUnitBase adUnit)
        {
            if(m_currentAdUnit != null)
            {
                throw new Exception("[AdvertisementManager][SetAdUnit] Can't have 2 AdUnit at same time. Please use RemoveAdUnit first.");
            }

            m_currentAdUnit = adUnit;
        }

        public void RemoveAdUnit()
        {
            m_currentAdUnit = null;
        }

        public void ShowRewardVideo(Action onShown, Action<FailType> onFailed)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                onFailed?.Invoke(FailType.Disconnected);
                return;
            }

            if(m_currentAdUnit == null)
            {
                throw new Exception("[AdvertisementManager][ShowRewardVideo] Need to set AdUnit up first. Did you initial ad plugins?");
            }

            m_currentAdUnit.ShowRewardVideo(onShown, onFailed);
        }

        public void ShowInterstitial(Action onShown, Action<FailType> onFailed)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                onFailed?.Invoke(FailType.Disconnected);
                return;
            }

            if (m_currentAdUnit == null)
            {
                throw new Exception("[AdvertisementManager][ShowInterstitial] Need to set AdUnit up first. Did you initial ad plugins?");
            }

            m_currentAdUnit.ShowRewardVideo(onShown, onFailed);
        }
    }
}

