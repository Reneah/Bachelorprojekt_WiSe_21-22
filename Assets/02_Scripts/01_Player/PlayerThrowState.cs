using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using untitledProject;

public class PlayerThrowState : IPlayerState
{
    // this bool prevents multiple execution of the Vector3.Dot if Condition
    private bool _reachedThrowDirection = false;

    // based on the rotation speed, this time has to be modified to get the right time to throw
    private float _activateThrowAnimationTime;

    // activate and deactivate the cooldown to run not all the time
    private bool _activateCooldown = true;
    
    public IPlayerState Execute(PlayerController player)
    {
        // rotate to the destined location
        Quaternion playerRotation =  Quaternion.Slerp(player.transform.rotation, Quaternion.LookRotation(player.PlayerThrowTrigger.NoisyItem.transform.position - player.transform.position), Time.deltaTime * player.PlayerThrowTrigger.RotationSpeed);
        playerRotation.x = 0;
        playerRotation.z = 0;
        player.transform.rotation = playerRotation;

        if (_activateCooldown)
        {
            _activateThrowAnimationTime -= Time.deltaTime;
        }
        
        // when the player rotated to the throw direction, the animation will be played
        if (_activateThrowAnimationTime <= 0 && !_reachedThrowDirection)
        {
            _activateCooldown = false;
            _reachedThrowDirection = true;
            _activateThrowAnimationTime = player.PlayerThrowTrigger.WaitToThrowDuringRotation;
            player.PlayerAnimationHandler.PlayerThrow();
        }

        // wait to the end of the animation to be able to move and throw again
        // activate sound
        if (player.PlayerAnimationHandler.RunningThrowAnimation)
        {
            player.PlayerThrowTrigger.NoisyItem.SoundRangeCollider.SetActive(true);

            // if the item was already used the item stage will be increased
            if (player.PlayerThrowTrigger.NoisyItem.OneTimeUsed)
            {
                player.PlayerThrowTrigger.NoisyItem.Stage++;

                if (player.PlayerThrowTrigger.NoisyItem.Stage >= 3)
                {
                    player.PlayerThrowTrigger.NoisyItem.Stage = 3;
                }
            }
            player.PlayerThrowTrigger.NoisyItem.ItemUsed = true;
            
            return PlayerController.PlayerIdleState;
        }
        
        return this;
    }

    public void Enter(PlayerController player)
    {
        _activateThrowAnimationTime = player.PlayerThrowTrigger.WaitToThrowDuringRotation;
        player.CollectStones.StoneUsed();
    }

    public void Exit(PlayerController player)
    {
        player.PlayerThrowTrigger.Throwstate = false;
        player.PlayerAnimationHandler.RunningThrowAnimation = false;
        _reachedThrowDirection = false;
        _activateCooldown = true;
    }
}
