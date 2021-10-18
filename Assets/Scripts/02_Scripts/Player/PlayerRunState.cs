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
