using UnityEngine;

namespace MegaJumper
{
    public class SoundEffectController : MonoBehaviour
    {
        public static SoundEffectController Instance { get; private set; }

        [SerializeField] private AudioSource[] m_se;

        private int m_currentIndex = -1;

        private void Awake()
        {
            Instance = this;
        }

        public void Play(AudioClip clip)
        {
            m_currentIndex++;
            if (m_currentIndex >= m_se.Length)
            {
                m_currentIndex = 0;
            }

            m_se[m_currentIndex].loop = false;
            m_se[m_currentIndex].clip = clip;
            m_se[m_currentIndex].Play();
        }

        public void Stop(AudioClip clip)
        {
            for (int i = 0; i < m_se.Length; i++)
            {
                if (m_se[i].clip == clip)
                {
                    m_se[i].Stop();
                }
            }
        }
    }
}