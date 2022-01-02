using Enemy.Controller;
using UnityEngine;

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
            enemy.AcousticTimeToSpot = 0;
            enemy.VisionTimeToSpot = 0;
            
            enemy.PlayerSpotted = false;
            enemy.UseSpottedBar = false;
            
            enemy.AnimationHandler.SetSpeed(enemy.SearchSpeed);

            if (!enemy.SearchArea.PreparedSearchPoints)
            {
                enemy.SearchArea.GetSearchPoints();
                enemy.SearchArea.PrepareSearchBehaviour();
            }

            enemy.SearchArea.EnemySearchAmount++;
            enemy.SearchArea.StartSearchBehaviour(enemy.Agent,enemy.AnimationHandler, enemy.SearchSpeed);
            
            enemy.Agent.isStopped = false;


        }

        public void Exit(EnemyController enemy)
        {
            enemy.AcousticTimeToSpot = enemy.AcousticSecondsToSpot;
            enemy.VisionTimeToSpot = enemy.VisionSecondsToSpot;
            
            enemy.SearchArea.FinishChecking = false;
            enemy.SearchArea.EnemySearchAmount--;
            
            enemy.Agent.enabled = true;
            
            enemy.HighGroundViewCone.SetActive(false);
            enemy.LowGroundViewCone.SetActive(true);
            
            enemy.SearchArea.PreparedSearchPoints = false;
        }
    }
}
