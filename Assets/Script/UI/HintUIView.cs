using UnityEngine;

namespace MegaJumper.UI
{
    public class HintUIView : MonoBehaviour
    {
        [SerializeField] private GameObject m_contolHint;
        [SerializeField] private GameObject m_releaseHint;

        public void EnableControlHint(bool enable)
        {
            m_contolHint.SetActive(enable);
        }

        public void EnableReleaseHint(bool enable)
        {
            m_releaseHint.SetActive(enable);
        }
    }
}

