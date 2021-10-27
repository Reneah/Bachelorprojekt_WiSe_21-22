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
            return  EnemyController.EnemyPatrolState;
        }
        
        enemy.UpdateSearchBehaviour();
        return this;
    }

    public void Enter(EnemyController enemy)
    {
        enemy.StartSearchBehaviour();
    }

    public void Exit(EnemyController enemy)
    {
        enemy.FinishChecking = false;
    }
}
