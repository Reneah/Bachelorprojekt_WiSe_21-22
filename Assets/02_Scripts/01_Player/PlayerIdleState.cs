using UnityEngine;
using untitledProject;

namespace untitledProject
{
    public class PlayerIdleState : IPlayerState
    {
        Vector3 currentPosition;
        public IPlayerState Execute(PlayerController player)
        {
            bool move = player.MoveDirection.magnitude >= 0.1f;
            if (move)
            {
                return PlayerController.PlayerRunState;
            }
            
            bool jump = Input.GetKeyDown(KeyCode.Space);
            if (jump)
            {
                player.Jump();
                return PlayerController.PlayerJumpState;
            }

            if (player.PlayerThrowTrigger.Throwstate && player.CollectStones.StonesCounter > 0)
            {
                return PlayerController.PlayerThrowState;
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
