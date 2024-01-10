using UnityEngine;

public class DestroyTarget : MonoBehaviour
{
    [SerializeField] private GameObject target;

    public void ApplyEffect(PlayerController playerController)
    {
        Destroy(target);
    }
}