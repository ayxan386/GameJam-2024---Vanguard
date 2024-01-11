using UnityEngine;

public class AnimationEffect : MonoBehaviour
{
    [SerializeField] private string animationName;
    [SerializeField] private bool value;

    public void ApplyEffect(PlayerController playerController)
    {
        playerController.Animator.SetBool(animationName, value);
    }
}