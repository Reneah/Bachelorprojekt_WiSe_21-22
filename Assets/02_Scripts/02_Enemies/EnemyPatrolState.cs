using Enemy.Controller;

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

            if (enemy.EnemyTalkCheck.Talkable)
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
            // only when the enemy enters the patrol or guard mode, the enemy will stop to see the player instantly, because he lost the orientation of him
            enemy.SpotTime = 0;
            enemy.PlayerSpotted = false;
        
            enemy.AnimationHandler.SetSpeed(enemy.PatrolSpeed);
            enemy.StartPatrolBehaviour();
            
            enemy.Agent.isStopped = false;
        }

        public void Exit(EnemyController enemy)
        {
            enemy.SoundNoticed = false;
        }
    }
}


