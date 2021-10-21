using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : IEnemyState
{
    public IEnemyState Execute(EnemyController enemy)
    {
        if (enemy.Patrolling)
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
