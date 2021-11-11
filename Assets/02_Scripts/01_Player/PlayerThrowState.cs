using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using untitledProject;

public class PlayerThrowState : IPlayerState
{
    // This bool prevents multiple execution of the Vector3.Dot if Condition
    private bool _throwDirection = false;
    
    public IPlayerState Execute(PlayerController player)
    {
        // MAYBE: adjustable angle to throw the itm
        
        // rotate to the destined location
        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, Quaternion.LookRotation(player.PlayerThrowTrigger.NoiseItemPosition.position - player.transform.position), Time.deltaTime * 5);

       // Debug.Log(Vector3.Dot(player.transform.TransformDirection(player.transform.forward),  player.PlayerThrowTrigger.NoiseItemPosition.position - player.transform.position));
        
        // when the player rotated to the throw direction, the animation will be played
        if (Vector3.Dot(player.transform.TransformDirection(player.transform.forward),  player.PlayerThrowTrigger.NoiseItemPosition.position - player.transform.position) <= -1 && !_throwDirection)
        {
            _throwDirection = true;
            player.PlayerAnimationHandler.PlayerThrow();
            // activate sound
            // destroy the noisy item
        }

        // wait to the end of the animation to be able to move and throw again
        if (player.PlayerAnimationHandler.RunningThrowAnimation)
        {
            return PlayerController.PlayerIdleState;
        }
        
        return this;
    }

    public void Enter(PlayerController player)
    {
        
    }

    public void Exit(PlayerController player)
    {
        player.PlayerThrowTrigger.Throwstate = false;
        player.PlayerAnimationHandler.RunningThrowAnimation = false;
        _throwDirection = false;
    }
}
