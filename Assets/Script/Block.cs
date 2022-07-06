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
            DisappearRepeat,
            MoveRepeat,
            SquashReapet,
            SlowDisappear
        }

        [SerializeField] private GameObject[] m_blockModels;
        [SerializeField] private GameObject m_startFeverHint;
        [SerializeField] private GameObject m_gray;

        public class Factory : PlaceholderFactory<Block>
        {

        }

        private GameObject m_cloneStage;

        private float m_sizeScale = 1f;

        private System.Collections.Generic.List<int> m_targetModelIndexs = new System.Collections.Generic.List<int>();
        private BlockType m_blockType = BlockType.None;

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
            //return (BlockType)Random.Range(0, 5);
            if (Random.Range(0f, 100f) <= 50f) return BlockType.None; else return BlockType.DisappearRepeat;
        }

        public void SetType(BlockType blockType)
        {
            m_blockType = blockType;
            switch (m_blockType)
            {
                case BlockType.DisappearRepeat:
                    {
                        m_cloneStage = Instantiate(m_gray);
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
                        break;
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
            switch (m_blockType)
            {
                case BlockType.DisappearRepeat: { StartCoroutine(IETickBlockType_DisappearRepeat(m_sizeScale)); break; }
                case BlockType.SlowDisappear: { DOTween.To(GetCurrentScale, SetCurrentScale, 0f, 15f);  break; }
            }
        }

        private System.Collections.IEnumerator IETickBlockType_DisappearRepeat(float orginScale)
        {
            if (m_blockType == BlockType.DisappearRepeat) m_cloneStage.transform.DOShakePosition(1.5f, new Vector3(1f, 0f, 1f));
            yield return new WaitForSeconds(1.5f);
            if (m_blockType == BlockType.DisappearRepeat) DOTween.To(GetCurrentScale, SetCurrentScale, 0f, 0.15f);
            yield return new WaitForSeconds(0.65f);
            if (m_blockType == BlockType.DisappearRepeat) DOTween.To(GetCurrentScale, SetCurrentScale, orginScale, 0.25f);
            yield return new WaitForSeconds(1.25f);
            if (m_blockType == BlockType.DisappearRepeat) StartCoroutine(IETickBlockType_DisappearRepeat(orginScale));
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
    }
}