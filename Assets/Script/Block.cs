using UnityEngine;
using DG.Tweening;

namespace MegaJumper
{
    public class Block : MonoBehaviour
    {
        [SerializeField] private MoreMountains.Feedbacks.MMF_Player m_feedback;

        public class Factory : Zenject.PlaceholderFactory<Block>
        {

        }

        public void PlayFeedback()
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, 0.1f).OnComplete(m_feedback.PlayFeedbacks);
        }
    }
}