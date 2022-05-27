using System;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace MegaJumper
{
    public class Jumper : MonoBehaviour
    {
        [SerializeField] private Rigidbody m_rigidbody;
        [SerializeField] private Collider m_collider;
        [SerializeField] private GameObject m_debuger;

        private GameProperties m_gameProperties;
        private SignalBus m_signalBus;

        private bool m_stop = true;
        private bool m_pressing = false;
        private Block m_currentDirectionBlock;

        [Inject]
        public void Constructor(SignalBus signalBus, GameProperties gameProperties)
        {
            m_signalBus = signalBus;
            m_gameProperties = gameProperties;

            signalBus.Subscribe<Event.InGameEvent.OnGameStarted>(OnGameStart);
            signalBus.Subscribe<Event.InGameEvent.OnJumpFailDetected>(OnJumpFailDetected);
            signalBus.Subscribe<Event.InGameEvent.OnPointDown>(OnPointDown);
            signalBus.Subscribe<Event.InGameEvent.OnPointUp>(OnPointUp);
            signalBus.Subscribe<Event.InGameEvent.OnGameResetCalled>(OnGameResetCalled);
            signalBus.Subscribe<Event.InGameEvent.OnBlockSpawned>(OnBlockSpawned);

            m_debuger.transform.SetParent(null);
        }

        private void OnBlockSpawned(Event.InGameEvent.OnBlockSpawned obj)
        {
            m_currentDirectionBlock = obj.Block;
        }

        private void OnPointDown()
        {
            if (m_stop)
                return;

            m_pressing = true;
            m_debuger.transform.position = transform.position + Vector3.up;
        }

        private void OnGameResetCalled()
        {
            transform.position = Vector3.zero;
            m_rigidbody.transform.localPosition = Vector3.zero + Vector3.up * 2f;
            m_rigidbody.transform.localRotation = Quaternion.identity;
            m_rigidbody.isKinematic = true;
            m_collider.isTrigger = true;
        }

        private void OnJumpFailDetected()
        {
            m_stop = true;
            m_rigidbody.isKinematic = false;
            m_collider.isTrigger = false;
        }

        private void OnGameStart()
        {
            m_stop = false;
            OnPointDown();
        }

        private void OnPointUp(Event.InGameEvent.OnPointUp obj)
        {
            if (m_stop)
                return;

            Vector3 _finalPos = transform.position;

            if (m_currentDirectionBlock == null)
            {
                _finalPos += Vector3.forward * obj.PressTime * m_gameProperties.MOVE_DIS_PER_SEC;
            }
            else
            {
                _finalPos += (m_currentDirectionBlock.transform.position - transform.position).normalized * obj.PressTime * m_gameProperties.MOVE_DIS_PER_SEC;
            }

            m_rigidbody.transform.localScale = Vector3.one;
            m_rigidbody.transform.localPosition = Vector3.zero + Vector3.up * 2f;

            transform.DOJump(_finalPos, obj.PressTime * m_gameProperties.JUMP_FORCE, 1, obj.PressTime * m_gameProperties.JUMP_TIME_SCALE).SetEase(Ease.Linear).OnComplete(OnJumpEnded);

            m_pressing = false;
        }

        private void OnJumpEnded()
        {
            m_signalBus.Fire(new Event.InGameEvent.OnJumpEnded(transform.position));
        }

        private void Update()
        {
            if (m_pressing)
            {
                if (m_currentDirectionBlock == null)
                {
                    m_debuger.transform.position += Vector3.forward * m_gameProperties.MOVE_DIS_PER_SEC * Time.deltaTime;
                }
                else
                {
                    m_debuger.transform.position += (m_currentDirectionBlock.transform.position - transform.position).normalized * m_gameProperties.MOVE_DIS_PER_SEC * Time.deltaTime;
                }

                if (m_rigidbody.transform.localScale.y >= 0.25f)
                {
                    m_rigidbody.transform.localScale += new Vector3(1f, -1f, 1f) * Time.deltaTime;
                    m_rigidbody.transform.localPosition -= Vector3.up * Time.deltaTime;
                }
            }
        }
    }
}