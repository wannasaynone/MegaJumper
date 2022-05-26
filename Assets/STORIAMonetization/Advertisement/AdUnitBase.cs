using System;
using FailType = STORIAMonetization.Advertisement.AdvertisementManager.FailType;

namespace STORIAMonetization.Advertisement
{
    public abstract class AdUnitBase
    {
        public bool IsRewardVideoReady { get; protected set; }
        public bool IsInterstitialReady { get; protected set; }

        public abstract void ShowRewardVideo(Action onShown, Action<FailType> onFailed);
        public abstract void ShowInterstitial(Action onShown, Action<FailType> onFailed);
    }
}

