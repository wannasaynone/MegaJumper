using UnityEngine;
using TMPro;

namespace MegaJumper.UI
{
    public class GameResultView_ScoreObject : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_descriptionText;
        [SerializeField] private TextMeshProUGUI m_coinText;

        public void SetUp(SettlementSetting setting, ScoreManager scoreManager)
        {
            m_descriptionText.text = ProjectBS.ContextConverter.Instance.GetContext(setting.Description);
            if (setting.TimesScore)
            {
                m_coinText.text = (setting.AddCoin * scoreManager.Score).ToString();
            }
            else
            {
                m_coinText.text = setting.AddCoin.ToString();
            }
        }
    }
}