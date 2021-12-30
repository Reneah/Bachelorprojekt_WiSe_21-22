using Enemy.Controller;
using UnityEngine;

namespace Enemy.States
{
    public class EnemyPatrolState : IEnemyState
    {
        public IEnemyState Execute(EnemyController enemy)
        {
            if (enemy.CanSeePlayer || enemy.ActivateChasing)
            {
                return EnemyController.EnemyVisionChaseState;
            }

            if (enemy.SoundNoticed)
            {
                return EnemyController.EnemySoundInvestigationState;
            }

            if (enemy.EnemyTalkCheck.Talk)
            {
                return EnemyController.EnemyTalkState;
            }

            if (enemy.Loot)
            {
                return EnemyController.EnemyLootState;
            }
        
            enemy.UpdatePatrolBehaviour();
            return this;
        }

        public void Enter(EnemyController enemy)
        {
            enemy.PlayerSpotted = false;
        
            enemy.AnimationHandler.SetSpeed(enemy.PatrolSpeed);
            enemy.StartPatrolBehaviour();
            
            enemy.Agent.isStopped = false;
            
            enemy.EnemyTalkCheck.Talkable = true;
        }

        public void Exit(EnemyController enemy)
        {
            enemy.SoundNoticed = false;
        }
    }
}


