using UnityEngine;
using DG.Tweening;

public class GrowCompressEffect : MonoBehaviour
{
    [SerializeField] private Vector3 m_BaseScale;
    [SerializeField] private float m_AnimationCycleDuration;
    [SerializeField] private float m_GrowSize = 1.1F;
    [SerializeField] private float m_CompressSize = 0.8F;

    private void Start()
    {
        Compress();
    }

    public void Grow()
    {
        Vector3 newScale = m_BaseScale * m_GrowSize;
        transform.DOScale(newScale, m_AnimationCycleDuration).OnComplete(Compress);
    }

    private void Compress()
    {
        Vector3 newScale = m_BaseScale * m_CompressSize;
        transform.DOScale(newScale, m_AnimationCycleDuration).OnComplete(Grow);
    }
}
