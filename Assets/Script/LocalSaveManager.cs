using Zenject;

namespace MegaJumper
{
    public class LocalSaveManager 
    {
        private const string IS_TUTORIAL_ENDED = "IsTutorialEnded";
        private const string COIN = "Coin";

        private readonly SignalBus m_signalBus;

        public LocalSaveManager(SignalBus signalBus)
        {
            m_signalBus = signalBus;
        }

        public class SaveData
        {
            public bool IsTutorialEnded { get; private set; }
            public int Coin { get; private set; }

            private readonly SignalBus m_signalBus;

            public SaveData(SignalBus signalBus)
            {
                m_signalBus = signalBus;
            }

            public SaveData(SignalBus signalBus, bool isTutorialEnded, int coin)
            {
                m_signalBus = signalBus;
                IsTutorialEnded = isTutorialEnded;
                Coin = coin;
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
        }

        public SaveData SaveDataInstance { get; private set; }

        public void LoadAll()
        {
            SaveDataInstance = new SaveData(
                m_signalBus,
                UnityEngine.PlayerPrefs.GetInt(IS_TUTORIAL_ENDED, 0) == 1,
                UnityEngine.PlayerPrefs.GetInt(COIN, 0));
        }

        public void SaveAll()
        {
            if (SaveDataInstance == null)
            {
                LoadAll();
            }
            UnityEngine.PlayerPrefs.SetInt(IS_TUTORIAL_ENDED, SaveDataInstance.IsTutorialEnded ? 1 : 0);
            UnityEngine.PlayerPrefs.SetInt(COIN, SaveDataInstance.Coin);
            UnityEngine.PlayerPrefs.Save();
        }
    }
}