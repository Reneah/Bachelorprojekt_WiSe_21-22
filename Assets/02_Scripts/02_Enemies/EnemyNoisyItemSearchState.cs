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
        // lost the sight of the player
        enemy.SpottedBar.fillAmount = 0;
        
        enemy.StartSearchNoisyItemBehaviour();
    }

    public void Exit(EnemyController enemy)
    {
        enemy.FinishChecking = false;
    }
}
