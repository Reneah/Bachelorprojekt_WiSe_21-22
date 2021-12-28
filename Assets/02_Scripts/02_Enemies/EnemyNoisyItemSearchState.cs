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

            // NOTE: Search Area Finishing is wrong here
            if (enemy.SearchArea.FinishChecking)
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
            enemy.EnemyTalkCheck.Talkable = false;
            enemy.PrepareSearchNoisyItemBehaviour();
            enemy.StartSearchNoisyItemBehaviour();
            
            enemy.Agent.isStopped = false;
        }

        public void Exit(EnemyController enemy)
        {
            enemy.ResetNoisyItemWaypoints = true;
            
            // NOTE: Search Area Finishing is wrong here
            enemy.SearchArea.FinishChecking = false;
        }
    }
}


