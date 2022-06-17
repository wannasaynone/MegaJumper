using Zenject;

namespace MegaJumper
{
    public class LocalSaveManager 
    {
        private const string IS_TUTORIAL_ENDED = "IsTutorialEnded";
        private const string COIN = "Coin";
        private const string LAST_DATE = "LastDate";
        private const string HIGHEST_SCORE_DAY = "HighestScore_Day";
        private const string HIGHEST_SCORE_ALL = "HighestScore_All";

        private readonly SignalBus m_signalBus;

        public LocalSaveManager(SignalBus signalBus)
        {
            m_signalBus = signalBus;
        }

        public class SaveData
        {
            public bool IsTutorialEnded { get; private set; }
            public int Coin { get; private set; }
            public System.DateTime LastPlayDate { get; private set; }
            public int Highscore_Day { get; private set; }
            public int Highscore_All { get; private set; }

            private readonly SignalBus m_signalBus;

            private int m_score;

            public SaveData(SignalBus signalBus)
            {
                m_signalBus = signalBus;
            }

            public SaveData(SignalBus signalBus, bool isTutorialEnded, int coin, System.DateTime lastPlay, int highscore_day, int highscore_all)
            {
                m_signalBus = signalBus;
                IsTutorialEnded = isTutorialEnded;
                Coin = coin;
                LastPlayDate = lastPlay;
                Highscore_Day = highscore_day;
                Highscore_All = highscore_all;

                m_signalBus.Subscribe<Event.InGameEvent.OnScoreAdded>(OnScoreAdded);
                m_signalBus.Subscribe<Event.InGameEvent.OnTutorialStart>(OnTutorialStart);
                m_signalBus.Subscribe<Event.InGameEvent.OnTutorialEnded>(OnTutorialEnded);

                if (IsPassOneDay())
                {
                    LastPlayDate = System.DateTime.Now;
                    Highscore_Day = 0;
                }
            }

            public void SetIsTutorialEnded()
            {
                IsTutorialEnded = true;
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

            private bool IsPassOneDay()
            {
                System.DateTime _cur = System.DateTime.Now;
                System.TimeSpan _pass = _cur - LastPlayDate;

                return _pass.Days > 0;
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
        }

        public SaveData SaveDataInstance { get; private set; }

        public void LoadAll()
        {
            System.DateTime _saveDate = System.DateTime.Parse(UnityEngine.PlayerPrefs.GetString(LAST_DATE, new System.DateTime(1991, 11, 14).ToString()));
            SaveDataInstance = new SaveData(
                m_signalBus,
                UnityEngine.PlayerPrefs.GetInt(IS_TUTORIAL_ENDED, 0) == 1,
                UnityEngine.PlayerPrefs.GetInt(COIN, 0),
                _saveDate,
                UnityEngine.PlayerPrefs.GetInt(HIGHEST_SCORE_DAY, 0),
                UnityEngine.PlayerPrefs.GetInt(HIGHEST_SCORE_ALL, 0));
        }

        public void SaveAll()
        {
            if (SaveDataInstance == null)
            {
                LoadAll();
            }
            UnityEngine.PlayerPrefs.SetInt(IS_TUTORIAL_ENDED, SaveDataInstance.IsTutorialEnded ? 1 : 0);
            UnityEngine.PlayerPrefs.SetInt(COIN, SaveDataInstance.Coin);
            UnityEngine.PlayerPrefs.SetString(LAST_DATE, SaveDataInstance.LastPlayDate.ToString());
            UnityEngine.PlayerPrefs.SetInt(HIGHEST_SCORE_DAY, SaveDataInstance.Highscore_Day);
            UnityEngine.PlayerPrefs.SetInt(HIGHEST_SCORE_ALL, SaveDataInstance.Highscore_All);
            UnityEngine.PlayerPrefs.Save();
        }
    }
}