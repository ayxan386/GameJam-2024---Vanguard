using UnityEngine;
using UnityEngine.Events;

public class FreezePowerUp : MonoBehaviour
{
    [SerializeField] private UnityEvent<PlayerController> otherPlayerEffects;
    [SerializeField] private UnityEvent<PlayerController> otherPlayerEffectReversal;

    public void ApplyEffect(PlayerController player)
    {
        foreach (var otherPlayer in MainGameManager.Instance.Players)
        {
            if (otherPlayer.PlayerIndex == player.PlayerIndex) continue;

            otherPlayer.IsFrozen = true;
            otherPlayerEffects.Invoke(otherPlayer);
        }
    }

    public void ReverseEffect(PlayerController player)
    {
        foreach (var otherPlayer in MainGameManager.Instance.Players)
        {
            if (otherPlayer.PlayerIndex == player.PlayerIndex) continue;

            otherPlayer.IsFrozen = false;
            otherPlayerEffectReversal.Invoke(otherPlayer);
        }
    }
}