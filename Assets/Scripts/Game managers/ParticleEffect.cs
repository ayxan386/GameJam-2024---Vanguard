using System.Collections.Generic;
using UnityEngine;

public class ParticleEffect : MonoBehaviour
{
    [SerializeField] private GameObject particleEffect;

    private Dictionary<int, GameObject> particles;

    private void Start()
    {
        particles = new Dictionary<int, GameObject>();
    }

    public void ApplyEffect(PlayerController playerController)
    {
        var part = Instantiate(particleEffect, playerController.transform);
        particles[playerController.PlayerIndex] = part;
    }

    public void RemoveEffect(PlayerController playerController)
    {
        Destroy(particles[playerController.PlayerIndex]);
        particles.Remove(playerController.PlayerIndex);
    }
}