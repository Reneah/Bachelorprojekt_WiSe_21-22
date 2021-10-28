using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolState : IEnemyState
{
    public IEnemyState Execute(EnemyController enemy)
    {
        if (enemy.CanSeePlayer)
        {
            return EnemyController.EnemyChaseState;
        }

        if (enemy.SoundNoticed)
        {
            return EnemyController.EnemySoundInvestigationState;
        }
        
        enemy.UpdatePatrolBehaviour();
        return this;
    }

    public void Enter(EnemyController enemy)
    {
        enemy.AnimationHandler.SetSpeed(enemy.PatrolSpeed);
        enemy.StartPatrolBehaviour();
    }

    public void Exit(EnemyController enemy)
    {
        enemy.SoundNoticed = false;
    }
}
