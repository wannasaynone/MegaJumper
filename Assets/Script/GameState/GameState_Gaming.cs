using Zenject;

namespace MegaJumper.GameState
{
    public class GameState_Gaming : GameStateBase
    {
        private readonly BlockManager m_blockManager;
        private readonly GameProperties m_gameProperties;
        private readonly ScoreManager m_scoreManager;

        public GameState_Gaming(ScoreManager scoreManager, BlockManager blockManager, GameProperties gameProperties, SignalBus signalBus) : base(signalBus)
        {
            m_gameProperties = gameProperties;
            m_blockManager = blockManager;
            m_scoreManager = scoreManager;
        }

        private void OnJumpEnded(Event.InGameEvent.OnJumpEnded obj)
        {
            if (obj.IsSuccess)
            {
                m_scoreManager.Add(1);
                m_blockManager.CreateNew();
            }
        }

        public override void Start()
        {
            SignalBus.Subscribe<Event.InGameEvent.OnJumpEnded>(OnJumpEnded);
        }

        public override void Tick()
        {
        }

        public override void Stop()
        {
            SignalBus.Unsubscribe<Event.InGameEvent.OnJumpEnded>(OnJumpEnded);
        }
    }
}