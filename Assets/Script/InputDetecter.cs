using Zenject;
using UnityEngine;

namespace MegaJumper
{
    public class InputDetecter : MonoBehaviour
    {
        private SignalBus m_signalBus;
        private UnityEngine.EventSystems.EventSystem m_eventSystem;
        private GameProperties m_gameProperties;

        private float m_timer;
        private bool m_isOnUI;

        [Inject]
        public void Constructor(SignalBus signalBus, UnityEngine.EventSystems.EventSystem eventSystem, GameProperties gameProperties)
        {
            m_signalBus = signalBus;
            m_eventSystem = eventSystem;
            m_gameProperties = gameProperties;
        }

        private void Update()
        {
            DetectMouseInput();
        }

        private void DetectMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                m_isOnUI = IsPointerOverUIObject();
                if (!m_isOnUI)
                {
                    m_timer = 0f;
                    m_signalBus.Fire(new Event.InGameEvent.OnPointDown());
                }
            }

            if (Input.GetMouseButton(0) && !m_isOnUI)
            {
                m_timer += Time.deltaTime;
                if (m_timer >= m_gameProperties.MAX_PRESS_TIME)
                {
                    m_timer = m_gameProperties.MAX_PRESS_TIME;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (m_isOnUI)
                {
                    m_isOnUI = false;
                }
                else
                {
                    m_signalBus.Fire(new Event.InGameEvent.OnPointUp(m_timer));
                }
            }
        }

        private bool IsPointerOverUIObject()
        {
            UnityEngine.EventSystems.PointerEventData eventDataCurrentPosition = new UnityEngine.EventSystems.PointerEventData(m_eventSystem)
            {
                position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
            };
            System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult> results = new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();
            m_eventSystem.RaycastAll(eventDataCurrentPosition, results);

            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}