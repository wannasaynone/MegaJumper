using UnityEngine;

namespace MegaJumper
{
    [CreateAssetMenu(menuName = "Jumper Setting")]
    public class JumperSetting : ScriptableObject
    {
        public GameObject Prefab { get { return m_prefab; } }
        [SerializeField] private GameObject m_prefab;

        public Vector3 ModelOffset { get { return m_modelOffset; } }
        [SerializeField] private Vector3 m_modelOffset;

        public float GameOverDistance { get { return m_gameOverDistance; } }
        [SerializeField] private float m_gameOverDistance;

        public float ComboHitAdjust { get { return m_comboHitAdjust; } }
        [SerializeField] private float m_comboHitAdjust;

        public int FeverRequireCombo { get { return m_feverRequireCombo; } }
        [SerializeField] private int m_feverRequireCombo;

        public int FeverAddScore { get { return m_feverAddScore; } }
        [SerializeField] private int m_feverAddScore;

        public int AddidtionalAddScoreEveryJump { get { return m_addidtionalAddScoreEveryJump; } }
        [SerializeField] private int m_addidtionalAddScoreEveryJump;

        public int Life { get { return m_life; } }
        [SerializeField] private int m_life;

        public int NameID { get { return m_nameID; } }
        [SerializeField] private int m_nameID;

        public int DescriptionID { get { return m_descriptionID; } }
        [SerializeField] private int m_descriptionID;

        public bool SkipPerfectCheck { get { return m_skipPerfectCheck; } }
        [SerializeField] private bool m_skipPerfectCheck;

        public int UnlockPrice { get { return m_unlockPrice; } }
        [SerializeField] private int m_unlockPrice;
    }
}