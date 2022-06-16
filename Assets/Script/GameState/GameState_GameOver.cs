using Zenject;

namespace MegaJumper.GameState
{
    public class GameState_GameOver : GameStateBase
    {
        private float m_timer;
        private readonly UI.GameResultView m_gameResultView;

        public GameState_GameOver(SignalBus signalBus, UI.GameResultView gameResultView) : base(signalBus)
        {
            m_gameResultView = gameResultView;
        }

        public override void Start()
        {
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
                m_gameResultView.ShowWithCurrent();
            }
        }
    }
}