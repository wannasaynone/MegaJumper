namespace MegaJumper
{
    public class LocalSaveManager 
    {
        private const string IS_TUTORIAL_ENDED = "IsTutorialEnded";

        public class SaveData
        {
            public bool IsTutorialEnded { get; private set; }

            public SaveData() { }

            public SaveData(bool isTutorialEnded)
            {
                IsTutorialEnded = isTutorialEnded;
            }

            public void SetIsTutorialEnded()
            {
                IsTutorialEnded = true;
            }
        }

        public SaveData SaveDataInstance { get; private set; }

        public void LoadAll()
        {
            SaveDataInstance = new SaveData(
                UnityEngine.PlayerPrefs.GetInt(IS_TUTORIAL_ENDED, 0) == 1);
        }

        public void SaveAll()
        {
            if (SaveDataInstance == null)
                return;
            UnityEngine.Debug.Log("Save All");
            UnityEngine.PlayerPrefs.SetInt(IS_TUTORIAL_ENDED, SaveDataInstance.IsTutorialEnded ? 1 : 0);
            UnityEngine.PlayerPrefs.Save();
        }
    }
}