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
        else
        {
            return EnemyController.EnemyPatrolState;
        }
        return this;
    }

    public void Enter(EnemyController enemy)
    {
   
    }

    public void Exit(EnemyController enemy)
    {

    }
}
