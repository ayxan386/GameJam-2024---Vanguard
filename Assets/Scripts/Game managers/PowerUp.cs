using UnityEngine;
using UnityEngine.Events;

public class PowerUp : MonoBehaviour
{
    [field: SerializeField] public int Id { get; private set; }
    [SerializeField] private UnityEvent<PlayerController> effects;

    public bool CanBeUsed { get; set; } = true;

    public void Use(PlayerController playerController)
    {
        if (!CanBeUsed) return;

        CanBeUsed = false;
        effects.Invoke(playerController);
    }
}