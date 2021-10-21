using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolState : IEnemyState
{
    public IEnemyState Execute(EnemyController _enemy)
    {
        _enemy.UpdatePatrolBehaviour();
        return this;
    }

    public void Enter(EnemyController _enemy)
    {
        _enemy.StartPatrolBehaviour();
    }

    public void Exit(EnemyController _enemy)
    {
        
    }
}
