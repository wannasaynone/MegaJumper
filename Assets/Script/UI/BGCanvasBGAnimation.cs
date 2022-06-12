using UnityEngine;
using DG.Tweening;

namespace MegaJumper.UI
{
    public class BGCanvasBGAnimation : MonoBehaviour
    {
        [SerializeField] private Transform m_bg1;
        [SerializeField] private Transform m_bg2;
        [SerializeField] private float m_endPosY;
        [SerializeField] private float m_restartPosY;
        [SerializeField] private float m_moveTime_bg1;
        [SerializeField] private float m_moveTime_bg2;

        private void Start()
        {
            m_bg1.DOLocalMoveY(m_endPosY, m_moveTime_bg1).SetEase(Ease.Linear).OnComplete(OnBG1MovedToEnd);
            m_bg2.DOLocalMoveY(m_endPosY, m_moveTime_bg2).SetEase(Ease.Linear).OnComplete(OnBG2MovedToEnd);
        }

        private void OnBG1MovedToEnd()
        {
            Reset(m_bg1, m_moveTime_bg1, OnBG1MovedToEnd);
        }

        private void OnBG2MovedToEnd()
        {
            Reset(m_bg2, m_moveTime_bg2, OnBG2MovedToEnd);
        }

        private void Reset(Transform bgTrans, float time, TweenCallback onEnded)
        {
            bgTrans.localPosition = new Vector3(bgTrans.localPosition.x, m_restartPosY);
            bgTrans.DOLocalMoveY(m_endPosY, time).SetEase(Ease.Linear).OnComplete(onEnded);
        }
    }
}