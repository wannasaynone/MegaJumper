using DG.Tweening;
using UnityEngine;
using Zenject;

namespace MegaJumper
{
    public class CameraContoller : MonoBehaviour
    {
        private GameProperties m_gameProperties;

        [Inject]
        public void Constructor(SignalBus signalBus, GameProperties gameProperties)
        {
            m_gameProperties = gameProperties;
            signalBus.Subscribe<Event.InGameEvent.OnJumpEnded>(OnJumpEnded);
            signalBus.Subscribe<Event.InGameEvent.OnGameResetCalled>(OnGameResetCalled);

            transform.eulerAngles = m_gameProperties.CAMERA_ANGLE_OFFSET;
        }

        private void OnGameResetCalled()
        {
            transform.position = m_gameProperties.CAMERA_OFFSET;
        }

        private void OnJumpEnded(Event.InGameEvent.OnJumpEnded obj)
        {
            transform.DOMove(obj.Position + m_gameProperties.CAMERA_OFFSET, 0.5f);
        }
    }
}