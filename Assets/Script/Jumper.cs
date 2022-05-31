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

        private int m_tutorialMode_feverTimes = 2;

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
            m_tutorialMode_feverTimes = 2;
        }

        private void OnStartFever()
        {
            if (m_tutorialMode_feverTimes < 2)
            {
                m_tutorialMode_feverTimes++;
                if (m_tutorialMode_feverTimes >= 2)
                {
                    Debug.Log("OnTutorialEnded");
                    m_signalBus.Fire<Event.InGameEvent.OnTutorialEnded>();
                }
            }

            m_currentState = State.FeverPressing;
            m_jumperModel.transform.DOShakePosition(m_gameProperties.FEVER_ANIMATION_TIME + 0.5f).OnComplete(OnFeverShakeEnded);
        }

        private void OnFeverShakeEnded()
        {
            m_rigidbody.transform.localScale = Vector3.one;
            m_rigidbody.transform.localPosition = Vector3.zero + Vector3.up * 2f;
            StartJump(m_currentDirectionBlock.transform.position, m_gameProperties.FEVER_JUMP_FORCE, true, OnFeverJumpEnded);
        }

        private void OnFeverJumpEnded()
        {
            m_currentState = State.Idle;
            CreateVFX(m_landingVfx, Vector3.up, m_pressTime);
            m_landingFeedback.PlayFeedbacks();
            m_signalBus.Fire(new Event.InGameEvent.OnJumpEnded(transform.position, true, false));
            m_signalBus.Fire<Event.InGameEvent.OnFeverEnded>();
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
            m_jumperModel.transform.localScale = Vector3.one;
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
            m_hintView.EnableControlHint(false);
            m_hintView.EnableTutorialHint(false);
            m_hintObject.SetActive(m_tutorialMode_feverTimes < 2);
            for (int i = 0; i < m_hintImages.Length; i++)
            {
                m_hintImages[i].color = new Color(m_hintImages[i].color.r, m_hintImages[i].color.g, m_hintImages[i].color.b, 1f);
            }
        }

        private void OnGameResetCalled()
        {
            transform.position = Vector3.zero;
            m_rigidbody.isKinematic = true;
            m_collider.isTrigger = true;
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

            if (m_tutorialMode_feverTimes < 2 && Vector3.Distance(m_hintObject.transform.position - Vector3.up, m_currentDirectionBlock.transform.position) > m_gameProperties.GAMEOVER_DIS)
            {
                SetIdle();
                m_hintView.EnableTutorialHint(false);
                m_hintView.EnableControlHint(true);
                return;
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
                if (Vector3.Distance(_finalPos, m_lastBlock.transform.position) <= m_gameProperties.GAMEOVER_DIS)
                {
                    SetIdle();
                    return;
                }
            }
            else
            {

                if (Vector3.Distance(_finalPos, transform.position) <= m_gameProperties.GAMEOVER_DIS)
                {
                    SetIdle();
                    return;
                }
            }

            float _adjust = m_jumperSetting.ComboHitAdjust;
            if (m_tutorialMode_feverTimes < 2) _adjust += 0.3f;

            if (Vector3.Distance(_finalPos, m_currentDirectionBlock.transform.position) <= _adjust)
            {
                _finalPos = m_currentDirectionBlock.transform.position;
            }

            StartJump(_finalPos, obj.PressTime, false, OnJumpEnded);
        }

        private void StartJump(Vector3 endPos, float pressTime, bool isFever, TweenCallback onEnded)
        {
            CreateVFX(m_jumpVfx, Vector3.up, pressTime);

            transform.DOJump(endPos, pressTime * m_gameProperties.JUMP_FORCE, 1, pressTime * m_gameProperties.JUMP_TIME_SCALE).SetEase(Ease.Linear).OnComplete(onEnded);

            m_currentState = State.Jumping;
            m_pressTime = pressTime;
            m_signalBus.Fire(new Event.InGameEvent.OnStartJump(isFever));
        }

        private void OnJumpEnded()
        {
            SetIdle();
            bool isScuess = Vector3.Distance(m_currentDirectionBlock.transform.position, transform.position) < m_gameProperties.GAMEOVER_DIS * m_currentDirectionBlock.SizeScale;
            bool isPerfect = Vector3.Distance(m_currentDirectionBlock.transform.position, transform.position) <= m_jumperSetting.ComboHitAdjust;

            if (isScuess)
            {
                if (m_tutorialMode_feverTimes < 2 && !isPerfect)
                {
                    m_hintView.EnableReleaseHint(false);
                    m_hintView.EnableControlHint(false);
                    m_hintView.EnableTutorialHint(true);
                }

                CreateVFX(m_landingVfx, Vector3.up, m_pressTime);
                m_landingFeedback.PlayFeedbacks();
                m_signalBus.Fire(new Event.InGameEvent.OnJumpEnded(transform.position, true, isPerfect));
            }
            else
            {
                m_remainingLife--;

                m_currentState = State.Died;
                m_rigidbody.isKinematic = false;
                m_collider.isTrigger = false;
                m_rigidbody.AddForce(new Vector3(0f, -1000f, 0f));

                if (m_remainingLife <= 0)
                {
                    m_signalBus.Fire(new Event.InGameEvent.OnJumpEnded(transform.position, false, false));
                }
                else
                {
                    KahaGameCore.Common.TimerManager.Schedule(1f, Revive);
                }
            }
        }

        private void Revive()
        {
            m_currentState = State.Jumping;
            m_rigidbody.transform.localPosition = Vector3.zero + Vector3.up * 2f;
            m_rigidbody.transform.localRotation = Quaternion.identity;
            m_rigidbody.isKinematic = true;
            m_collider.isTrigger = true;
            transform.DOJump(m_currentDirectionBlock.transform.position, m_gameProperties.JUMP_FORCE, 1, 0.5f).SetEase(Ease.Linear).OnComplete(OnRevived);
        }

        private void OnRevived()
        {
            m_currentState = State.Idle;
            m_signalBus.Fire(new Event.InGameEvent.OnJumpEnded(transform.position, true, false));
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
                    if (m_tutorialMode_feverTimes < 2)
                    {
                        m_hintView.EnableReleaseHint(true);

                        //if (Vector3.Distance(m_hintObject.transform.position - Vector3.up, m_currentDirectionBlock.transform.position) <= m_jumperSetting.ComboHitAdjust)
                        //{
                        //    m_hintObject.transform.position = m_currentDirectionBlock.transform.position + Vector3.up;
                        //}
                        //else
                        //{
                            m_hintObject.transform.position += (m_currentDirectionBlock.transform.position - transform.position).normalized * m_gameProperties.MOVE_DIS_PER_SEC * Time.deltaTime;
                            UpdatePressDownScale();
                        //}

                        if (m_tutorialMode_feverTimes == 1)
                        {
                            for (int i = 0; i < m_hintImages.Length; i++)
                            {
                                m_hintImages[i].color = new Color(m_hintImages[i].color.r, m_hintImages[i].color.g, m_hintImages[i].color.b, m_hintImages[i].color.a - Time.deltaTime * 2f);
                            }
                        }
                    }
                    else
                    {
                        m_hintObject.transform.position += (m_currentDirectionBlock.transform.position - transform.position).normalized * m_gameProperties.MOVE_DIS_PER_SEC * Time.deltaTime;
                        UpdatePressDownScale();
                    }
                }
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
        }
    }
}