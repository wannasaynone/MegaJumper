using Zenject;

namespace MegaJumper.GameState
{
    public class GameState_GameOver : GameStateBase
    {
        private float m_timer;
        private readonly UI.GameResultView m_gameResultView;
        private readonly SettlemenManager m_settlemenManager;
        private readonly LocalSaveManager m_localSave;

        private System.Collections.Generic.List<SettlementSetting> m_result;

        public GameState_GameOver(SignalBus signalBus, UI.GameResultView gameResultView, SettlemenManager settlemenManager, LocalSaveManager localSaveManager) : base(signalBus)
        {
            m_gameResultView = gameResultView;
            m_settlemenManager = settlemenManager;
            m_localSave = localSaveManager;
        }

        public override void Start()
        {
            m_result = m_settlemenManager.GetResult();
        }

        public override void Stop()
        {
        }

        public override void Tick()
        {
            if (m_timer >= 1.5f)
            {
                return;
            }

            m_timer += UnityEngine.Time.deltaTime;
            if (m_timer >= 1.5f)
            {
                m_gameResultView.ShowWith(m_result, m_localSave.SaveDataInstance.Coin, ApplyCoin);
            }
        }

        private void ApplyCoin()
        {
            int _total = 0;

            for (int i = 0; i < m_result.Count; i++)
            {
                _total += m_result[i].AddCoin;
            }

            m_localSave.SaveDataInstance.AddCoin(_total);
        }
    }
}