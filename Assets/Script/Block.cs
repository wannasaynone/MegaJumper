﻿using UnityEngine;
using DG.Tweening;

namespace MegaJumper
{
    public class Block : MonoBehaviour
    {
        public float SizeScale { get; private set; } = 1f;

        [SerializeField] private GameObject[] m_blockModels;

        public class Factory : Zenject.PlaceholderFactory<Block>
        {

        }

        private void OnEnable()
        {
            int _ran = Random.Range(1, 3);

            for (int i = 0; i < _ran; i++)
            {
                m_blockModels[Random.Range(0, m_blockModels.Length - 1)].SetActive(true);
            }
        }

        public void RerollSize(float min = 0.5f, float max = 1f)
        {
            if (min < 0.5f)
            {
                min = 0.5f;
            }
            
            if (max > 1f)
            {
                max = 1f;
            }

            SizeScale = Random.Range(min, max);
            transform.localScale = new Vector3(SizeScale, 1f, SizeScale);
        }

        public void PlayFeedback()
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(new Vector3(SizeScale, 1f, SizeScale), 0.1f).OnComplete(Squash);
        }

        private void Squash()
        {
            transform.DOScale(new Vector3(SizeScale + 0.2f, 1.2f, SizeScale + 0.2f), 0.1f).OnComplete(SquashEnd);
        }

        private void SquashEnd()
        {
            transform.DOScale(new Vector3(SizeScale, 1f, SizeScale), 0.1f);
        }
    }
}