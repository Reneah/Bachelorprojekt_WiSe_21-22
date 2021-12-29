using Enemy.Controller;
using UnityEngine;

namespace Enemy.States
{
    public class EnemySearchState : IEnemyState
    {
        public IEnemyState Execute(EnemyController enemy)
        {
            enemy.CheckPlayerGround();
            
            if (enemy.CanSeePlayer || enemy.ActivateChasing)
            {
                enemy.ResetSearchWaypoints = true;
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
            // NOTE: reduce here the spotted time when searching after the player
            
            enemy.PlayerSpotted = false;
            enemy.UseSpottedBar = false;

            enemy.EnemyTalkCheck.Talkable = false;
            enemy.AnimationHandler.SetSpeed(enemy.SearchSpeed);

            if (!enemy.SearchArea.PreparedSearchPoints)
            {
                enemy.SearchArea.PrepareSearchBehaviour();
            }

            enemy.SearchArea.EnemySearchAmount++;
            enemy.SearchArea.StartSearchBehaviour(enemy.Agent,enemy.AnimationHandler, enemy.SearchSpeed);
            
            enemy.Agent.isStopped = false;


        }

        public void Exit(EnemyController enemy)
        {
            // NOTE: reset here the spotted time when the search is over
            
            enemy.SearchArea.FinishChecking = false;
            enemy.SearchArea.EnemySearchAmount--;
            
            enemy.Agent.enabled = true;
        }
    }
}
