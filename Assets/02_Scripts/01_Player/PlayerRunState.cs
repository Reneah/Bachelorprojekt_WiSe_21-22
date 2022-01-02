using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace untitledProject
{
    public class PlayerRunState : IPlayerState
    {
        Vector3 currentPosition;
        public IPlayerState Execute(PlayerController player)
        {
            bool move = player.MoveDirection.magnitude >= 0.1f;
            if (!move)
            {
                return PlayerController.PlayerIdleState;
            }

            bool jump = Input.GetKeyDown(KeyCode.Space);
            if (jump)
            {
                player.Jump();
                return PlayerController.PlayerJumpState;
            }
            
            if (player.PlayerThrowTrigger.Throwstate && player.CollectStones.StonesCounter > 0 && player.IsGrounded)
            {
                return PlayerController.PlayerThrowState;
            }
            
            if (player.PickUpItem)
            {
                return PlayerController.PlayerPickUpState;
            }
            
            player.MovementExecution();
            return this;
        }
    
        public void Enter(PlayerController player)
        {
            
        }
    
        public void Exit(PlayerController player)
        {
            
        }
    }
}
