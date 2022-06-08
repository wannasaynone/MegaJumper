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
            if (m_eventSystem.IsPointerOverGameObject())
            {
                return;
            }

            foreach (UnityEngine.Touch touch in UnityEngine.Input.touches)
            {
                int id = touch.fingerId;
                if (m_eventSystem.IsPointerOverGameObject(id))
                {
                    return;
                }
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
    }
}