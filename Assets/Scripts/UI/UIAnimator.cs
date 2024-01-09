using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIAnimator : MonoBehaviour
{
        // Enum to define the animation types
        public enum AnimationType
        {
            Scale,
            Move,
            Fade
        }
    
        [SerializeField] private AnimationType animationType = AnimationType.Scale;
        [SerializeField] private Vector3 targetScale = new Vector3(1.2f, 1.2f, 1.2f);
        [SerializeField] private Vector3 targetPosition = new Vector3(100f, 100f, 0f);
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private Ease easeType = Ease.OutQuad;
    
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
    
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
        }
    
        public void PlayAnimation()
        {
            // Ensure the DOTween module is initialized
            if (!DOTween.IsTweening(rectTransform))
            {
                // Choose the appropriate animation type
                switch (animationType)
                {
                    case AnimationType.Scale:
                        PlayScaleAnimation();
                        break;
                    case AnimationType.Move:
                        PlayMoveAnimation();
                        break;
                    case AnimationType.Fade:
                        PlayFadeAnimation();
                        break;
                }
            }
        }
    
        private void PlayScaleAnimation()
        {
            rectTransform.DOScale(targetScale, animationDuration)
                .SetEase(easeType);
        }
    
        private void PlayMoveAnimation()
        {
            rectTransform.DOAnchorPos(targetPosition, animationDuration)
                .SetEase(easeType);
        }
    
        private void PlayFadeAnimation()
        {
            if (canvasGroup != null)
            {
                canvasGroup.DOFade(0f, animationDuration)
                    .SetEase(easeType);
            }
        }
}
