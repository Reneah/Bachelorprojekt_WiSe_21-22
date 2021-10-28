using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using untitledProject;

public class PlayerJumpState : IPlayerState
{
    public IPlayerState Execute(PlayerController player)
    {
        player.Move();
        
        if (player.IsGrounded)
        {
            return PlayerController.PlayerIdleState;
        }

        return this;
    }

    public void Enter(PlayerController player)
    {
        player.IsJumping = true;
    }

    public void Exit(PlayerController player)
    {
        player.IsJumping = false;
    }
}
