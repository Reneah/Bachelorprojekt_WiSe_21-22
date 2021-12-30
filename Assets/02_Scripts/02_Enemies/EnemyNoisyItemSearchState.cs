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
            
            if (enemy.NoisyItemSearchArea.FinishChecking)
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
            
            if (!enemy.NoisyItemSearchArea.PreparedSearchPoints)
            {
                enemy.NoisyItemSearchArea.GetSearchPoints();
                enemy.NoisyItemSearchArea.PrepareSearchNoisyItemBehaviour();
            }
            
            enemy.NoisyItemSearchArea.StartSearchNoisyItemBehaviour(enemy.Agent, enemy.AnimationHandler, enemy.InvestigationRunSpeed, enemy.NoisyItemScript);
            
            enemy.Agent.isStopped = false;
        }

        public void Exit(EnemyController enemy)
        {
            enemy.NoisyItemSearchArea.FinishChecking = false;
            enemy.NoisyItemSearchArea.PreparedSearchPoints = false;
        }
    }
}


