using UnityEngine;
using TMPro;

namespace MegaJumper.UI
{
    public class GameResultView_ScoreObject : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_descriptionText;
        [SerializeField] private TextMeshProUGUI m_coinText;

        public void SetUp(SettlementSetting setting)
        {
            m_descriptionText.text = setting.Description;
            m_coinText.text = setting.AddCoin.ToString();
        }
    }
}