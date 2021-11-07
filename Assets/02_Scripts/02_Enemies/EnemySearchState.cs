using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySearchState : IEnemyState
{
    public IEnemyState Execute(EnemyController enemy)
    {
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
        
        enemy.UpdateSearchBehaviour();
        return this;
    }

    public void Enter(EnemyController enemy)
    {
        // lost the sight of the player
        enemy.SpottedBar.fillAmount = 0;

        enemy.AnimationHandler.SetSpeed(enemy.SearchSpeed);
        enemy.StartSearchBehaviour();
    }

    public void Exit(EnemyController enemy)
    {
        enemy.FinishChecking = false;
    }
}
