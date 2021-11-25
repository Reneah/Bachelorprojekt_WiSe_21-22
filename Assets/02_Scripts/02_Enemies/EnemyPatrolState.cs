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
            return EnemyController.EnemyVisionChaseState;
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
        // only when the enemy enters the patrol or guard mode, the enemy will stop to see the player instantly, because he lost the orientation of him
        enemy.SpottedTime = 0;
        enemy.PlayerSpotted = false;
        
        enemy.AnimationHandler.SetSpeed(enemy.PatrolSpeed);
        enemy.StartPatrolBehaviour();
    }

    public void Exit(EnemyController enemy)
    {
        enemy.SoundNoticed = false;
    }
}
