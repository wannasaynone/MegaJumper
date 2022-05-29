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

        public float ComboHitAdjust { get { return m_comboHitAdjust; } }
        [SerializeField] private float m_comboHitAdjust;

        public int FeverRequireCombo { get { return m_feverRequireCombo; } }
        [SerializeField] private int m_feverRequireCombo;

        public int FeverAddScore { get { return m_feverAddScore; } }
        [SerializeField] private int m_feverAddScore;

        public int Life { get { return m_life; } }
        [SerializeField] private int m_life;
    }
}