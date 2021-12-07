using Enemy.Controller;
using UnityEngine;

namespace Enemy.States
{
    public class EnemyTalkState : IEnemyState
    {
        public IEnemyState Execute(EnemyController enemy)
        {
            if (enemy.CanSeePlayer)
            {
                return EnemyController.EnemyVisionChaseState;
            }

            if (enemy.SoundNoticed)
            {
                return EnemyController.EnemySoundInvestigationState;
            }
            
            // When the enemy is finished talking he will go back to his routine
            if (!enemy.EnemyTalkCheck.Talkable)
            {
                if (enemy.Patrolling)
                {
                    return EnemyController.EnemyPatrolState;
                }
                
                if (enemy.Guarding)
                {
                    return EnemyController.EnemyGuardState;
                }
              
            }
            return this;
        }

        public void Enter(EnemyController enemy)
        {
           
        }

        public void Exit(EnemyController enemy)
        {
            enemy.Agent.isStopped = false;
            enemy.EnemyTalkCheck.Talkable = false;
            
        }
    }
}
