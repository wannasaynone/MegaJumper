using Zenject;

namespace MegaJumper.GameState
{
    public class GameState_GameOver : GameStateBase
    {
        private float m_timer;

        public GameState_GameOver(SignalBus signalBus) : base(signalBus)
        {
        }

        public override void Start()
        {
        }

        public override void Stop()
        {
        }

        public override void Tick()
        {
            if (m_timer >= 3f)
            {
                return;
            }

            m_timer += UnityEngine.Time.deltaTime;
            if (m_timer >= 1.5f)
            {
                SignalBus.Fire<Event.InGameEvent.OnGameResetCalled>();
            }
        }
    }
}