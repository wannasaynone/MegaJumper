using UnityEngine;

namespace MegaJumper
{
    [CreateAssetMenu(menuName = "Settlement Setting")]
    public class SettlementSetting : ScriptableObject
    {
        public enum SettlementType
        {
            HightestScore_Today,
            HightestScore_AllTime,
            ReachScore,
            FeverFrequency,
            ContinuousFever,
            Combo,
            GetScore
        }

        public SettlementType Type { get { return m_type; } }
        [SerializeField] private SettlementType m_type;

        public int Value { get { return m_value; } }
        [SerializeField] private int m_value;

        public int AddCoin { get { return m_addCoin; } }
        [SerializeField] private int m_addCoin;

        public bool TimesScore { get { return m_timesScore; } }
        [SerializeField] private bool m_timesScore;

        public string Description { get { return m_description; } }
        [SerializeField] private string m_description;
    }
}