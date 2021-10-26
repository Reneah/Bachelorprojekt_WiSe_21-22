using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInvestigationState : IEnemyState
{
    public IEnemyState Execute(EnemyController enemy)
    {
        if (enemy.CanSeePlayer)
        {
            return EnemyController.EnemyChaseState;
        }
        
        // the distance between the sound event and the enemy
        float distance = Vector3.Distance(enemy.SoundEventPosition.position, enemy.transform.position);

        if (distance <= 0.4f)
        {
            if (enemy.SoundBehaviourStage == 1)
            {
                // play the investigation animation & when the Animation is finished, the enemy will patrol again
                return EnemyController.EnemyPatrolState;
            }

            if (enemy.SoundBehaviourStage == 2)
            {
                // play the investigation animation, when the Animation is finished, the enemy searches a little bit and then goes back to patrolling 
                return EnemyController.EnemyPatrolState;
            }
            
            if (enemy.SoundBehaviourStage == 3)
            {
                // play the investigation animation, when the Animation is finished, the enemy goes in search mode for the player
                return EnemyController.EnemyPatrolState;
            }

        }
        return this;
    }

    public void Enter(EnemyController enemy)
    {
        if (enemy.SoundBehaviourStage == 1)
        {
            enemy.Agent.speed = enemy.FirstStageRunSpeed;
            enemy.Agent.SetDestination(enemy.SoundEventPosition.position);
        }
        if (enemy.SoundBehaviourStage == 2)
        {
            enemy.Agent.speed = enemy.SecondStageRunSpeed;
            enemy.Agent.SetDestination(enemy.SoundEventPosition.position);
        }
        if (enemy.SoundBehaviourStage == 3)
        {
            enemy.Agent.speed = enemy.ThirdStageRunSpeed;
            enemy.Agent.SetDestination(enemy.SoundEventPosition.position);
        }
    }

    public void Exit(EnemyController enemy)
    {
        
    }
}
