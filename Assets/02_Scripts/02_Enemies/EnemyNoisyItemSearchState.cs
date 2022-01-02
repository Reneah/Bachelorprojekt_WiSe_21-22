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
        
            if (enemy.CanSeePlayer || enemy.PlayerSoundSpotted || enemy.ActivateChasing)
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
            // If the points are not prepared, they will be and the if condition will block other enemies to do the same again
            // will be false again, when all points are used or this state will be exit
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


