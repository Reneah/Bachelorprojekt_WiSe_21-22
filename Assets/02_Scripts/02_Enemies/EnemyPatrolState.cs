using Enemy.Controller;
using UnityEngine;

namespace Enemy.States
{
    public class EnemyPatrolState : IEnemyState
    {
        public IEnemyState Execute(EnemyController enemy)
        {
            if (enemy.CanSeePlayer || enemy.ActivateChasing || enemy.PlayerSoundSpotted)
            {
                enemy.EnemyTalkCheck.Talkable = false;
                return EnemyController.EnemyVisionChaseState;
            }

            if (enemy.SoundNoticed)
            {
                enemy.EnemyTalkCheck.Talkable = false;
                return EnemyController.EnemySoundInvestigationState;
            }

            if (enemy.EnemyTalkCheck.Talk)
            {
                return EnemyController.EnemyTalkState;
            }

            if (enemy.Loot)
            {
                enemy.EnemyTalkCheck.Talkable = false;
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
            enemy.AbleToLoot = true;
        }

        public void Exit(EnemyController enemy)
        {
            enemy.SoundNoticed = false;
            enemy.AbleToLoot = false;
        }
    }
}


