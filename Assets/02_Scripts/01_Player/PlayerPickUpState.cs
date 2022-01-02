using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using untitledProject;


    public class PlayerPickUpState : IPlayerState
    {
        public IPlayerState Execute(PlayerController player)
        {
            if (!player.PlayerAnimationHandler.RunningPickUpAnimation)
            {
                return PlayerController.PlayerIdleState;
            }
            
            return PlayerController.PlayerPickUpState;
        }

        public void Enter(PlayerController player)
        {
            player.PlayerAnimationHandler.RunningPickUpAnimation = true;
            player.PlayerAnimationHandler.PlayerPickUp();
        }

        public void Exit(PlayerController player)
        {
            player.PlayerAnimationHandler.ResetPickUpAnimation();
            player.PlayerAnimationHandler.RunningPickUpAnimation = false;
            player.PickUpItem = false;
       
        }
    }

