using Zenject;

namespace MegaJumper.GameState
{
    public class GameState_Gaming : GameStateBase
    {
        private readonly BlockManager m_blockManager;
        private readonly GameProperties m_gameProperties;

        public GameState_Gaming(BlockManager blockManager, GameProperties gameProperties, SignalBus signalBus) : base(signalBus)
        {
            m_gameProperties = gameProperties;
            m_blockManager = blockManager;
        }

        private void OnJumpEnded(Event.InGameEvent.OnJumpEnded obj)
        {
            float _dis = UnityEngine.Vector3.Distance(obj.Position, m_blockManager.GetLastBlockPosition());

            if (_dis > m_gameProperties.GAMEOVER_DIS)
            {
                SignalBus.Fire(new Event.InGameEvent.OnJumpFailDetected());
            }
            else
            {
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