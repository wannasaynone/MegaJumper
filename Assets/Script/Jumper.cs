using DG.Tweening;
using UnityEngine;
using Zenject;

namespace MegaJumper
{
    public class Jumper : MonoBehaviour
    {
        [SerializeField] private Rigidbody m_rigidbody;
        [SerializeField] private Collider m_collider;
        [SerializeField] private GameObject m_hintObject;
        [SerializeField] private ParticleSystem m_jumpVfx;
        [SerializeField] private ParticleSystem m_landingVfx;
        [SerializeField] private MoreMountains.Feedbacks.MMF_Player m_landingFeedback;
        [SerializeField] private UnityEngine.UI.Image[] m_hintImages;
        [Header("Aduio")]
        [SerializeField] private AudioClip m_holdClip;
        [SerializeField] private AudioClip m_jumpClip;
        [SerializeField] private AudioClip m_landClip;
        [SerializeField] private AudioClip m_perfectLandClip;

        private enum State
        {
            Idle,
            Pressing,
            Jumping,
            Died,
            FeverPressing
        }

        private State m_currentState = State.Idle;

        private GameProperties m_gameProperties;
        private SignalBus m_signalBus;
        private UI.HintUIView m_hintView;

        private bool m_skipNextUp = false;
        private Block m_currentDirectionBlock;
        private Block m_lastBlock;

        private JumperSetting m_jumperSetting;
        private int m_remainingLife;
        private GameObject m_jumperModel;

        private string m_animator_idle = "Idle_A";
        private string m_animator_roll = "Roll";
        private Animator[] m_animators;

        private int m_tutorialMode_feverTimes = 99;

        [Inject]
        public void Constructor(SignalBus signalBus, GameProperties gameProperties, UI.HintUIView hintUIView)
        {
            m_signalBus = signalBus;
            m_gameProperties = gameProperties;
            m_hintView = hintUIView;

            signalBus.Subscribe<Event.InGameEvent.OnGameStarted>(OnGameStart);
            signalBus.Subscribe<Event.InGameEvent.OnPointDown>(OnPointDown);
            signalBus.Subscribe<Event.InGameEvent.OnPointUp>(OnPointUp);
            signalBus.Subscribe<Event.InGameEvent.OnGameResetCalled>(OnGameResetCalled);
            signalBus.Subscribe<Event.InGameEvent.OnBlockSpawned>(OnBlockSpawned);
            signalBus.Subscribe<Event.InGameEvent.OnJumperSettingSet>(OnJumperSettingSet);
            signalBus.Subscribe<Event.InGameEvent.OnStartFever>(OnStartFever);
            signalBus.Subscribe<Event.InGameEvent.OnTutorialStart>(OnTutorialStart);
            signalBus.Subscribe<Event.InGameEvent.OnTutorialEnded>(OnTutorialEnded);

            m_hintObject.transform.SetParent(null);
        }

        private void OnTutorialStart()
        {
            m_tutorialMode_feverTimes = 0;
            m_hintObject.SetActive(true);
        }

        private void OnTutorialEnded()
        {
            m_tutorialMode_feverTimes = 99;
        }

        private void OnStartFever()
        {
            m_currentState = State.FeverPressing;
            m_jumperModel.transform.DOShakePosition(m_gameProperties.FEVER_ANIMATION_TIME + 0.5f).OnComplete(OnFeverShakeEnded);
        }

        private void OnFeverShakeEnded()
        {
            m_rigidbody.transform.localScale = Vector3.one;
            m_rigidbody.transform.localPosition = Vector3.zero + Vector3.up * 2f;
            PlayAnimation(m_animator_roll);
            StartJump(m_currentDirectionBlock.transform.position, (float)m_jumperSetting.FeverAddScore / 2f, true, OnFeverJumpEnded);
        }

        private void OnFeverJumpEnded()
        {
            m_currentState = State.Idle;
            CreateVFX(m_landingVfx, Vector3.up, m_pressTime);
            m_landingFeedback.PlayFeedbacks();
            PlayAnimation(m_animator_idle);

            m_signalBus.Fire(new Event.InGameEvent.OnJumpEnded(transform.position, true, false, m_remainingLife));
            m_signalBus.Fire<Event.InGameEvent.OnFeverEnded>();

            if (!IsTutorialEnded())
            {
                m_tutorialMode_feverTimes++;
                if (IsTutorialEnded())
                {
                    m_hintView.EnableGameStartHint(true);
                    m_signalBus.Fire<Event.InGameEvent.OnTutorialEnded>();
                }
                else
                {
                    m_hintView.EnableStartHint(true);
                }
            }

            SoundEffectController.Instance.Play(m_perfectLandClip);
        }

        private void OnJumperSettingSet(Event.InGameEvent.OnJumperSettingSet obj)
        {
            SetSetting(obj.JumperSetting);
        }

        private void SetSetting(JumperSetting jumperSetting)
        {
            m_jumperSetting = jumperSetting;
            m_remainingLife = m_jumperSetting.Life;

            if (m_jumperModel != null)
            {
                Destroy(m_jumperModel);
            }

            transform.rotation = Quaternion.identity;
            m_jumperModel = Instantiate(m_jumperSetting.Prefab);
            m_jumperModel.transform.SetParent(m_rigidbody.transform);
            m_jumperModel.transform.localRotation = Quaternion.identity;
            m_jumperModel.transform.localPosition = Vector3.zero + m_jumperSetting.ModelOffset;
            m_jumperModel.transform.localScale = Vector3.one * 1.25f;
            m_animators = m_jumperModel.GetComponentsInChildren<Animator>();
        }

        private void OnBlockSpawned(Event.InGameEvent.OnBlockSpawned obj)
        {
            m_lastBlock = m_currentDirectionBlock;
            m_currentDirectionBlock = obj.Block;
            Vector3 _cur = transform.eulerAngles;
            transform.LookAt(m_currentDirectionBlock.transform.position);
            Vector3 _new = transform.eulerAngles;
            transform.eulerAngles = _cur;
            transform.DORotate(_new, 0.2f);
        }

        private void OnPointDown()
        {
            if (m_currentState != State.Idle)
            {
                m_skipNextUp = true;
                return;
            }

            m_currentState = State.Pressing;
            m_hintObject.transform.position = transform.position + Vector3.up;

            m_hintView.EnableStartHint(false);
            m_hintView.EnableTutorialHint(false);
            m_hintView.EnableLandHint(false);
            m_hintView.EnableGameStartHint(false);

            m_hintObject.SetActive(m_tutorialMode_feverTimes < 2);
            for (int i = 0; i < m_hintImages.Length; i++)
            {
                m_hintImages[i].color = new Color(m_hintImages[i].color.r, m_hintImages[i].color.g, m_hintImages[i].color.b, 1f);
            }

            SoundEffectController.Instance.Play(m_holdClip);
        }

        private void OnGameResetCalled(Event.InGameEvent.OnGameResetCalled obj)
        {
            transform.position = Vector3.zero;
            m_rigidbody.isKinematic = true;
            m_collider.isTrigger = true;
            m_lastBlock = m_currentDirectionBlock = null;
            SetIdle();
            SetSetting(m_jumperSetting);
        }

        private void OnGameStart()
        {
            m_currentState = State.Idle;
            OnPointDown();
        }

        private float m_pressTime;
        private void OnPointUp(Event.InGameEvent.OnPointUp obj)
        {
            if (m_skipNextUp)
            {
                m_skipNextUp = false;
                return;
            }

            m_hintView.EnableReleaseHint(false);
            m_hintObject.SetActive(false);
            m_rigidbody.transform.localScale = Vector3.one;
            m_rigidbody.transform.localPosition = Vector3.zero + Vector3.up * 2f;

            Vector3 _finalPos = transform.position;

            if (!IsTutorialEnded() && Vector3.Distance(m_hintObject.transform.position - Vector3.up, m_currentDirectionBlock.transform.position) > m_jumperSetting.GameOverDistance)
            {
                m_remainingLife++;
            }

            if (m_currentDirectionBlock == null)
            {
                _finalPos += Vector3.forward * obj.PressTime * m_gameProperties.MOVE_DIS_PER_SEC;
            }
            else
            {
                _finalPos += (m_currentDirectionBlock.transform.position - transform.position).normalized * obj.PressTime * m_gameProperties.MOVE_DIS_PER_SEC;
            }

            if (m_lastBlock != null)
            {
                if (Vector3.Distance(_finalPos, m_lastBlock.transform.position) <= m_jumperSetting.GameOverDistance)
                {
                    SetIdle();
                    return;
                }
            }
            else
            {

                if (Vector3.Distance(_finalPos, transform.position) <= m_jumperSetting.GameOverDistance)
                {
                    SetIdle();
                    return;
                }
            }

            float _adjust = m_jumperSetting.ComboHitAdjust;
            if (!IsTutorialEnded()) _adjust += 0.3f;

            if (Vector3.Distance(_finalPos, m_currentDirectionBlock.transform.position) <= _adjust)
            {
                _finalPos = m_currentDirectionBlock.transform.position;
            }

            StartJump(_finalPos, obj.PressTime, false, OnJumpEnded);
        }

        private void StartJump(Vector3 endPos, float pressTime, bool isFever, TweenCallback onEnded)
        {
            if (UnityEngine.PlayerPrefs.GetInt("Played", 0) == 0)
            {
                UnityEngine.PlayerPrefs.SetInt("Played", 1);
                UnityEngine.PlayerPrefs.Save();
                GameAnalyticsSDK.GameAnalytics.NewDesignEvent("NewPlayer");
            }

            if (m_currentDirectionBlock.CurrentBlockType == Block.BlockType.MoveRepeat)
            {
                m_currentDirectionBlock.SetType(Block.BlockType.None);
            }
            CreateVFX(m_jumpVfx, Vector3.up, pressTime);

            transform.DOJump(endPos, pressTime * m_gameProperties.JUMP_FORCE, 1, pressTime * m_gameProperties.JUMP_TIME_SCALE).SetEase(Ease.Linear).OnComplete(onEnded);

            m_currentState = State.Jumping;
            m_pressTime = pressTime;
            m_signalBus.Fire(new Event.InGameEvent.OnStartJump(isFever, pressTime));

            SoundEffectController.Instance.Stop(m_holdClip);
            SoundEffectController.Instance.Play(m_jumpClip);
        }

        private void OnJumpEnded()
        {
            m_currentDirectionBlock.DisableHint();

            SetIdle();
            bool isScuess = m_currentDirectionBlock.IsOnBlock(transform.position, m_jumperSetting.GameOverDistance);
            bool isPerfect = m_currentDirectionBlock.IsOnBlockPerfect(transform.position, m_jumperSetting.ComboHitAdjust);

            if (isScuess)
            {
                if (!IsTutorialEnded() && !isPerfect)
                {
                    m_hintView.EnableReleaseHint(false);
                    m_hintView.EnableStartHint(false);
                    m_hintView.EnableTutorialHint(true);
                }

                CreateVFX(m_landingVfx, Vector3.up, m_pressTime);
                m_landingFeedback.PlayFeedbacks();
                m_signalBus.Fire(new Event.InGameEvent.OnJumpEnded(transform.position, true, isPerfect, m_remainingLife));
            }
            else
            {
                SetFall();
            }

            if (isScuess)
            {
                if (isPerfect)
                {
                    SoundEffectController.Instance.Play(m_perfectLandClip);
                }
                else
                {
                    SoundEffectController.Instance.Play(m_landClip);
                }
            }
        }

        private void Revive()
        {
            m_currentState = State.Jumping;
            m_rigidbody.isKinematic = true;
            m_collider.isTrigger = true;
            m_signalBus.Fire<Event.InGameEvent.OnStartRevive>();
            KahaGameCore.Common.TimerManager.Schedule(Time.deltaTime * 2f, StartReviveJump);
        }

        private void StartReviveJump()
        {
            m_rigidbody.transform.localPosition = Vector3.zero + Vector3.up * 2f;
            m_rigidbody.transform.localRotation = Quaternion.identity;
            transform.DOJump(m_currentDirectionBlock.transform.position, m_gameProperties.JUMP_FORCE, 1, 0.5f).SetEase(Ease.Linear).OnComplete(OnRevived);
        }

        private void OnRevived()
        {
            if (!IsTutorialEnded())
            {
                m_hintView.EnableReleaseHint(false);
                m_hintView.EnableStartHint(false);
                m_hintView.EnableTutorialHint(false);
                m_hintView.EnableLandHint(true);
            }

            m_currentState = State.Idle;

            m_signalBus.Fire(new Event.InGameEvent.OnJumpEnded(transform.position, true, false, m_remainingLife));
        }

        private void Update()
        {
            if (m_currentState == State.FeverPressing)
            {
                UpdatePressDownScale();
            }

            if (m_currentState == State.Pressing)
            {
                if (m_currentDirectionBlock == null)
                {
                    m_hintObject.transform.position += Vector3.forward * m_gameProperties.MOVE_DIS_PER_SEC * Time.deltaTime;
                    UpdatePressDownScale();
                }
                else
                {
                    if (!IsTutorialEnded())
                    {
                        m_hintView.EnableReleaseHint(true);

                        m_hintObject.transform.position += (m_currentDirectionBlock.transform.position - transform.position).normalized * m_gameProperties.MOVE_DIS_PER_SEC * Time.deltaTime;
                        UpdatePressDownScale();

                        if (m_tutorialMode_feverTimes == 1)
                        {
                            for (int i = 0; i < m_hintImages.Length; i++)
                            {
                                m_hintImages[i].color = new Color(m_hintImages[i].color.r, m_hintImages[i].color.g, m_hintImages[i].color.b, m_hintImages[i].color.a - Time.deltaTime * 3f);
                            }
                        }
                    }
                    else
                    {
                        m_hintObject.transform.position += (m_currentDirectionBlock.transform.position - transform.position).normalized * m_gameProperties.MOVE_DIS_PER_SEC * Time.deltaTime;
                        UpdatePressDownScale();
                    }
                }

                if (!Input.GetMouseButton(0))
                {
                    SetIdle();
                }
            }

            if (m_currentState == State.Idle && m_lastBlock != null && !m_lastBlock.IsOnBlock(transform.position, m_jumperSetting.GameOverDistance))
            {
                SetFall();
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                SetIdle();
            }
        }

        private void UpdatePressDownScale()
        {
            if (m_rigidbody.transform.localScale.y >= 0.25f)
            {
                m_rigidbody.transform.localScale += new Vector3(1f, -1f, 1f) * Time.deltaTime;
                m_rigidbody.transform.localPosition -= Vector3.up * Time.deltaTime;
            }
        }

        private void CreateVFX(ParticleSystem vfx, Vector3 offset, float scale)
        {
            ParticleSystem _cloneVfx = Instantiate(vfx);
            _cloneVfx.transform.SetParent(null);
            _cloneVfx.transform.position = transform.position + offset;
            _cloneVfx.transform.localScale = Vector3.one * scale;
            _cloneVfx.Play();
            Destroy(_cloneVfx.gameObject, 2f);
        }

        private void SetIdle()
        {
            m_currentState = State.Idle;
            m_rigidbody.transform.localPosition = Vector3.zero + Vector3.up * 2f;
            m_rigidbody.transform.localRotation = Quaternion.identity;
            m_rigidbody.transform.localScale = Vector3.one;
        }

        private bool IsTutorialEnded()
        {
            return m_tutorialMode_feverTimes >= 1;
        }

        private void PlayAnimation(string name)
        {
            if (m_animators == null)
                return;

            for (int i = 0; i < m_animators.Length; i++)
            {
                m_animators[i].Play(name);
            }
        }

        private void SetFall()
        {
            m_remainingLife--;

            m_currentState = State.Died;
            m_rigidbody.isKinematic = false;
            m_collider.isTrigger = false;
            m_rigidbody.AddForce(new Vector3(0f, -1000f, 0f));

            if (m_remainingLife <= 0)
            {
                m_signalBus.Fire(new Event.InGameEvent.OnJumpEnded(transform.position, false, false, m_remainingLife));
            }
            else
            {
                KahaGameCore.Common.TimerManager.Schedule(1f, Revive);
            }
        }
    }
}