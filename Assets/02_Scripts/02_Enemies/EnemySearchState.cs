using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySearchState : IEnemyState
{
    public IEnemyState Execute(EnemyController enemy)
    {
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
    }

    public void Exit(EnemyController enemy)
    {
        enemy.FinishChecking = false;
    }
}
