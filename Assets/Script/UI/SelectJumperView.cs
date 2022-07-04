using DG.Tweening;
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
        [SerializeField] private UnityEngine.UI.Image m_lockImage;
        [SerializeField] private UnityEngine.UI.Image m_lockImage_cover;
        [SerializeField] private UnityEngine.UI.Button m_selectJumperButton;
        [SerializeField] private UnityEngine.UI.Button m_unlockJumperButton;
        [SerializeField] private TMPro.TextMeshProUGUI m_unlockPriceText;
        [SerializeField] private Color m_normalColor;
        [SerializeField] private Color m_cantUnlockColor;
        [SerializeField] private GameObject m_unlockPanel;
        [SerializeField] private UnityEngine.UI.Image m_coinImage;
        [SerializeField] private RectTransform m_coinEndPos;
        [SerializeField] private GameObject m_unlockHint;
        [SerializeField] private GameObject m_removeAdButton;
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
        private ScoreUIView m_scoreUI;
        private GoldRewardView m_goldRewardView;

        private string m_curUsingJumper;

        [Inject]
        public void Constructor(SignalBus signalBus, LocalSaveManager localSaveManager, ScoreUIView scoreUIView, GoldRewardView goldRewardView)
        {
            m_signalBus = signalBus;
            m_localSaveManager = localSaveManager;
            m_scoreUI = scoreUIView;
            m_goldRewardView = goldRewardView;
            m_goldRewardView.OnCoinGained += M_goldRewardView_OnCoinGained;

            m_signalBus.Subscribe<Event.InGameEvent.OnGameStarted>(OnGameStarted);
            m_signalBus.Subscribe<Event.InGameEvent.OnGameResetCalled>(OnGameResetCalled);
            m_signalBus.Subscribe<Event.InGameEvent.OnScoreReset>(OnScoreReset);
        }

        private void M_goldRewardView_OnCoinGained()
        {
            SetUIWithIndex(m_currentIndex);
        }

        private void OnScoreReset()
        {
            m_enableButtonRoot.SetActive(m_localSaveManager.SaveDataInstance.IsTutorialEnded
                                         &&
                                        (m_localSaveManager.SaveDataInstance.UnlockedJumpers.Count >= 2 || m_localSaveManager.SaveDataInstance.Coin >= 3000));
            if (string.IsNullOrEmpty(m_localSaveManager.SaveDataInstance.UsingJumper))
            {
                m_signalBus.Fire(new Event.InGameEvent.OnJumperSettingSet(m_settings[0].JumperSetting));
                m_curUsingJumper = m_settings[0].JumperSetting.name;
            }
            else
            {
                SetJumperWithSave();
            }

            if (m_localSaveManager.SaveDataInstance.RemoveAd)
            {
                m_removeAdButton.SetActive(false);
            }
        }

        private void OnGameResetCalled(Event.InGameEvent.OnGameResetCalled obj)
        {
            m_enableButtonRoot.SetActive(m_localSaveManager.SaveDataInstance.IsTutorialEnded
                                         &&
                                        (m_localSaveManager.SaveDataInstance.UnlockedJumpers.Count >= 2 || m_localSaveManager.SaveDataInstance.Coin >= 3000));
        }

        private void SetJumperWithSave()
        {
            if (name == m_curUsingJumper)
            {
                return;
            }

            for (int i = 0; i < m_settings.Length; i++)
            {
                if (m_settings[i].JumperSetting.name == m_localSaveManager.SaveDataInstance.UsingJumper)
                {
                    m_signalBus.Fire(new Event.InGameEvent.OnJumperSettingSet(m_settings[i].JumperSetting));
                    m_curUsingJumper = m_settings[i].JumperSetting.name;
                    break;
                }
            }
        }

        private void OnGameStarted()
        {
            m_enableButtonRoot.SetActive(false);
            m_unlockHint.SetActive(false);
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

            if (active)
            {
                m_unlockHint.SetActive(false);
            }
        }

        public void Scroll_OnValueChanged(Vector2 pos)
        {
            if (m_isUnlocking)
                return;

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
            m_localSaveManager.SaveAll();
        }

        public void Button_Unlock()
        {
            if (m_localSaveManager.SaveDataInstance.Coin < m_settings[m_currentIndex].JumperSetting.UnlockPrice)
            {
                m_scoreUI.ShakeCoinPanel();
                StartCoroutine(IEShowGoldRewardView());
                return;
            }

            m_unlockPanel.SetActive(true);

            int _orginCoint = m_localSaveManager.SaveDataInstance.Coin;

            m_localSaveManager.SaveDataInstance.AddCoin(-m_settings[m_currentIndex].JumperSetting.UnlockPrice);
            m_localSaveManager.SaveDataInstance.Unlock(m_settings[m_currentIndex].JumperSetting.name);
            m_localSaveManager.SaveAll();

            KahaGameCore.Common.GameUtility.RunNunber(_orginCoint, m_localSaveManager.SaveDataInstance.Coin, 0.5f, OnCoinNumberUpdate, null);
            StartCoroutine(IEUnlock());
        }

        private IEnumerator IEShowGoldRewardView()
        {
            m_unlockPanel.SetActive(true);

            yield return new WaitForSeconds(0.5f);

            m_goldRewardView.Show();
            m_unlockPanel.SetActive(false);
        }

        private bool m_isUnlocking;
        private IEnumerator IEUnlock()
        {
            m_isUnlocking = true;

            float _flyTime = 0.2f;
            float _spawnGap = 0.05f;
            int _amount = Convert.ToInt32(0.25f / _spawnGap);
            for (int i = 0; i < _amount; i++)
            {
                CloneCoin(_flyTime);
                yield return new WaitForSeconds(_spawnGap);
            }

            yield return new WaitForSeconds(0.5f);

            DOTween.To(GetLockImageColor, SetLockImageColor, new Color(1f, 1f, 1f, 1f), 0.25f);

            yield return new WaitForSeconds(0.25f);

            m_lockImage.gameObject.SetActive(false);
            m_lockImage_cover.transform.DOShakePosition(2f, 50f, 20);

            yield return new WaitForSeconds(2f);

            m_lockImage_cover.transform.DOScale(Vector3.one * 3f, 0.15f);
            DOTween.To(GetLockImageColor, SetLockImageColor, new Color(1f, 1f, 1f, 0f), 0.15f);

            yield return new WaitForSeconds(0.15f);

            OnNumberRunEnded();
            m_isUnlocking = false;
        }

        private Color GetLockImageColor()
        {
            return m_lockImage_cover.color;
        }

        private void SetLockImageColor(Color color)
        {
            m_lockImage_cover.color = color;
        }

        private void OnCoinNumberUpdate(float cur)
        {
            m_scoreUI.UpdateCoinText(Convert.ToInt32(cur));
        }

        private void CloneCoin(float flyTime)
        {
            UnityEngine.UI.Image _cloneCoin = Instantiate(m_coinImage);
            _cloneCoin.transform.SetParent(transform);
            _cloneCoin.transform.position = m_coinImage.transform.position;
            _cloneCoin.transform.localScale = Vector3.one;

            _cloneCoin.transform.DOMove(m_coinEndPos.position, flyTime).SetEase(Ease.Linear);
            Destroy(_cloneCoin.gameObject, flyTime + 0.1f);
        }

        private void OnNumberRunEnded()
        {
            m_unlockPanel.SetActive(false);
            SetUIWithIndex(m_currentIndex);//
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

            bool _unlocked = m_localSaveManager.SaveDataInstance.UnlockedJumpers.Contains(_jumperSetting.name);
            m_lockImage.gameObject.SetActive(!_unlocked);
            m_lockImage_cover.color = new Color(1f, 1f, 1f, 0f);
            m_lockImage_cover.transform.localScale = Vector3.one;
            m_selectJumperButton.gameObject.SetActive(_unlocked);
            m_unlockJumperButton.gameObject.SetActive(!_unlocked);
            m_unlockPriceText.text = _jumperSetting.UnlockPrice.ToString("N0");

            if (m_localSaveManager.SaveDataInstance.Coin >= _jumperSetting.UnlockPrice)
            {
                m_unlockPriceText.color = m_normalColor;
            }
            else
            {
                m_unlockPriceText.color = m_cantUnlockColor;
                m_unlockPriceText.text += "\n<size=33>Not Enough Coin</size>";
            }
        }

        public void ShowChangeButtonUnlock()
        {
            m_signalBus.Fire(new Event.InGameEvent.OnGameResetCalled(false));
            StartCoroutine(IEChangeButtonUnlock());
        }

        private IEnumerator IEChangeButtonUnlock()
        {
            m_unlockPanel.SetActive(true);
            m_unlockHint.SetActive(true);
            m_unlockHint.transform.localScale = Vector3.zero;
            m_enableButtonRoot.transform.localScale = Vector3.zero;
            m_enableButtonRoot.transform.DOScale(Vector3.one * 1.1f, 0.5f);
            yield return new WaitForSeconds(0.25f);
            m_unlockHint.transform.DOScale(Vector3.one * 1.1f, 0.5f).OnComplete(delegate
            {
                m_unlockHint.transform.DOScale(Vector3.one, 0.25f);
            });
            yield return new WaitForSeconds(0.25f);
            m_enableButtonRoot.transform.DOScale(Vector3.one, 0.25f);
            yield return new WaitForSeconds(0.25f);
            m_unlockPanel.SetActive(false);
            m_localSaveManager.SaveDataInstance.SetIsTutorial2Ended();
            m_localSaveManager.SaveAll();
        }

        public void RemoveAllAd()
        {
            m_localSaveManager.SaveDataInstance.SetRemoveAd();
            m_localSaveManager.SaveAll();
            m_removeAdButton.SetActive(false);
            Monetization.AdmobAdUnit _admobUnit = STORIAMonetization.MonetizeCenter.Instance.AdManager.GetAdUnit() as Monetization.AdmobAdUnit;
            _admobUnit.HideBanner();
            STORIAMonetization.MonetizeCenter.Instance.AdManager.SetAdIsRemoved();
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