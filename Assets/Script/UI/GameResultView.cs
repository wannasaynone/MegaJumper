using UnityEngine;
using Zenject;
using System.Collections.Generic;

namespace MegaJumper.UI
{
    public class GameResultView : MonoBehaviour
    {
        [SerializeField] private GameObject m_root;

        private SettlemenManager m_settlemenManager;
        private SignalBus m_signalBus;

        [Inject]
        public void Constructor(SettlemenManager settlemenManager, SignalBus signalBus)
        {
            m_settlemenManager = settlemenManager;
            m_signalBus = signalBus;
        }

        public void ShowWithCurrent()
        {
            List<SettlementSetting> _result = m_settlemenManager.GetResult();
            for (int i = 0; i < _result.Count; i++)
            {
                Debug.Log(_result[i].Type + " " + _result[i].Value);
            }
            m_root.SetActive(true);
        }

        public void Button_Next()
        {
            m_signalBus.Fire<Event.InGameEvent.OnGameResetCalled>();
            m_root.SetActive(false);
        }
    }
}