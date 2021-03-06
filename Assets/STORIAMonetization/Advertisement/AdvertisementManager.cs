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
        private bool m_isRemoved = false;

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

        public void SetAdIsRemoved()
        {
            m_isRemoved = true;
        }

        public AdUnitBase GetAdUnit()
        {
            return m_currentAdUnit;
        }

        public void ShowRewardVideo(Action onShown, Action<FailType> onFailed)
        {
            if (m_isRemoved)
            {
                onShown?.Invoke();
                return;
            }

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                onFailed?.Invoke(FailType.Disconnected);
                return;
            }

            if(m_currentAdUnit == null)
            {
                Debug.LogError("[AdvertisementManager][ShowRewardVideo] Need to set AdUnit up first. Did you initial ad plugins?");
                onFailed?.Invoke(FailType.SeeConsole);
                return;
            }

            m_currentAdUnit.ShowRewardVideo(onShown, onFailed);
        }

        public void ShowInterstitial(Action onShown, Action<FailType> onFailed)
        {
            if (m_isRemoved)
            {
                onShown?.Invoke();
                return;
            }

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                onFailed?.Invoke(FailType.Disconnected);
                return;
            }

            if (m_currentAdUnit == null)
            {
                Debug.LogError("[AdvertisementManager][ShowInterstitial] Need to set AdUnit up first. Did you initial ad plugins?");
                onFailed?.Invoke(FailType.SeeConsole);
                return;
            }

            m_currentAdUnit.ShowInterstitial(onShown, onFailed);
        }
    }
}

