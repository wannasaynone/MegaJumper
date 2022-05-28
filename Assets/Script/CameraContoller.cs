using System;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace MegaJumper
{
    public class CameraContoller : MonoBehaviour
    {
        [SerializeField] private Transform m_camRoot;

        private GameProperties m_gameProperties;

        private float m_pressTime;

        [Inject]
        public void Constructor(SignalBus signalBus, GameProperties gameProperties)
        {
            m_gameProperties = gameProperties;
            signalBus.Subscribe<Event.InGameEvent.OnPointUp>(OnPointUp);
            signalBus.Subscribe<Event.InGameEvent.OnStartJump>(OnJumpStart);
            signalBus.Subscribe<Event.InGameEvent.OnJumpEnded>(OnJumpEnded);
            signalBus.Subscribe<Event.InGameEvent.OnGameResetCalled>(OnGameResetCalled);

            transform.eulerAngles = m_gameProperties.CAMERA_ANGLE_OFFSET;
        }

        private void OnPointUp(Event.InGameEvent.OnPointUp obj)
        {
            m_pressTime = obj.PressTime;
        }

        private void OnJumpStart()
        {
            transform.DOMove(transform.position + Vector3.up * m_pressTime * 2f, 0.3f);
        }

        private void OnGameResetCalled()
        {
            transform.position = m_gameProperties.CAMERA_OFFSET;
        }

        private Vector3 m_currentJumperPos;
        private void OnJumpEnded(Event.InGameEvent.OnJumpEnded obj)
        {
            m_currentJumperPos = obj.Position;
            m_camRoot.DOShakePosition(m_gameProperties.CAMERA_SHAKE_TIME * m_pressTime, new Vector3(0f, m_pressTime * m_gameProperties.CAMERA_SHAKE_FORCE, 0f), 10, 90, false , false).OnComplete(MoveCameraToCurrentJumperPostion);
        }

        private void MoveCameraToCurrentJumperPostion()
        {
            transform.DOMove(m_currentJumperPos + m_gameProperties.CAMERA_OFFSET, 0.5f);
        }
    }
}