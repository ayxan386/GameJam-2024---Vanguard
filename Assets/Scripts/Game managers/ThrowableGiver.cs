using UnityEngine;

public class ThrowableGiver : MonoBehaviour
{
    [SerializeField] private ThrowableObject throwableObjectPrefab;

    public void ApplyEffect(PlayerController playerController)
    {
        if (playerController.Throwable != null) return;

        var throwableObject = Instantiate(throwableObjectPrefab, playerController.handAnimator.targetPoint);
        throwableObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        playerController.Throwable = throwableObject;
        playerController.handAnimator.StartHolding();
    }
}