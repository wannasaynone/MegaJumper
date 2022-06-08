using Zenject;

namespace MegaJumper
{
    public class InputDetecter : ITickable
    {
        private readonly SignalBus m_signalBus;
        private readonly UnityEngine.EventSystems.EventSystem m_eventSystem;

        private float m_timer;

        public InputDetecter(SignalBus signalBus, UnityEngine.EventSystems.EventSystem eventSystem)
        {
            m_signalBus = signalBus;
            m_eventSystem = eventSystem;
        }

        public void Tick()
        {
            DetectMouseInput();
        }

        private void DetectMouseInput()
        {
            if (IsPointerOverUIObject())
            {
                return;
            }

            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                m_timer = 0f;
                m_signalBus.Fire(new Event.InGameEvent.OnPointDown());
            }

            if (UnityEngine.Input.GetMouseButton(0))
            {
                m_timer += UnityEngine.Time.deltaTime;
            }

            if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                m_signalBus.Fire(new Event.InGameEvent.OnPointUp(m_timer));
            }
        }

        private bool IsPointerOverUIObject()
        {
            UnityEngine.EventSystems.PointerEventData eventDataCurrentPosition = new UnityEngine.EventSystems.PointerEventData(m_eventSystem);
            eventDataCurrentPosition.position = new UnityEngine.Vector2(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y);
            System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult> results = new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();
            m_eventSystem.RaycastAll(eventDataCurrentPosition, results);

            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].gameObject.layer != UnityEngine.LayerMask.NameToLayer("Ignore Raycast"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}