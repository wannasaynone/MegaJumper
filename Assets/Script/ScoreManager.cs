using Zenject;

namespace MegaJumper
{
    public class ScoreManager
    {
        public int Score { get; private set; }

        private readonly SignalBus m_signalBus;

        public ScoreManager(SignalBus signalBus)
        {
            m_signalBus = signalBus;
        }

        public void Add(int add)
        {
            long _curLong = Score;

            if (_curLong + add > int.MaxValue)
            {
                Score = int.MaxValue;
            }
            else if (_curLong + add < 0)
            {
                Score = 0;
            }
            else
            {
                Score += add;
            }

            m_signalBus.Fire(new Event.InGameEvent.OnScoreAdded(add, Score));
        }

        public void Reset()
        {
            Score = 0;
            m_signalBus.Fire<Event.InGameEvent.OnScoreReset>();
        }
    }
}
