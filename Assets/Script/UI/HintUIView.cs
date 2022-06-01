using UnityEngine;
using Zenject;

namespace MegaJumper.UI
{
    public class HintUIView : MonoBehaviour
    {
        [SerializeField] private GameObject m_contolHint;
        [SerializeField] private GameObject m_releaseHint;
        [SerializeField] private GameObject m_tutorialHint;
        [SerializeField] private GameObject m_gameStartHint;
        [SerializeField] private GameObject m_landHint;

        [Inject]
        public void Constructor(SignalBus signalBus)
        {
            signalBus.Subscribe<Event.InGameEvent.OnGameResetCalled>(OnGameReset);
        }

        private void OnGameReset()
        {
            EnableStartHint(true);
        }

        public void EnableStartHint(bool enable)
        {
            m_contolHint.SetActive(enable);
        }

        public void EnableReleaseHint(bool enable)
        {
            m_releaseHint.SetActive(enable);
        }

        public void EnableTutorialHint(bool enable)
        {
            m_tutorialHint.SetActive(enable);
        }

        public void EnableGameStartHint(bool enable)
        {
            m_gameStartHint.SetActive(enable);
        }

        public void EnableLandHint(bool enable)
        {
            m_landHint.SetActive(enable);
        }
    }
}

