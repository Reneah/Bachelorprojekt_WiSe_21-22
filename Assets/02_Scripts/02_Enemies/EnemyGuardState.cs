using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGuardState : IEnemyState
{
    public IEnemyState Execute(EnemyController enemy)
    {
        if (enemy.CanSeePlayer)
        {
            return EnemyController.EnemyVisionChaseState;
        }

        if (enemy.SoundNoticed)
        {
            return EnemyController.EnemySoundInvestigationState;
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
        // only when the enemy enters the patrol or guard mode, the enemy will stop to see the player instantly, because he lost the orientation of him
        enemy.SpottedTime = 0;
        enemy.PlayerSpotted = false;
        
        enemy.Agent.SetDestination(enemy.GuardPoint.transform.position);
        enemy.AnimationHandler.SetSpeed(enemy.PatrolSpeed);
    }

    public void Exit(EnemyController enemy)
    {
        enemy.GuardBehaviour = false;
        enemy.ReachedGuardpoint = false;
    }
}
