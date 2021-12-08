using Enemy.Controller;

namespace Enemy.States
{
    public class EnemySearchState : IEnemyState
    {
        public IEnemyState Execute(EnemyController enemy)
        {
            enemy.CheckPlayerGround();
            
            if (enemy.CanSeePlayer)
            {
                enemy.ResetSearchWaypoints = true;
                return EnemyController.EnemyVisionChaseState;
            }
        
            if (enemy.SoundNoticed)
            {
                return EnemyController.EnemySoundInvestigationState;
            }

            if (enemy.FinishChecking)
            {
                if (enemy.Guarding)
                {
                    return EnemyController.EnemyGuardState;
                }
            
                return EnemyController.EnemyPatrolState;
            }
        
            enemy.UpdateSearchBehaviour();
            return this;
        }

        public void Enter(EnemyController enemy)
        {
            enemy.AnimationHandler.SetSpeed(enemy.SearchSpeed);
            enemy.StartSearchBehaviour();
            
            enemy.Agent.isStopped = true;
        }

        public void Exit(EnemyController enemy)
        {
            enemy.FinishChecking = false;
        }
    
    }

}
