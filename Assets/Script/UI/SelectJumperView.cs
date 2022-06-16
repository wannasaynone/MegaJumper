using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace MegaJumper.UI
{
    public class SelectJumperView : MonoBehaviour
    {
        [SerializeField] private GameObject m_ingameRoot;
        [SerializeField] private GameObject[] m_ingameUI;
        [SerializeField] private GameObject m_selectJumperUIRoot;
        [SerializeField] private GameObject m_selectJumperModelRoot;
        [SerializeField] private GameObject m_enableButtonRoot;
        [SerializeField] private UnityEngine.UI.ScrollRect m_scrollRect;
        [SerializeField] private RectTransform m_selectButtonRect;
        [SerializeField] private Transform m_jumperModelRootParent;
        [SerializeField] private GameObject m_modelRootCameraRoot;
        [Header("Stats UI")]
        [SerializeField] private TMPro.TextMeshProUGUI m_nameText;
        [SerializeField] private UnityEngine.UI.Image m_accurateBarImage;
        [SerializeField] private UnityEngine.UI.Image m_feverScoreBarImage;
        [SerializeField] private UnityEngine.UI.Image m_comboNeedBarImage;
        [SerializeField] private TMPro.TextMeshProUGUI m_descriptionText;

        [SerializeField] private JumperUISetting[] m_settings;

        private List<GameObject> m_cloneModelRoot = new List<GameObject>();
        private List<GameObject> m_cloneTexture = new List<GameObject>();

        private SignalBus m_signalBus;
        private LocalSaveManager m_localSaveManager;

        [Inject]
        public void Constructor(SignalBus signalBus, LocalSaveManager localSaveManager)
        {
            m_signalBus = signalBus;
            m_localSaveManager = localSaveManager;
            m_signalBus.Subscribe<Event.InGameEvent.OnGameStarted>(OnGameStarted);
            m_signalBus.Subscribe<Event.InGameEvent.OnGameResetCalled>(OnGameResetCalled);
            signalBus.Subscribe<Event.InGameEvent.OnScoreReset>(OnScoreReset);
        }

        private void OnScoreReset()
        {
            m_enableButtonRoot.SetActive(false);
        }

        private void OnGameResetCalled()
        {
            if (m_localSaveManager.SaveDataInstance.IsTutorialEnded)
            {
                m_enableButtonRoot.SetActive(true);
            }
            else
            {
                m_enableButtonRoot.SetActive(false);
            }
        }

        private void OnGameStarted()
        {
            m_enableButtonRoot.SetActive(false);
        }

        private int m_currentIndex = 0;

        public void Button_SetActive(bool active)
        {
            m_ingameRoot.SetActive(!active);
            m_selectJumperModelRoot.SetActive(active);
            m_selectJumperUIRoot.SetActive(active);
            for (int i = 0; i < m_ingameUI.Length; i++)
            {
                m_ingameUI[i].SetActive(!active);
            }
            m_enableButtonRoot.SetActive(!active);

            if (m_cloneModelRoot.Count <= 0)
            {
                for (int i = 0; i < m_settings.Length; i++)
                {
                    GameObject _cloneModelRoot = Instantiate(m_settings[i].ModelRootPrefab);
                    _cloneModelRoot.transform.SetParent(m_jumperModelRootParent);
                    _cloneModelRoot.transform.localPosition = Vector3.right * 10f * i;
                    _cloneModelRoot.transform.localScale = Vector3.one;
                    m_cloneModelRoot.Add(_cloneModelRoot);

                    GameObject _cloneTexture = Instantiate(m_settings[i].TextureButtonPrefab);
                    _cloneTexture.transform.SetParent(m_selectButtonRect);
                    _cloneTexture.transform.localScale = Vector3.one;
                    m_cloneTexture.Add(_cloneTexture);
                }

                SetUIWithIndex(0);
            }
        }

        public void Scroll_OnValueChanged(Vector2 pos)
        {
            float _each = 1f / (m_cloneTexture.Count - 1);

            for (int i = 0; i < m_cloneTexture.Count; i++)
            {
                if (Mathf.Abs(pos.x - _each * i) <= _each / 2f)
                {
                    m_currentIndex = i;
                    SetUIWithIndex(m_currentIndex);
                    break;
                }
            }
        }

        public void Button_OnSelect()
        {
            Button_SetActive(false);
            m_signalBus.Fire(new Event.InGameEvent.OnJumperSettingSet(m_settings[m_currentIndex].JumperSetting));
        }

        private void SetUIWithIndex(int index)
        {
            m_modelRootCameraRoot.transform.localPosition = new Vector3(10f * index, 0f, 0f);

            JumperSetting _jumperSetting = m_settings[index].JumperSetting;
            JumperSetting _strongestSettint = m_settings[m_settings.Length - 1].JumperSetting;
            m_nameText.text = _jumperSetting.name;
            m_accurateBarImage.fillAmount = _jumperSetting.ComboHitAdjust / _strongestSettint.ComboHitAdjust;
            m_feverScoreBarImage.fillAmount = (float)_jumperSetting.FeverAddScore / (float)_strongestSettint.FeverAddScore;
            m_comboNeedBarImage.fillAmount = 0.25f + (0.75f - 0.75f * ((float)(_jumperSetting.FeverRequireCombo - _strongestSettint.FeverRequireCombo) * (1f / (float)(m_settings[0].JumperSetting.FeverRequireCombo - _strongestSettint.FeverRequireCombo))));
            m_descriptionText.text = _jumperSetting.Description;
        }

        private float m_waitTimer = 0f;
        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                return;
            }

            if (m_waitTimer > 0f)
            {
                m_waitTimer -= Time.deltaTime;
                return;
            }

            if (Input.GetMouseButtonUp(0))
            {
                m_waitTimer = 0.25f;
                return;
            }

            m_scrollRect.normalizedPosition = Vector2.Lerp(m_scrollRect.normalizedPosition,
                new Vector2(1f / (m_cloneTexture.Count - 1) * m_currentIndex, 0f), 0.1f);
        }
    }
}