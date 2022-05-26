using System.Collections.Generic;

namespace MegaJumper
{
    public class BlockManager 
    {
        private readonly Block.Factory m_blockFactory;
        private readonly GameProperties m_gameProperties;

        private List<Block> m_clonedBlock = new List<Block>();

        public BlockManager(Block.Factory factory, GameProperties gameProperties)
        {
            m_blockFactory = factory;
            m_gameProperties = gameProperties;
        }

        public void CreateNew()
        {
            UnityEngine.Vector3 _orginPos = UnityEngine.Vector3.zero;

            if (m_clonedBlock.Count > 0)
            {
                _orginPos = m_clonedBlock[m_clonedBlock.Count - 1].transform.position;
            }

            UnityEngine.Vector3 _newPos = _orginPos + new UnityEngine.Vector3(0f, 0f, UnityEngine.Random.Range(m_gameProperties.MIN_ADD_DISTANCE, m_gameProperties.MAX_ADD_DISTANCE));

            Block _clone = m_blockFactory.Create();
            _clone.transform.position = _newPos;
            m_clonedBlock.Add(_clone);

            if (m_clonedBlock.Count >= m_gameProperties.MAX_CLONE_COUNT)
            {
                UnityEngine.Object.Destroy(m_clonedBlock[0].gameObject);
                m_clonedBlock.RemoveAt(0);
            }
        }
    }
}