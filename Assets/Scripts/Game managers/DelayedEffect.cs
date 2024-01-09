using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DelayedEffect : MonoBehaviour
{
    [SerializeField] private float waitDuration;
    [SerializeField] private UnityEvent<PlayerController> effect;

    public void ApplyEffect(PlayerController playerController)
    {
        StartCoroutine(WaitThenApply(playerController));
    }

    private IEnumerator WaitThenApply(PlayerController playerController)
    {
        yield return new WaitForSeconds(waitDuration);

        effect.Invoke(playerController);
    }
}