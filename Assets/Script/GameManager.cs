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
        private readonly UI.HintUIView m_hintView;
        private readonly UI.GameResultView m_resultView;
        private readonly SettlemenManager m_settlementManager;

        private GameState.GameStateBase m_currentState;
        private JumperSetting m_jumperSetting;

        private bool m_startWithFeverNext = false;

        public GameManager(
            Jumper jumper, 
            BlockManager blockManager, 
            ScoreManager scoreManager, 
            LocalSaveManager localSaveManager,
            SettlemenManager settlemenManager,
            SignalBus signalBus, 
            GameProperties gameProperties,
            UI.HintUIView hintUIView,
            UI.GameResultView gameResultView)
        {
            m_jumper = jumper;
            m_blockManager = blockManager;
            m_scoreManager = scoreManager;
            m_localSaveManager = localSaveManager;
            m_signalBus = signalBus;
            m_gameProperties = gameProperties;
            m_hintView = hintUIView;
            m_resultView = gameResultView;
            m_settlementManager = settlemenManager;

            m_signalBus.Subscribe<Event.InGameEvent.OnGameStarted>(OnGameStarted);
            m_signalBus.Subscribe<Event.InGameEvent.OnJumpEnded>(OnJumpEnded);
            m_signalBus.Subscribe<Event.InGameEvent.OnGameResetCalled>(OnGameReset);
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
                ChangeState(new GameState.GameState_GameOver(m_signalBus, m_resultView, m_settlementManager, m_localSaveManager, m_scoreManager));
            }
        }

        private void OnGameStarted()
        {
            ChangeState(new GameState.GameState_Gaming(
                m_blockManager, 
                m_gameProperties, 
                m_signalBus, 
                m_jumperSetting,
                m_startWithFeverNext, 
                !m_localSaveManager.SaveDataInstance.IsTutorialEnded));
        }

        public void Initialize()
        {
            m_localSaveManager.LoadAll();

            if (m_localSaveManager.SaveDataInstance.RemoveAd)
            {
                UnityEngine.GameObject _banner = UnityEngine.GameObject.Find("BANNER(Clone)");
                UnityEngine.Object.Destroy(_banner);
            }

            m_signalBus.Fire(new Event.InGameEvent.OnCoinAdded(m_localSaveManager.SaveDataInstance.Coin, 0));
            OnGameReset(new Event.InGameEvent.OnGameResetCalled(false));
        }

        private void OnGameReset(Event.InGameEvent.OnGameResetCalled obj)
        {
            m_startWithFeverNext = obj.StartWithFever;
            m_hintView.EnableStartHint(true);
            m_localSaveManager.SaveAll();
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