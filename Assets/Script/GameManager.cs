using System;
using Zenject;

namespace MegaJumper
{
    public class GameManager : IInitializable, ITickable
    {
        private readonly BlockManager m_blockManager;
        private readonly ScoreManager m_scoreManager;
        private readonly SignalBus m_signalBus;
        private readonly Jumper m_jumper;
        private readonly GameProperties m_gameProperties;
        private readonly LocalSaveManager m_localSaveManager;

        private GameState.GameStateBase m_currentState;
        private JumperSetting m_jumperSetting;

        public GameManager(
            Jumper jumper, 
            BlockManager blockManager, 
            ScoreManager scoreManager, 
            LocalSaveManager localSaveManager,
            SignalBus signalBus, 
            GameProperties gameProperties)
        {
            m_jumper = jumper;
            m_blockManager = blockManager;
            m_scoreManager = scoreManager;
            m_localSaveManager = localSaveManager;
            m_signalBus = signalBus;
            m_gameProperties = gameProperties;

            m_localSaveManager.LoadAll();

            m_signalBus.Subscribe<Event.InGameEvent.OnGameStarted>(OnGameStarted);
            m_signalBus.Subscribe<Event.InGameEvent.OnJumpEnded>(OnJumpEnded);
            m_signalBus.Subscribe<Event.InGameEvent.OnGameResetCalled>(Initialize);
            m_signalBus.Subscribe<Event.InGameEvent.OnJumperSettingSet>(OnJumperSettingSet);
            m_signalBus.Subscribe<Event.InGameEvent.OnTutorialEnded>(OnTutorialEnded);
        }

        private void OnTutorialEnded()
        {
            m_localSaveManager.SaveDataInstance.SetIsTutorialEnded();
            m_localSaveManager.SaveAll();
        }

        private void OnJumperSettingSet(Event.InGameEvent.OnJumperSettingSet obj)
        {
            m_jumperSetting = obj.JumperSetting;
        }

        private void OnJumpEnded(Event.InGameEvent.OnJumpEnded obj)
        {
            if (!obj.IsSuccess)
            {
                ChangeState(new GameState.GameState_GameOver(m_signalBus));
            }
        }

        private void OnGameStarted()
        {
            ChangeState(new GameState.GameState_Gaming(
                m_blockManager, 
                m_gameProperties, 
                m_signalBus, 
                m_jumperSetting, 
                false, 
                !m_localSaveManager.SaveDataInstance.IsTutorialEnded));
        }

        public void Initialize()
        {
            m_signalBus.Fire(new Event.InGameEvent.OnJumperSettingSet(m_gameProperties.DEFAULF_JUMPER_SETTING));
            ChangeState(new GameState.GameState_WaitStart(m_scoreManager, m_blockManager, m_signalBus));
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