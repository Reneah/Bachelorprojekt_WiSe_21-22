using Enemy.Controller;

namespace Enemy.States
{
    public class EnemyNoisyItemSearchState : IEnemyState
    {
        public IEnemyState Execute(EnemyController enemy)
        {
            if (enemy.SoundNoticed)
            {
                return EnemyController.EnemySoundInvestigationState;
            }
        
            if (enemy.CanSeePlayer)
            {
                return EnemyController.EnemyVisionChaseState;
            }

            if (enemy.FinishChecking)
            {
                if (enemy.Guarding)
                {
                    return EnemyController.EnemyGuardState;
                }
            
                return EnemyController.EnemyPatrolState;
            }
        
            enemy.UpdateSearchNoisyItemBehaviour();
            return this;
        }

        public void Enter(EnemyController enemy)
        {
            enemy.PrepareSearchNoisyItemBehaviour();
            enemy.StartSearchNoisyItemBehaviour();
            
            enemy.Agent.isStopped = false;
        }

        public void Exit(EnemyController enemy)
        {
            enemy.ResetNoisyItemWaypoints = true;
            enemy.FinishChecking = false;
        }
    }
}


