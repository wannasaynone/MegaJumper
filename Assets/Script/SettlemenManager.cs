using System.Collections.Generic;

namespace MegaJumper
{
    public class SettlemenManager 
    {
        private readonly SettlementSettingContainer m_settlementSettingContainer;
        private readonly LocalSaveManager m_saveManager;

        private int m_highscore_day = 0;
        private int m_highscore_all = 0;

        private int m_continuousFeverCount = 0;
        private int m_feverFrequency = 0;
        private int m_score = 0;

        public SettlemenManager(SettlementSettingContainer settingContainer, LocalSaveManager localSaveManager, Zenject.SignalBus signalBus)
        {
            m_settlementSettingContainer = settingContainer;
            m_saveManager = localSaveManager;

            signalBus.Subscribe<Event.InGameEvent.OnGameStarted>(OnGameStarted);
            signalBus.Subscribe<Event.InGameEvent.OnScoreAdded>(OnScoreAdded);
            signalBus.Subscribe<Event.InGameEvent.OnStartFever>(OnStartFever);
            signalBus.Subscribe<Event.InGameEvent.OnGameResetCalled>(OnGameResetCalled);
            signalBus.Subscribe<Event.InGameEvent.OnJumpEnded>(OnJumpEnded);
            signalBus.Subscribe<Event.InGameEvent.OnTutorialEnded>(OnTutorialEnded);
        }

        public List<SettlementSetting> GetResult()
        {
            UnityEngine.Debug.Log("m_highscore_day=" + m_highscore_day);
            UnityEngine.Debug.Log("m_highscore_all=" + m_highscore_all);
            UnityEngine.Debug.Log("m_continuousFeverCount=" + m_continuousFeverCount);
            UnityEngine.Debug.Log("m_feverFrequency=" + m_feverFrequency);
            UnityEngine.Debug.Log("m_score=" + m_score);

            List<SettlementSetting> _result = new List<SettlementSetting>();
            for (int i = 0; i < m_settlementSettingContainer.settlementSettings.Length; i++)
            {
                switch (m_settlementSettingContainer.settlementSettings[i].Type)
                {
                    case SettlementSetting.SettlementType.ContinuousFever:
                        {
                            if (m_continuousFeverCount >= m_settlementSettingContainer.settlementSettings[i].Value)
                            {
                                _result.Add(m_settlementSettingContainer.settlementSettings[i]);
                            }
                            break;
                        }
                    case SettlementSetting.SettlementType.FeverFrequency:
                        {
                            if (m_feverFrequency >= m_settlementSettingContainer.settlementSettings[i].Value)
                            {
                                _result.Add(m_settlementSettingContainer.settlementSettings[i]);
                            }
                            break;
                        }
                    case SettlementSetting.SettlementType.GetScore:
                        {
                            if (m_score >= m_settlementSettingContainer.settlementSettings[i].Value)
                            {
                                _result.Add(m_settlementSettingContainer.settlementSettings[i]);
                            }
                            break;
                        }
                    case SettlementSetting.SettlementType.HightestScore_AllTime:
                        {
                            if (m_score >= m_highscore_all)
                            {
                                _result.Add(m_settlementSettingContainer.settlementSettings[i]);
                            }
                            break;
                        }
                    case SettlementSetting.SettlementType.HightestScore_Today:
                        {
                            if (m_score >= m_highscore_day)
                            {
                                _result.Add(m_settlementSettingContainer.settlementSettings[i]);
                            }
                            break;
                        }
                }
            }

            return _result;
        }

        private void OnGameStarted()
        {
            m_highscore_all = m_saveManager.SaveDataInstance.HighScore_All;
            m_highscore_day = m_saveManager.SaveDataInstance.HighScore_Day;
        }

        private void OnScoreAdded(Event.InGameEvent.OnScoreAdded obj)
        {
            m_score = obj.Current;
        }

        private void OnStartFever()
        {
            m_feverFrequency++;
            m_temp_continuousFeverCount++;
            if (m_temp_continuousFeverCount > m_continuousFeverCount)
            {
                m_continuousFeverCount = m_temp_continuousFeverCount;
            }
        }

        private void OnGameResetCalled()
        {
            m_feverFrequency = 0;
            m_temp_continuousFeverCount = m_continuousFeverCount = 0;
        }

        private int m_temp_continuousFeverCount = 0;
        private void OnJumpEnded(Event.InGameEvent.OnJumpEnded obj)
        {
            if (!obj.IsPerfect)
            {
                m_temp_continuousFeverCount = 0;
            }
        }

        private void OnTutorialEnded()
        {
            m_feverFrequency = 0;
            m_temp_continuousFeverCount = m_continuousFeverCount = 0;
        }
    }
}