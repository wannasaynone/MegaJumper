using System;
using Zenject;

namespace MegaJumper
{
    public class GameManager : IInitializable, ITickable
    {
        private readonly BlockManager m_blockManager;
        private readonly SignalBus m_signalBus;
        private readonly Jumper m_jumper;
        private readonly GameProperties m_gameProperties;

        private GameState.GameStateBase m_currentState;

        public GameManager(Jumper jumper, BlockManager blockManager, SignalBus signalBus, GameProperties gameProperties)
        {
            m_jumper = jumper;
            m_blockManager = blockManager;
            m_signalBus = signalBus;
            m_gameProperties = gameProperties;

            m_signalBus.Subscribe<Event.InGameEvent.OnGameStarted>(OnGameStarted);
            m_signalBus.Subscribe<Event.InGameEvent.OnJumpFailDetected>(OnJumpFailDetected);
            m_signalBus.Subscribe<Event.InGameEvent.OnGameResetCalled>(Initialize);
        }

        private void OnJumpFailDetected()
        {
            ChangeState(new GameState.GameState_GameOver(m_signalBus));
        }

        private void OnGameStarted()
        {
            ChangeState(new GameState.GameState_Gaming(m_blockManager, m_gameProperties, m_signalBus));
        }

        public void Initialize()
        {
            ChangeState(new GameState.GameState_WaitStart(m_blockManager, m_signalBus));
        }

        public void Tick()
        {
            if (m_currentState != null)
            {
                m_currentState.Tick();
            }
        }

        private void ChangeState(GameState.GameStateBase stateBase)
        {
            if (m_currentState != null)
            {
                m_currentState.Stop();
            }

            if (stateBase != null)
            {
                m_currentState = stateBase;
                m_currentState.Start();
            }
        }
    }
}