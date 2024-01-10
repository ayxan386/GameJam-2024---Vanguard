using System.Collections;
using UnityEngine;

public class HandAnimator : MonoBehaviour
{
    [field: SerializeField] public Transform targetPoint { get; set; }
    [SerializeField] [Range(0, 1f)] private float ikWeight;
    [SerializeField] private Animator animator;
    [SerializeField] private int holdingLayer;

    public void StartHolding()
    {
        animator.SetLayerWeight(holdingLayer, 1f);
    }

    public void StopHolding(ThrowableObject throwableObject, Vector3 dir)
    {
        StartCoroutine(ResetLayerWeight(throwableObject, dir));
    }

    private IEnumerator ResetLayerWeight(ThrowableObject throwableObject, Vector3 dir)
    {
        animator.SetTrigger("throw");
        yield return new WaitForSeconds(0.2f);
        throwableObject.Throw(dir);
        animator.SetLayerWeight(holdingLayer, 0f);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, ikWeight);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, ikWeight);
        animator.SetIKPosition(AvatarIKGoal.RightHand, targetPoint.position);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, targetPoint.position);
    }
}