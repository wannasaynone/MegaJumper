using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaJumper.UI
{
    public class SelectJumperView : MonoBehaviour
    {
        [SerializeField] private GameObject m_ingameRoot;
        [SerializeField] private GameObject m_ingameUI;
        [SerializeField] private GameObject m_selectJumperUIRoot;
        [SerializeField] private GameObject m_selectJumperModelRoot;
        [SerializeField] private GameObject m_enableButtonRoot;
        [SerializeField] private UnityEngine.UI.ScrollRect m_scrollRect;
        [SerializeField] private RectTransform m_selectButtonRect;
        [SerializeField] private Transform m_jumperModelRootParent;

        [SerializeField] private JumperUISetting[] m_settings;

        private List<GameObject> m_cloneModelRoot = new List<GameObject>();
        private List<GameObject> m_cloneTexture = new List<GameObject>();

        public void Button_SetActive(bool active)
        {
            m_ingameRoot.SetActive(!active);
            m_selectJumperModelRoot.SetActive(active);
            m_selectJumperUIRoot.SetActive(active);
            m_ingameUI.SetActive(!active);
            m_enableButtonRoot.SetActive(!active);

            for (int i = 0; i < m_cloneModelRoot.Count; i++)
            {
                if (m_cloneModelRoot[i] != null)
                {
                    Destroy(m_cloneModelRoot[i]);
                }
            }
            m_cloneModelRoot.Clear();

            for (int i = 0; i < m_cloneTexture.Count; i++)
            {
                if (m_cloneTexture[i] != null)
                {
                    Destroy(m_cloneTexture[i]);
                }
            }
            m_cloneTexture.Clear();

            for (int i = 0; i < m_settings.Length; i++)
            {
                GameObject _cloneModelRoot = Instantiate(m_settings[i].ModelRootPrefab);
                _cloneModelRoot.transform.SetParent(m_jumperModelRootParent);
                _cloneModelRoot.transform.localPosition = Vector3.right * 5f * i;
                _cloneModelRoot.transform.localScale = Vector3.one;
                m_cloneModelRoot.Add(_cloneModelRoot);

                GameObject _cloneTexture = Instantiate(m_settings[i].TextureButtonPrefab);
                _cloneTexture.transform.SetParent(m_selectButtonRect);
                _cloneTexture.transform.localScale = Vector3.one;
                m_cloneTexture.Add(_cloneTexture);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                float _each = 1f / (float)(m_cloneTexture.Count - 1);
                m_scrollRect.normalizedPosition = new Vector2(_each * 0f, 0f);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                float _each = 1f / (float)(m_cloneTexture.Count - 1);
                m_scrollRect.normalizedPosition = new Vector2(_each * 1f, 0f);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                float _each = 1f / (float)(m_cloneTexture.Count - 1);
                m_scrollRect.normalizedPosition = new Vector2(_each * 2f, 0f);
            }
        }
    }
}