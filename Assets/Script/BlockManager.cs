using System.Collections.Generic;
using Zenject;

namespace MegaJumper
{
    public class BlockManager
    {
        private bool m_currentDirectionToZ = true;

        private readonly SignalBus m_signalBus;
        private readonly Block.Factory m_blockFactory;
        private readonly GameProperties m_gameProperties;
        private readonly ScoreManager m_scoreManager;

        private List<Block> m_clonedBlock = new List<Block>();
        private float m_currentChangeDirectionChance = 0f;

        private bool m_tutorialMode = false;
        private bool m_feverMode = false;
        private bool m_showHintNext = false;

        private int m_index;

        public BlockManager(Block.Factory factory, ScoreManager scoreManager, GameProperties gameProperties, SignalBus signalBus)
        {
            m_blockFactory = factory;
            m_gameProperties = gameProperties;
            m_signalBus = signalBus;
            m_scoreManager = scoreManager;

            signalBus.Subscribe<Event.InGameEvent.OnGameResetCalled>(OnGameResetCalled);
            signalBus.Subscribe<Event.InGameEvent.OnTutorialStart>(OnTutorialStart);
            signalBus.Subscribe<Event.InGameEvent.OnTutorialEnded>(OnTutorialEnded);
            signalBus.Subscribe<Event.InGameEvent.OnStartFever>(OnStartFever);
            signalBus.Subscribe<Event.InGameEvent.OnFeverEnded>(OnFeverEnded);
        }

        private void OnTutorialStart()
        {
            m_tutorialMode = true;
        }

        private void OnTutorialEnded()
        {
            m_tutorialMode = false;
        }

        private void OnStartFever()
        {
            m_feverMode = true;
        }

        private void OnFeverEnded()
        {
            m_feverMode = false;
        }

        private void OnGameResetCalled(Event.InGameEvent.OnGameResetCalled obj)
        {
            m_currentChangeDirectionChance = 0f;
            m_currentDirectionToZ = true;

            for (int i = 0; i < m_clonedBlock.Count; i++)
            {
                UnityEngine.Object.Destroy(m_clonedBlock[i].gameObject);
            }
            m_clonedBlock.Clear();

            m_showHintNext = obj.StartWithFever;
        }

        public void CreateNew()
        {
            UnityEngine.Vector3 _orginPos = UnityEngine.Vector3.zero;

            if (m_clonedBlock.Count > 0)
            {
                _orginPos = m_clonedBlock[m_clonedBlock.Count - 1].transform.position;
            }

            UnityEngine.Vector3 _newPos = _orginPos;
            float _maxDistanceMutiplier = (float)(m_scoreManager.Score - 10) / 10f;

            if (m_tutorialMode)
            {
                _maxDistanceMutiplier = 0f;
            }
            else
            {
                _maxDistanceMutiplier = UnityEngine.Mathf.Clamp(_maxDistanceMutiplier, 0f, 1f);
            }

            float _maxDistance = m_gameProperties.MIN_ADD_DISTANCE + (m_gameProperties.MAX_ADD_DISTANCE - m_gameProperties.MIN_ADD_DISTANCE) * _maxDistanceMutiplier;
            float _randomValue = UnityEngine.Random.Range(m_gameProperties.MIN_ADD_DISTANCE, _maxDistance);

            if (m_tutorialMode)
            {
                _randomValue = m_gameProperties.MIN_ADD_DISTANCE;
            }

            if (m_currentDirectionToZ)
            {
                _newPos += UnityEngine.Vector3.forward * _randomValue;
            }
            else
            {
                _newPos += UnityEngine.Vector3.left * _randomValue;
            }

            Block _clone = m_blockFactory.Create();
            _clone.transform.position = _newPos;
            _clone.name += "_" + m_index;
            _clone.currentDirectionToZ = m_currentDirectionToZ;
            m_clonedBlock.Add(_clone);

            if (UnityEngine.Random.Range(0f, 100f) <= m_currentChangeDirectionChance)
            {
                m_currentDirectionToZ = !m_currentDirectionToZ;
                m_currentChangeDirectionChance = 0f;
            }
            else
            {
                m_currentChangeDirectionChance += 10f;
            }

            if (!m_tutorialMode && m_scoreManager.Score >= 10)
            {
                float _min = 10f / (float)m_scoreManager.Score;
                if (_min < 0.5f) _min = 0.5f;

                float _max = 20f / (float)m_scoreManager.Score;
                if (_max > 1f) _max = 1f;
                if (_max < _min) _max = _min + 0.1f;

                _clone.RerollSize(_min, _max);
            }

            if (!m_feverMode && !m_tutorialMode && m_scoreManager.Score >= 10)
            {
                Block.BlockType _randomType = _clone.GetRandomType();
                _clone.SetType(_randomType);
                KahaGameCore.Common.TimerManager.Schedule(0.3f, _clone.StartTickBlockType);
            }

            if (m_showHintNext)
            {
                m_showHintNext = false;
                _clone.ShowHint();
            }

            if (m_clonedBlock.Count >= m_gameProperties.MAX_CLONE_COUNT)
            {
                UnityEngine.Object.Destroy(m_clonedBlock[0].gameObject);
                m_clonedBlock.RemoveAt(0);
            }

            _clone.PlayFeedback();

            m_index++;

            m_signalBus.Fire(new Event.InGameEvent.OnBlockSpawned(_clone));
        }

        public UnityEngine.Vector3 GetLastBlockPosition()
        {
            if (m_clonedBlock.Count <= 0)
            {
                return UnityEngine.Vector3.zero;
            }

            return m_clonedBlock[m_clonedBlock.Count - 1].transform.position;
        }
    }
}