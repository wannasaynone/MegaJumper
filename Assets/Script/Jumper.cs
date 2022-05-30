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

        private GameProperties m_gameProperties;
        private SignalBus m_signalBus;

        private bool m_isPause = true;
        private bool m_isJumping = false;
        private bool m_skipNextUp = false;
        private bool m_pressing = false;
        private Block m_currentDirectionBlock;

        private JumperSetting m_jumperSetting;
        private int m_remainingLife;
        private GameObject m_jumperModel;

        [Inject]
        public void Constructor(SignalBus signalBus, GameProperties gameProperties)
        {
            m_signalBus = signalBus;
            m_gameProperties = gameProperties;

            signalBus.Subscribe<Event.InGameEvent.OnGameStarted>(OnGameStart);
            signalBus.Subscribe<Event.InGameEvent.OnPointDown>(OnPointDown);
            signalBus.Subscribe<Event.InGameEvent.OnPointUp>(OnPointUp);
            signalBus.Subscribe<Event.InGameEvent.OnGameResetCalled>(OnGameResetCalled);
            signalBus.Subscribe<Event.InGameEvent.OnBlockSpawned>(OnBlockSpawned);
            signalBus.Subscribe<Event.InGameEvent.OnJumperSettingSet>(OnJumperSettingSet);
            signalBus.Subscribe<Event.InGameEvent.OnStartFever>(OnStartFever);

            m_hintObject.transform.SetParent(null);
        }

        private void OnStartFever()
        {
            m_isPause = true;
            m_pressing = true;
            m_jumperModel.transform.DOShakePosition(m_gameProperties.FEVER_ANIMATION_TIME + 0.5f).OnComplete(OnFeverShakeEnded);
        }

        private void OnFeverShakeEnded()
        {
            StartJump(m_currentDirectionBlock.transform.position, m_gameProperties.FEVER_JUMP_FORCE, true, OnFeverJumpEnded);
        }

        private void OnFeverJumpEnded()
        {
            m_isJumping = false;
            m_isPause = false;
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
            m_currentDirectionBlock = obj.Block;
            Vector3 _cur = transform.eulerAngles;
            transform.LookAt(m_currentDirectionBlock.transform.position);
            Vector3 _new = transform.eulerAngles;
            transform.eulerAngles = _cur;
            transform.DORotate(_new, 0.2f);
        }

        private void OnPointDown()
        {
            if (m_isPause)
            {
                return;
            }

            if (m_isJumping)
            {
                m_skipNextUp = true;
                return;
            }

            m_pressing = true;
            m_hintObject.transform.position = transform.position + Vector3.up;
        }

        private void OnGameResetCalled()
        {
            transform.position = Vector3.zero;
            m_rigidbody.transform.localPosition = Vector3.zero + Vector3.up * 2f;
            m_rigidbody.transform.localRotation = Quaternion.identity;
            m_rigidbody.isKinematic = true;
            m_collider.isTrigger = true;
            SetSetting(m_jumperSetting);
        }

        private void OnGameStart()
        {
            m_isPause = false;
            OnPointDown();
        }

        private float m_pressTime;
        private void OnPointUp(Event.InGameEvent.OnPointUp obj)
        {
            if (m_isPause)
            {
                return;
            }

            if (m_skipNextUp)
            {
                m_skipNextUp = false;
                return;
            }

            Vector3 _finalPos = transform.position;

            if (m_currentDirectionBlock == null)
            {
                _finalPos += Vector3.forward * obj.PressTime * m_gameProperties.MOVE_DIS_PER_SEC;
            }
            else
            {
                _finalPos += (m_currentDirectionBlock.transform.position - transform.position).normalized * obj.PressTime * m_gameProperties.MOVE_DIS_PER_SEC;
            }

            if (Vector3.Distance(_finalPos, m_currentDirectionBlock.transform.position) <= m_jumperSetting.ComboHitAdjust)
            {
                _finalPos = m_currentDirectionBlock.transform.position;
            }

            StartJump(_finalPos, obj.PressTime, false, OnJumpEnded);
        }

        private void StartJump(Vector3 endPos, float pressTime, bool isFever, TweenCallback onEnded)
        {
            m_rigidbody.transform.localScale = Vector3.one;
            m_rigidbody.transform.localPosition = Vector3.zero + Vector3.up * 2f;
            CreateVFX(m_jumpVfx, Vector3.up, pressTime);

            transform.DOJump(endPos, pressTime * m_gameProperties.JUMP_FORCE, 1, pressTime * m_gameProperties.JUMP_TIME_SCALE).SetEase(Ease.Linear).OnComplete(onEnded);

            m_pressing = false;
            m_isJumping = true;
            m_pressTime = pressTime;
            m_signalBus.Fire(new Event.InGameEvent.OnStartJump(isFever));
        }

        private void OnJumpEnded()
        {
            m_isJumping = false;
            bool isScuess = Vector3.Distance(m_currentDirectionBlock.transform.position, transform.position) < m_gameProperties.GAMEOVER_DIS * m_currentDirectionBlock.SizeScale;
            bool isPerfect = Vector3.Distance(m_currentDirectionBlock.transform.position, transform.position) <= m_jumperSetting.ComboHitAdjust;

            if (isScuess)
            {
                CreateVFX(m_landingVfx, Vector3.up, m_pressTime);
                m_landingFeedback.PlayFeedbacks();
                m_signalBus.Fire(new Event.InGameEvent.OnJumpEnded(transform.position, true, isPerfect));
            }
            else
            {
                m_remainingLife--;

                m_isPause = true;
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
            m_isJumping = true;
            m_rigidbody.transform.localPosition = Vector3.zero + Vector3.up * 2f;
            m_rigidbody.transform.localRotation = Quaternion.identity;
            m_rigidbody.isKinematic = true;
            m_collider.isTrigger = true;
            transform.DOJump(m_currentDirectionBlock.transform.position, m_gameProperties.JUMP_FORCE, 1, 0.5f).SetEase(Ease.Linear).OnComplete(OnRevived);
        }

        private void OnRevived()
        {
            m_isJumping = false;
            m_isPause = false;
            m_signalBus.Fire(new Event.InGameEvent.OnJumpEnded(transform.position, true, false));
        }

        private void Update()
        {
            if (m_pressing)
            {
                if (m_currentDirectionBlock == null)
                {
                    m_hintObject.transform.position += Vector3.forward * m_gameProperties.MOVE_DIS_PER_SEC * Time.deltaTime;
                }
                else
                {
                    m_hintObject.transform.position += (m_currentDirectionBlock.transform.position - transform.position).normalized * m_gameProperties.MOVE_DIS_PER_SEC * Time.deltaTime;
                }

                if (m_rigidbody.transform.localScale.y >= 0.25f)
                {
                    m_rigidbody.transform.localScale += new Vector3(1f, -1f, 1f) * Time.deltaTime;
                    m_rigidbody.transform.localPosition -= Vector3.up * Time.deltaTime;
                }
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
    }
}