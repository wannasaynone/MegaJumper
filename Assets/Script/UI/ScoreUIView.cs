using UnityEngine;
using Zenject;
using TMPro;

namespace MegaJumper
{
    public class ScoreUIView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_scoreText;
        [SerializeField] private TextMeshProUGUI m_comboText;

        private int m_currentHighest;

        [Inject]
        public void Constructor(SignalBus signalBus, GameProperties gameProperties)
        {
            signalBus.Subscribe<Event.InGameEvent.OnScoreAdded>(OnScoreAdded);
            signalBus.Subscribe<Event.InGameEvent.OnScoreReset>(OnScoreReset);
            signalBus.Subscribe<Event.InGameEvent.OnComboAdded>(OnComboAdded);
            signalBus.Subscribe<Event.InGameEvent.OnComboReset>(OnComboReset);
            m_scoreText.text = "0";
            m_comboText.text = "";
        }

        private void OnScoreReset()
        {
            m_scoreText.text = "0";
        }

        private void OnScoreAdded(Event.InGameEvent.OnScoreAdded obj)
        {
            if (obj.Current >= m_currentHighest)
            {
                m_scoreText.text = obj.Current.ToString();
                m_currentHighest = obj.Current;
            }
            else
            {
                m_scoreText.text = obj.Current.ToString() + " / " + m_currentHighest;
            }
        }

        private void OnComboAdded(Event.InGameEvent.OnComboAdded obj)
        {
            m_comboText.text = "Combo x" + obj.Current;
        }

        private void OnComboReset()
        {
            m_comboText.text = "";
        }
    }
}