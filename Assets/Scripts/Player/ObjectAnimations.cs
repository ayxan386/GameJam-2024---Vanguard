using System;
using DG.Tweening;
using UnityEngine;

public class ObjectAnimations : MonoBehaviour
{
    [SerializeField] private Vector3 targetPosition = new Vector3(5f, 0f, 0f);
    [SerializeField] private float duration = 2f;
    [SerializeField] private Ease easeType = Ease.OutQuad;
    [SerializeField] private int loops = -1; // -1 means infinite loops
    [SerializeField] private UIAnimator.AnimationType type = UIAnimator.AnimationType.Move;

    private void Start()
    {
        switch (type)
        {
            case UIAnimator.AnimationType.Scale:
                break;
            case UIAnimator.AnimationType.Move:
                MoveAnimation();
                break;
            case UIAnimator.AnimationType.Rotate:
                RotationalAnimation();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void MoveAnimation()
    {
        transform.DOLocalMove(targetPosition, duration)
            .SetEase(easeType)
            .SetLoops(loops, LoopType.Yoyo);
    }

    private void RotationalAnimation()
    {
        transform.DORotate(new Vector3(0, 360, 0), duration, RotateMode.WorldAxisAdd)
            .SetEase(easeType)
            .SetLoops(loops);
    }
}