using Zenject;

namespace MegaJumper
{
    public class InputDetecter : ITickable
    {
        private readonly SignalBus m_signalBus;
        private float m_timer;

        public InputDetecter(SignalBus signalBus)
        {
            m_signalBus = signalBus;
        }

        public void Tick()
        {
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