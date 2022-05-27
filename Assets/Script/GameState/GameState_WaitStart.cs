namespace MegaJumper.GameState
{
    public class GameState_WaitStart : GameStateBase
    {
        private readonly BlockManager m_blockManager;

        public GameState_WaitStart(BlockManager blockManager, Zenject.SignalBus signalBus) : base(signalBus)
        {
            m_blockManager = blockManager;
        }

        public override void Start()
        {
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