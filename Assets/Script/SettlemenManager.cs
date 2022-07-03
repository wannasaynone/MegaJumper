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
        private int m_highestCombo = 0;

        public SettlemenManager(SettlementSettingContainer settingContainer, LocalSaveManager localSaveManager, Zenject.SignalBus signalBus)
        {
            m_settlementSettingContainer = settingContainer;
            m_saveManager = localSaveManager;

            signalBus.Subscribe<Event.InGameEvent.OnGameStarted>(OnGameStarted);
            signalBus.Subscribe<Event.InGameEvent.OnScoreAdded>(OnScoreAdded);
            signalBus.Subscribe<Event.InGameEvent.OnScoreReset>(OnScoreReset);
            signalBus.Subscribe<Event.InGameEvent.OnStartFever>(OnStartFever);
            signalBus.Subscribe<Event.InGameEvent.OnGameResetCalled>(OnGameResetCalled);
            signalBus.Subscribe<Event.InGameEvent.OnComboAdded>(OnComboAdded);
            signalBus.Subscribe<Event.InGameEvent.OnComboReset>(OnComboReset);
            signalBus.Subscribe<Event.InGameEvent.OnTutorialEnded>(OnTutorialEnded);
        }

        public List<SettlementSetting> GetResult()
        {
            //UnityEngine.Debug.Log("m_highscore_day=" + m_highscore_day);
            //UnityEngine.Debug.Log("m_highscore_all=" + m_highscore_all);
            //UnityEngine.Debug.Log("m_continuousFeverCount=" + m_continuousFeverCount);
            //UnityEngine.Debug.Log("m_feverFrequency=" + m_feverFrequency);
            //UnityEngine.Debug.Log("m_score=" + m_score);

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
                    case SettlementSetting.SettlementType.ReachScore:
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
                            if (m_score > m_highscore_all)
                            {
                                _result.Add(m_settlementSettingContainer.settlementSettings[i]);
                            }
                            break;
                        }
                    case SettlementSetting.SettlementType.HightestScore_Today:
                        {
                            if (m_score > m_highscore_day)
                            {
                                _result.Add(m_settlementSettingContainer.settlementSettings[i]);
                            }
                            break;
                        }
                    case SettlementSetting.SettlementType.Combo:
                        {
                            if (m_highestCombo >= m_settlementSettingContainer.settlementSettings[i].Value)
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
            m_highscore_all = m_saveManager.SaveDataInstance.Highscore_All;
            m_highscore_day = m_saveManager.SaveDataInstance.Highscore_Day;
        }

        private void OnScoreAdded(Event.InGameEvent.OnScoreAdded obj)
        {
            m_score = obj.Current;
        }

        private void OnScoreReset()
        {
            m_score = 0;
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

        private void OnGameResetCalled(Event.InGameEvent.OnGameResetCalled obj)
        {
            m_feverFrequency = 0;
            m_temp_continuousFeverCount = m_continuousFeverCount = m_highestCombo = 0;
        }

        private void OnComboAdded(Event.InGameEvent.OnComboAdded obj)
        {
            if (obj.Current > m_highestCombo)
            {
                m_highestCombo = obj.Current;
            }
        }

        private int m_temp_continuousFeverCount = 0;
        private void OnComboReset()
        {
            m_temp_continuousFeverCount = 0;
        }

        private void OnTutorialEnded()
        {
            m_feverFrequency = 0;
            m_temp_continuousFeverCount = m_continuousFeverCount = 0;
        }
    }
}