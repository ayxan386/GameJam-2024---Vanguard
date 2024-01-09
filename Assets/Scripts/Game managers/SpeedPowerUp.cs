using System;
using UnityEngine;

public class SpeedPowerUp : MonoBehaviour
{
    [SerializeField] private float boostEffect;

    public void ApplyEffect(PlayerController playerController)
    {
        playerController.TemporaryBoostForSpeed(boostEffect);
    }

}