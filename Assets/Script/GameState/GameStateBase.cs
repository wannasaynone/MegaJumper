namespace MegaJumper.GameState
{
    public abstract class GameStateBase
    {
        protected Zenject.SignalBus SignalBus { get; private set; }

        public GameStateBase(Zenject.SignalBus signalBus)
        {
            SignalBus = signalBus;
        }

        public abstract void Start();
        public abstract void Tick();
        public abstract void Stop();
    }
}