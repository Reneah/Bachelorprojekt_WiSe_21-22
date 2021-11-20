using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            enemy.ResetNoisyItemWaypoints = true;
            return EnemyController.EnemyChaseState;
        }

        if (enemy.FinishChecking)
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
        enemy.PrepareSearchNoisyItemBehaviour();
        enemy.StartSearchNoisyItemBehaviour();
    }

    public void Exit(EnemyController enemy)
    {
        enemy.FinishChecking = false;
    }
}
