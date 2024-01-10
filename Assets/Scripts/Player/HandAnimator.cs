using UnityEngine;

public class HandAnimator : MonoBehaviour
{
    [SerializeField] private Transform targetPoint;
    [SerializeField] [Range(0, 1f)] private float ikWeight;
    [SerializeField] private Animator animator;

    private void OnAnimatorIK(int layerIndex)
    {
        print("Called");
        if (targetPoint == null) return;
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, ikWeight);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, ikWeight);
        animator.SetIKPosition(AvatarIKGoal.RightHand, targetPoint.position);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, targetPoint.position);
    }
}