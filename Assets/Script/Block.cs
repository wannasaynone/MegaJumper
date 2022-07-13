using UnityEngine;
using DG.Tweening;
using Zenject;

namespace MegaJumper
{
    public class Block : MonoBehaviour
    {
        public enum BlockType
        {
            None,
            Bouns,
            MoveRepeat,
            SlowDisappear
        }

        [SerializeField] private GameObject[] m_blockModels;
        [SerializeField] private GameObject m_startFeverHint;
        [SerializeField] private GameObject m_gray;
        [SerializeField] private GameObject m_brown;
        [SerializeField] private GameObject m_white;

        public class Factory : PlaceholderFactory<Block>
        {

        }

        [Inject]
        public void Constructor(SignalBus signalBus)
        {
            signalBus.Subscribe<Event.InGameEvent.OnStartRevive>(OnStartRevive);
        }

        private GameObject m_cloneStage;
        private Tweener m_tweener;

        private float m_sizeScale = 1f;

        private System.Collections.Generic.List<int> m_targetModelIndexs = new System.Collections.Generic.List<int>();
        public BlockType CurrentBlockType { get; private set; } = BlockType.None;

        public bool currentDirectionToZ;

        private bool m_randomReverse;
        private float m_randomSpeed;

        public bool IsOnBlock(Vector3 pos, float gameOverDistance)
        {
            if (m_cloneStage != null)
            {
                return Vector3.Distance(m_cloneStage.transform.position, pos) < gameOverDistance * m_sizeScale;
            }

            return Vector3.Distance(m_blockModels[m_targetModelIndexs[0]].transform.position, pos) < gameOverDistance * m_sizeScale;
        }

        public bool IsOnBlockPerfect(Vector3 pos, float adjust)
        {
            if (m_cloneStage != null)
            {
                return Vector3.Distance(m_cloneStage.transform.position, pos) <= adjust;
            }
            return Vector3.Distance(m_blockModels[m_targetModelIndexs[0]].transform.position, pos) <= adjust;
        }

        private void OnEnable()
        {
            int _ran = Random.Range(1, 3);

            for (int i = 0; i < _ran; i++)
            {
                int _ranIndex = Random.Range(0, m_blockModels.Length - 1);
                m_targetModelIndexs.Add(_ranIndex);

                m_blockModels[_ranIndex].SetActive(true);
            }
        }

        public void ShowHint()
        {
            m_startFeverHint.SetActive(true);
        }

        public void DisableHint()
        {
            m_startFeverHint.SetActive(false);
        }

        public void RerollSize(float min = 0.5f, float max = 1f)
        {
            if (min < 0.5f)
            {
                min = 0.5f;
            }

            if (min > 1f)
            {
                min = 1f;
            }

            if (max > 1f)
            {
                max = 1f;
            }

            if (max < 0.5f)
            {
                max = 0.5f;
            }

            m_sizeScale = Random.Range(min, max);
        }

        public BlockType GetRandomType()
        {
            return (BlockType)Random.Range(0, System.Enum.GetNames(typeof(BlockType)).Length);
        }

        public void SetType(BlockType blockType)
        {
            CurrentBlockType = blockType;
            switch (CurrentBlockType)
            {
                case BlockType.None:
                    {
                        transform.DOKill();
                        if (m_cloneStage != null) m_cloneStage.transform.DOKill();
                        if (m_tweener != null) m_tweener.Kill();
                        break;
                    }
                case BlockType.Bouns: { m_cloneStage = Instantiate(m_white); break; }
                case BlockType.MoveRepeat:{m_cloneStage = Instantiate(m_brown);break; }
                case BlockType.SlowDisappear: {  m_cloneStage = Instantiate(m_gray); break; }
            }

            if (CurrentBlockType != BlockType.None)
            {
                for (int i = 0; i < m_targetModelIndexs.Count; i++)
                {
                    if (i == 0)
                    {
                        m_cloneStage.transform.SetParent(transform);
                        m_cloneStage.transform.position = m_blockModels[m_targetModelIndexs[i]].transform.position;
                        m_cloneStage.transform.rotation = m_blockModels[m_targetModelIndexs[i]].transform.rotation;
                        m_cloneStage.transform.localScale = m_blockModels[m_targetModelIndexs[i]].transform.localScale;
                    }
                    m_blockModels[m_targetModelIndexs[i]].SetActive(false);
                }
                m_targetModelIndexs.Clear();
                m_sizeScale = 1f;

                if (CurrentBlockType == BlockType.MoveRepeat)
                {
                    m_randomReverse = Random.Range(0f, 100f) <= 50f;
                    m_randomSpeed = Random.Range(0.5f, 1.25f);
                    float _mutiplier = m_randomReverse ? 1 : -1f;
                    if (currentDirectionToZ)
                    {
                        m_cloneStage.transform.position += Vector3.right * 5f * _mutiplier;
                    }
                    else
                    {
                        m_cloneStage.transform.position += Vector3.forward * 5f * _mutiplier;
                    }
                }
            }
        }

        public void PlayFeedback(bool startWithZero = true)
        {
            if (startWithZero) transform.localScale = Vector3.zero;
            transform.DOScale(new Vector3(m_sizeScale, 1f, m_sizeScale), 0.1f).OnComplete(Squash);
        }

        private void Squash()
        {
            transform.DOScale(new Vector3(m_sizeScale + 0.2f, 1.2f, m_sizeScale + 0.2f), 0.1f).OnComplete(SquashEnd);
        }

        private void SquashEnd()
        {
            transform.DOScale(new Vector3(m_sizeScale, 1f, m_sizeScale), 0.1f);
        }

        public void StartTickBlockType()
        {
            switch (CurrentBlockType)
            {
                case BlockType.Bouns:
                    {
                        break;
                    }
                case BlockType.MoveRepeat:
                    {
                        float _mutiplier = m_randomReverse ? 1 : -1f;
                        StartCoroutine(IETickBlockType_MoveRepeat(_mutiplier, m_randomSpeed));
                        break;
                    }
                case BlockType.SlowDisappear:
                    {
                        m_tweener = DOTween.To(GetCurrentScale, SetCurrentScale, 0f, 10f);  break;
                    }
            }
        }

        private System.Collections.IEnumerator IETickBlockType_MoveRepeat(float _mutiplier, float _randomSpeed)
        {
            if (CurrentBlockType == BlockType.MoveRepeat)
            {
                if (currentDirectionToZ)
                {
                    m_cloneStage.transform.DOMoveX(m_cloneStage.transform.position.x - 5f * _mutiplier, _randomSpeed).SetEase(Ease.Linear);
                }
                else
                {
                    m_cloneStage.transform.DOMoveZ(m_cloneStage.transform.position.z - 5f * _mutiplier, _randomSpeed).SetEase(Ease.Linear);
                }
                yield return new WaitForSeconds(_randomSpeed);
            }
            if (CurrentBlockType == BlockType.MoveRepeat)
            {
                if (currentDirectionToZ)
                {
                    m_cloneStage.transform.DOMoveX(m_cloneStage.transform.position.x - 5f * _mutiplier, _randomSpeed).SetEase(Ease.Linear);
                }
                else
                {
                    m_cloneStage.transform.DOMoveZ(m_cloneStage.transform.position.z - 5f * _mutiplier, _randomSpeed).SetEase(Ease.Linear);
                }
                yield return new WaitForSeconds(_randomSpeed);
            }
            if (CurrentBlockType == BlockType.MoveRepeat)
            {
                if (currentDirectionToZ)
                {
                    m_cloneStage.transform.DOMoveX(m_cloneStage.transform.position.x + 5f * _mutiplier, _randomSpeed).SetEase(Ease.Linear);
                }
                else
                {
                    m_cloneStage.transform.DOMoveZ(m_cloneStage.transform.position.z + 5f * _mutiplier, _randomSpeed).SetEase(Ease.Linear);
                }
                yield return new WaitForSeconds(_randomSpeed);
            }
            if (CurrentBlockType == BlockType.MoveRepeat)
            {
                if (currentDirectionToZ)
                {
                    m_cloneStage.transform.DOMoveX(m_cloneStage.transform.position.x + 5f * _mutiplier, _randomSpeed).SetEase(Ease.Linear);
                }
                else
                {
                    m_cloneStage.transform.DOMoveZ(m_cloneStage.transform.position.z + 5f * _mutiplier, _randomSpeed).SetEase(Ease.Linear);
                }
                yield return new WaitForSeconds(_randomSpeed);
                StartCoroutine(IETickBlockType_MoveRepeat(_mutiplier, _randomSpeed));
            }
        }

        private float GetCurrentScale()
        {
            return m_sizeScale;
        }

        private void SetCurrentScale(float v)
        {
            m_sizeScale = v;
            transform.localScale = new Vector3(v, 1f, v);
            if (m_cloneStage != null) m_cloneStage.SetActive(v > 0.01f);
            for (int i = 0; i < m_targetModelIndexs.Count; i++)
            {
                m_blockModels[m_targetModelIndexs[i]].SetActive(v > 0.01f);
            }
        }

        private void OnStartRevive()
        {
            if (m_cloneStage != null) m_cloneStage.transform.DOLocalMove(Vector3.zero, 0.5f);
        }
    }
}