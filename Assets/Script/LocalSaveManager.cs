using Zenject;

namespace MegaJumper
{
    public class LocalSaveManager 
    {
        private const string IS_TUTORIAL_ENDED = "IsTutorialEnded";
        private const string IS_TUTORIAL_2_ENDED = "IsTutorial2Ended";
        private const string COIN = "Coin";
        private const string LAST_DATE = "LastDate";
        private const string HIGHEST_SCORE_DAY = "HighestScore_Day";
        private const string HIGHEST_SCORE_ALL = "HighestScore_All";
        private const string UNLOCLED_JUMPERS = "UnlockedJumpers";
        private const string USING_JUMPER = "UsingJumper";
        private const string REMOVE_AD = "RemoveAd";

        private readonly SignalBus m_signalBus;

        public LocalSaveManager(SignalBus signalBus)
        {
            m_signalBus = signalBus;
        }

        public class SaveData
        {
            public bool IsTutorialEnded { get; private set; }
            public bool IsTutorial2Ended { get; private set; }
            public int Coin { get; private set; }
            public System.DateTime LastPlayDate { get; private set; }
            public int Highscore_Day { get; private set; }
            public int Highscore_All { get; private set; }
            public System.Collections.Generic.List<string> UnlockedJumpers { get; private set; }
            public string UsingJumper { get; private set; }
            public bool RemoveAd { get; private set; }

            private readonly SignalBus m_signalBus;

            private int m_score;

            public SaveData(SignalBus signalBus)
            {
                m_signalBus = signalBus;
            }

            public SaveData(
                SignalBus signalBus, 
                bool isTutorialEnded, 
                bool isTutorial2Ended,
                int coin,
                System.DateTime lastPlay,
                int highscore_day, 
                int highscore_all, 
                string unlockedJumpers,
                string usingJumper,
                bool removeAd)
            {
                m_signalBus = signalBus;
                IsTutorialEnded = isTutorialEnded;
                IsTutorial2Ended = isTutorial2Ended;
                Coin = coin;
                LastPlayDate = lastPlay;
                Highscore_Day = highscore_day;
                Highscore_All = highscore_all;
                RemoveAd = removeAd;

                UnlockedJumpers = new System.Collections.Generic.List<string>();
                string[] _jumpers = unlockedJumpers.Split(';');
                for (int i = 0; i < _jumpers.Length; i++)
                {
                    if (string.IsNullOrEmpty(_jumpers[i]))
                        continue;

                    UnlockedJumpers.Add(_jumpers[i]);
                }

                UsingJumper = usingJumper;

                m_signalBus.Subscribe<Event.InGameEvent.OnScoreAdded>(OnScoreAdded);
                m_signalBus.Subscribe<Event.InGameEvent.OnTutorialStart>(OnTutorialStart);
                m_signalBus.Subscribe<Event.InGameEvent.OnTutorialEnded>(OnTutorialEnded);
                m_signalBus.Subscribe<Event.InGameEvent.OnJumperSettingSet>(SetUse);

                if (IsPassOneDay())
                {
                    LastPlayDate = System.DateTime.Now;
                    Highscore_Day = 0;
                }
            }

            public void SetIsTutorialEnded()
            {
                GameAnalyticsSDK.GameAnalytics.NewDesignEvent("TutorialStage1Ended");
                IsTutorialEnded = true;
            }

            public void SetIsTutorial2Ended()
            {
                GameAnalyticsSDK.GameAnalytics.NewDesignEvent("TutorialStage2Ended");
                IsTutorial2Ended = true;
            }

            public void SetCoin(int value)
            {
                int _cur = Coin;

                long _longCotainer = value;
                if (_longCotainer > int.MaxValue)
                {
                    Coin = int.MaxValue;
                }
                else if (_longCotainer < 0)
                {
                    Coin = 0;
                }
                else
                {
                    Coin = (int)_longCotainer;
                }

                m_signalBus.Fire(new Event.InGameEvent.OnCoinAdded(Coin, Coin - _cur));
            }

            public void AddCoin(int add)
            {
                long _longCotainer = Coin + add;
                if (_longCotainer > int.MaxValue)
                {
                    Coin = int.MaxValue;
                }
                else if (_longCotainer < 0)
                {
                    Coin = 0;
                }
                else
                {
                    Coin = (int)_longCotainer;
                }

                m_signalBus.Fire(new Event.InGameEvent.OnCoinAdded(Coin, add));
            }

            public void Unlock(string name)
            {
                if (UnlockedJumpers.Contains(name))
                    return;

                UnlockedJumpers.Add(name);
                GameAnalyticsSDK.GameAnalytics.NewDesignEvent("UnlockJumperCount" + UnlockedJumpers.Count);
            }

            private void SetUse(Event.InGameEvent.OnJumperSettingSet obj)
            {
                UsingJumper = obj.JumperSetting.name;
            }

            private bool IsPassOneDay()
            {
                System.DateTime _cur = System.DateTime.Now;
                int _pass = _cur.Day - LastPlayDate.Day;

                return _pass > 0;
            }

            private void OnScoreAdded(Event.InGameEvent.OnScoreAdded obj)
            {
                m_score = obj.Current;
                if (m_score > Highscore_All)
                {
                    Highscore_All = m_score;
                }
                if (m_score > Highscore_Day)
                {
                    Highscore_Day = m_score;
                }
            }

            private void OnTutorialStart()
            {
                m_signalBus.Unsubscribe<Event.InGameEvent.OnScoreAdded>(OnScoreAdded);
            }

            private void OnTutorialEnded()
            {
                m_signalBus.Subscribe<Event.InGameEvent.OnScoreAdded>(OnScoreAdded);
            }

            public void SetRemoveAd()
            {
                RemoveAd = true;
            }
        }

        public SaveData SaveDataInstance { get; private set; }

        public void LoadAll()
        {
            System.DateTime _saveDate = System.DateTime.Parse(UnityEngine.PlayerPrefs.GetString(LAST_DATE, new System.DateTime(1991, 11, 14).ToString()));
            SaveDataInstance = new SaveData(
                m_signalBus,
                UnityEngine.PlayerPrefs.GetInt(IS_TUTORIAL_ENDED, 0) == 1,
                UnityEngine.PlayerPrefs.GetInt(IS_TUTORIAL_2_ENDED, 0) == 1,
                UnityEngine.PlayerPrefs.GetInt(COIN, 0),
                _saveDate,
                UnityEngine.PlayerPrefs.GetInt(HIGHEST_SCORE_DAY, 0),
                UnityEngine.PlayerPrefs.GetInt(HIGHEST_SCORE_ALL, 0),
                UnityEngine.PlayerPrefs.GetString(UNLOCLED_JUMPERS, "Husky"),
                UnityEngine.PlayerPrefs.GetString(USING_JUMPER, ""),
                UnityEngine.PlayerPrefs.GetInt(REMOVE_AD, 0) == 1);
        }

        public void SaveAll()
        {
            if (SaveDataInstance == null)
            {
                LoadAll();
            }
            UnityEngine.PlayerPrefs.SetInt(IS_TUTORIAL_ENDED, SaveDataInstance.IsTutorialEnded ? 1 : 0);
            UnityEngine.PlayerPrefs.SetInt(IS_TUTORIAL_2_ENDED, SaveDataInstance.IsTutorial2Ended ? 1 : 0);
            UnityEngine.PlayerPrefs.SetInt(COIN, SaveDataInstance.Coin);
            UnityEngine.PlayerPrefs.SetString(LAST_DATE, SaveDataInstance.LastPlayDate.ToString());
            UnityEngine.PlayerPrefs.SetInt(HIGHEST_SCORE_DAY, SaveDataInstance.Highscore_Day);
            UnityEngine.PlayerPrefs.SetInt(HIGHEST_SCORE_ALL, SaveDataInstance.Highscore_All);
            UnityEngine.PlayerPrefs.SetString(USING_JUMPER, SaveDataInstance.UsingJumper);
            UnityEngine.PlayerPrefs.SetInt(REMOVE_AD, SaveDataInstance.RemoveAd ? 1 : 0);

            string _newList = "";
            for (int i = 0; i < SaveDataInstance.UnlockedJumpers.Count; i++)
            {
                _newList += SaveDataInstance.UnlockedJumpers[i];
                if (i != SaveDataInstance.UnlockedJumpers.Count - 1)
                {
                    _newList += ";";
                }
            }
            UnityEngine.PlayerPrefs.SetString(UNLOCLED_JUMPERS, _newList);

            UnityEngine.PlayerPrefs.Save();
        }
    }
}