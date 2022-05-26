using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace STORIAMonetization
{
    public class CommonADButtonSetter : MonoBehaviour
    {
        [System.Serializable]
        public class OnAdFailed : UnityEvent<string>
        {
        };

        public enum ADType
        {
            RewardVideo,
            Interstitial
        }

        [Header("Parts")]
        [SerializeField] private Button m_button = null;
        [Header("Setting")]
        [SerializeField] private ADType m_adType = ADType.RewardVideo;
        [Header("Events")]
        [SerializeField] private UnityEvent m_beforeAd = null;
        [SerializeField] private UnityEvent m_afterAd = null;
        [SerializeField] private OnAdFailed m_onFailed = null;

        private void Awake()
        {
            if(m_button == null)
            {
                throw new System.Exception("[MiniVaultADButton][Awake] m_button is null.");
            }
            m_button.onClick.AddListener(OnClicked);
        }

        public void EnableButton(bool enable)
        {
            m_button.interactable = enable;
        }

        private void OnClicked()
        {
            m_beforeAd?.Invoke();

            switch(m_adType)
            {
                case ADType.Interstitial:
                    {
                        MonetizeCenter.Instance.AdManager.ShowInterstitial(OnShown, OnFailed);
                        break;
                    }
                case ADType.RewardVideo:
                    {
                        MonetizeCenter.Instance.AdManager.ShowRewardVideo(OnShown, OnFailed);
                        break;
                    }
            }
        }

        private void OnShown()
        {
            m_afterAd?.Invoke();
        }

        private void OnFailed(Advertisement.AdvertisementManager.FailType failType)
        {
            m_onFailed?.Invoke(failType.ToString());
        }
    }
}
