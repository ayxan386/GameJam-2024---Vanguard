using DG.Tweening;
using UnityEngine;

public class ObjectAnimations : MonoBehaviour
{
    [SerializeField] private Vector3 targetPosition = new Vector3(5f, 0f, 0f);
    [SerializeField] private float duration = 2f;
    [SerializeField] private Ease easeType = Ease.OutQuad;
    [SerializeField] private int loops = -1; // -1 means infinite loops

    private void Start()
    {
        StartLoopedAnimation();
    }

    private void StartLoopedAnimation()
    {
        transform.DOLocalMove(targetPosition, duration)
            .SetEase(easeType)
            .SetLoops(loops, LoopType.Yoyo);
    }
}