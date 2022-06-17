using UnityEngine;
using Zenject;
using System.Collections.Generic;

namespace MegaJumper.UI
{
    public class GameResultView : MonoBehaviour
    {
        [SerializeField] private GameObject m_root;
        [SerializeField] private GameResultView_ScoreObject m_scoreObjectPrefab;
        [SerializeField] private RectTransform m_scoreObjectRoot;

        private List<GameResultView_ScoreObject> m_cloneScoreObjects = new List<GameResultView_ScoreObject>();

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

            for (int i = 0; i < m_cloneScoreObjects.Count; i++)
            {
                m_cloneScoreObjects[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < _result.Count; i++)
            {
                if (i < m_cloneScoreObjects.Count)
                {
                    m_cloneScoreObjects[i].SetUp(_result[i]);
                    m_cloneScoreObjects[i].gameObject.SetActive(true);
                }
                else
                {
                    GameResultView_ScoreObject _clone = Instantiate(m_scoreObjectPrefab);
                    _clone.transform.SetParent(m_scoreObjectRoot);
                    _clone.transform.localScale = Vector3.one;
                    _clone.SetUp(_result[i]);

                    m_cloneScoreObjects.Add(_clone);
                }
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