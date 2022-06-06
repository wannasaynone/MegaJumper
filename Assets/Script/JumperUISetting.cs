using UnityEngine;

namespace MegaJumper
{
    [CreateAssetMenu(menuName = "Jumper UI Setting")]
    public class JumperUISetting : ScriptableObject
    {
        public GameObject ModelRootPrefab { get { return m_modelRootPrefab; } }
        [SerializeField] private GameObject m_modelRootPrefab;

        public GameObject TextureButtonPrefab { get { return m_textureButtonPrefab; } }
        [SerializeField] private GameObject m_textureButtonPrefab;
    }
}