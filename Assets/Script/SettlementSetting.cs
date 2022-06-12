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
            GetScore,
            FeverFrequency,
            ContinuousFever
        }

        public SettlementType Type { get { return m_type; } }
        [SerializeField] private SettlementType m_type;

        public int Value { get { return m_value; } }
        [SerializeField] private int m_value;

        public int AddCoin { get { return m_addCoin; } }
        [SerializeField] private int m_addCoin;
    }
}