using RootMotion.Demos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStatus : MonoBehaviour
{
    [Header("Is Character State Item Used?")]
    public bool speedItemUsed;
    public bool jumpItemUsed;

    [Header("Jump")]
    [SerializeField]
    private float jumpPower = 1f;
    [SerializeField]
    private float jumpPowerUp = 20f;

    [Header("Speed")]
    [SerializeField]
    private float speedPower = 1f;
    [SerializeField]
    private float speedPowerUp = 5f;

    [Header("Settings")]
    [SerializeField]
    private CharacterThirdPerson character;

    private void Start()
    {
        character = GetComponentInChildren<CharacterThirdPerson>();

        jumpPower = character.jumpPower;
        speedPower = character.speedItemMultiplier;
    }

    private void FixedUpdate()
    {
        StatusItem();
    }


    private void StatusItem()
    {
        if (speedItemUsed)
        {
            SetSpeed(speedPowerUp);
            Invoke("SpeedItemFinished", 10f);
        }
        else
        {
            SetSpeed(speedPower);
        }

        if (jumpItemUsed)
        {
            SetJumpPower(jumpPowerUp);
            Invoke("JumpItemFinished", 10f);
        }
        else
        {
            SetJumpPower(jumpPower);
        }
    }

    private void SetSpeed(float speed)
    {
        if (!character) return;
        character.speedItemMultiplier = speed;
        
    }

    private void SetJumpPower(float power)
    {
        if (!character) return;
        character.jumpPower = power;
        
    }

    private void SpeedItemFinished()
    {
        speedItemUsed = false;
    }

    private void JumpItemFinished()
    {
        jumpItemUsed = false;
    }
}
