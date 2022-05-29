using UnityEngine;
using Zenject;
using TMPro;

namespace MegaJumper
{
    public class ScoreUIView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_scoreText;

        [Inject]
        public void Constructor(SignalBus signalBus, GameProperties gameProperties)
        {
            signalBus.Subscribe<Event.InGameEvent.OnScoreAdded>(OnScoreAdded);
            signalBus.Subscribe<Event.InGameEvent.OnScoreReset>(OnScoreReset);
        }

        private void OnScoreReset()
        {
            m_scoreText.text = "0";
        }

        private void OnScoreAdded(Event.InGameEvent.OnScoreAdded obj)
        {
            m_scoreText.text = obj.Current.ToString();
        }
    }
}