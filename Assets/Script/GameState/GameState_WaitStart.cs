namespace MegaJumper.GameState
{
    public class GameState_WaitStart : GameStateBase
    {
        private readonly BlockManager m_blockManager;
        private readonly ScoreManager m_scoreManager;

        public GameState_WaitStart(ScoreManager scoreManager, BlockManager blockManager, Zenject.SignalBus signalBus) : base(signalBus)
        {
            m_blockManager = blockManager;
            m_scoreManager = scoreManager;
        }

        public override void Start()
        {
            m_scoreManager.Reset();
            m_blockManager.CreateNew();
            SignalBus.Subscribe<Event.InGameEvent.OnPointDown>(StartGame);
        }

        private void StartGame()
        {
            SignalBus.Fire<Event.InGameEvent.OnGameStarted>();
        }

        public override void Tick()
        {

        }

        public override void Stop()
        {
            SignalBus.Unsubscribe<Event.InGameEvent.OnPointDown>(StartGame);
        }
    }
}