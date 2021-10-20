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
