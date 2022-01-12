using Enemy.Controller;

namespace Enemy.States
{
    public class EnemySearchState : IEnemyState
    {
        public IEnemyState Execute(EnemyController enemy)
        {
            enemy.CheckPlayerGround();
            
            if (enemy.CanSeePlayer || enemy.ActivateChasing || enemy.PlayerSoundSpotted)
            {
                return EnemyController.EnemyVisionChaseState;
            }
        
            if (enemy.SoundNoticed)
            {
                return EnemyController.EnemySoundInvestigationState;
            }

            if (enemy.SearchArea.FinishChecking)
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
            // detection time will be set to 0 because the enemy knows that the player is nearby and will react instantly
            enemy.AcousticTimeToSpot = 0;
            enemy.VisionTimeToSpot = 0;
            
            enemy.PlayerSpotted = false;
            enemy.UseSpottedBar = false;
            
            enemy.AnimationHandler.SetSpeed(enemy.SearchSpeed);

            // If the points are not prepared, they will be and the if condition will block other enemies to do the same again
            // will be false again, when all points are used or this state will be exit
            if (!enemy.SearchArea.PreparedSearchPoints)
            {
                enemy.SearchArea.GetSearchPoints();
                enemy.SearchArea.PrepareSearchBehaviour();
            }

            // max amount of enemies is 2 to search the player, so this value counts + 1
            enemy.SearchArea.EnemySearchAmount++;
            
            enemy.SearchArea.StartSearchBehaviour(enemy.Agent,enemy.AnimationHandler, enemy.SearchSpeed);
            
            enemy.Agent.isStopped = false;
        }

        public void Exit(EnemyController enemy)
        {
            // detection time will be reset
            enemy.AcousticTimeToSpot = enemy.AcousticSecondsToSpot;
            enemy.VisionTimeToSpot = enemy.VisionSecondsToSpot;
            
            enemy.SearchArea.FinishChecking = false;
            
            // enemy won't search anymore and counts down
            enemy.SearchArea.EnemySearchAmount--;
            
            enemy.Agent.enabled = true;
            
            // be sure that the low vision cone will be activated when the enemy goes back to the routine
            enemy.HighGroundViewCone.SetActive(false);
            enemy.LowGroundViewCone.SetActive(true);
            
            enemy.SearchArea.PreparedSearchPoints = false;
        }
    }
}
