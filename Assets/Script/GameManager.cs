using Zenject;

namespace MegaJumper
{
    public class GameManager : IInitializable, ITickable
    {
        private readonly BlockManager m_blockManager;
        private readonly SignalBus m_signalBus;

        private GameState.GameStateBase m_currentState;

        public GameManager(BlockManager blockManager, SignalBus signalBus)
        {
            m_blockManager = blockManager;
            m_signalBus = signalBus;

            m_signalBus.Subscribe<Event.InGameEvent.OnGameStarted>(OnGameStarted);
        }

        private void OnGameStarted()
        {
            m_currentState = new GameState.GameState_Gaming(m_signalBus);
            m_currentState.Start();
        }

        public void Initialize()
        {
            m_currentState = new GameState.GameState_WaitStart(m_blockManager, m_signalBus);
            m_currentState.Start();
        }

        public void Tick()
        {
            if (m_currentState != null)
            {
                m_currentState.Tick();
            }
        }
    }
}