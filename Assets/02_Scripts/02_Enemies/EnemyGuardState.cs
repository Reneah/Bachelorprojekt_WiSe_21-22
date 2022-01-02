using Enemy.Controller;
using UnityEngine;

namespace Enemy.States
{
    public class EnemyGuardState : IEnemyState
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
            
            if (enemy.GuardPointDistance())
            {
                enemy.ReachedGuardpoint = true;
                enemy.AnimationHandler.SetSpeed(0);
                enemy.GuardBehaviour = true;
            }
        
            if (enemy.ReachedGuardpoint)
            {
                enemy.DesiredStandingLookDirection();
                enemy.UpdateGuardBehaviour();
            }

            return this;
        }

        public void Enter(EnemyController enemy)
        {
            enemy.PlayerSpotted = false;
        
            enemy.Agent.SetDestination(enemy.GuardPoint.transform.position);
            enemy.AnimationHandler.SetSpeed(enemy.PatrolSpeed);
            
            enemy.Agent.isStopped = false;
            
            enemy.EnemyTalkCheck.Talkable = true;
        }

        public void Exit(EnemyController enemy)
        {
            enemy.GuardBehaviour = false;
            enemy.ReachedGuardpoint = false;
        }
    }
}

