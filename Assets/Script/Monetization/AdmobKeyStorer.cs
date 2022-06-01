using UnityEngine;

namespace MegaJumper.Monetization
{
    [CreateAssetMenu(menuName = "Admob Key Storer")]
    public class AdmobKeyStorer : ScriptableObject
    {
        public string RewardAdID { get { return m_rewardID; } }
        [SerializeField] private string m_rewardID;
        public string InterstitialAdID { get { return m_interstitialID; } }
        [SerializeField] private string m_interstitialID;
    }
}