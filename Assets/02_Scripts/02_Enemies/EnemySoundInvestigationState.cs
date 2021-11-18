using System.Collections;
using System.Collections.Generic;
using System.Net.Configuration;
using UnityEngine;

//BUG: At Footsteps the soundevent Location will be updated but not the agent destination, thus the enemy can get stuck and the distance can't get under 1
public class EnemySoundInvestigationState : IEnemyState
{
    public IEnemyState Execute(EnemyController enemy)
    {
        if (enemy.CanSeePlayer)
        {
            enemy.AnimationHandler.FinishedInvestigationAnimation = false;
            enemy.AnimationHandler.FinishedLookingAnimation = false;
            enemy.AnimationHandler.ResetInvestigatePoint();
            enemy.AnimationHandler.ResetLookingAround();
            
            return EnemyController.EnemyChaseState;
        }
        
        UpdateSearchStage(enemy);
        enemy.DistanceToSoundEvent();
        
        if (enemy.DistanceToSoundEvent() <= 1)
        {
            // prevent that the walking animation will be played
            enemy.AnimationHandler.SetSpeed(0);
            
            if (enemy.SoundBehaviourStage == 1)
            {
                // plays the investigation animation & when the Animation is finished, the enemy will patrol again
                if (!enemy.AnimationActivated)
                {
                    enemy.AnimationHandler.InvestigatePoint();
                    enemy.AnimationActivated = true;
                }
                
                if (enemy.AnimationHandler.FinishedInvestigationAnimation)
                {
                    
                    enemy.AnimationHandler.ResetInvestigatePoint();
                    enemy.AnimationHandler.FinishedInvestigationAnimation = false;

                    if (enemy.Guarding)
                    {
                        return EnemyController.EnemyGuardState;
                    }
                    
                    return EnemyController.EnemyPatrolState;
                }
                
            }

            if (enemy.SoundBehaviourStage == 2)
            {
                // play the investigation animation, when the Animation is finished, the enemy looks around and then goes back to patrolling 
                if (!enemy.AnimationActivated)
                {
                    enemy.AnimationHandler.InvestigatePoint();
                    enemy.AnimationHandler.LookingAround();
                    enemy.AnimationActivated = true;
                }

                if (enemy.AnimationHandler.FinishedLookingAnimation)
                {
                    enemy.AnimationHandler.FinishedInvestigationAnimation = false;
                    enemy.AnimationHandler.FinishedLookingAnimation = false;
                    enemy.AnimationHandler.ResetInvestigatePoint();
                    enemy.AnimationHandler.ResetLookingAround();
                    
                    if (enemy.Guarding)
                    {
                        return EnemyController.EnemyGuardState;
                    }
                    
                    return EnemyController.EnemyPatrolState;
                }
            }
            
            if (enemy.SoundBehaviourStage == 3)
            {
                // play the investigation animation, when the Animation is finished, the enemy goes in search mode for the player
                if (!enemy.AnimationActivated)
                {
                    enemy.AnimationHandler.InvestigatePoint();
                    enemy.AnimationActivated = true;
                }
                
                if (enemy.AnimationHandler.FinishedInvestigationAnimation)
                {
                    enemy.AnimationHandler.FinishedInvestigationAnimation = false;
                    enemy.AnimationHandler.ResetInvestigatePoint();
                    
                    return EnemyController.EnemySearchState;
                }
            }

        }
        return this;
    }

    public void Enter(EnemyController enemy)
    {
        enemy.SoundNoticed = false;
        
        enemy.CurrentSoundStage = enemy.SoundBehaviourStage;
        
        if (enemy.SoundBehaviourStage == 1)
        {
            enemy.Agent.speed = enemy.FirstStageRunSpeed;
            enemy.AnimationHandler.SetSpeed(enemy.FirstStageRunSpeed);
            enemy.Agent.SetDestination(enemy.SoundEventPosition.position);
        }
        if (enemy.SoundBehaviourStage == 2)
        {
            enemy.Agent.speed = enemy.SecondStageRunSpeed;
            enemy.AnimationHandler.SetSpeed(enemy.SecondStageRunSpeed);
            enemy.Agent.SetDestination(enemy.SoundEventPosition.position);
        }
        if (enemy.SoundBehaviourStage == 3)
        {
            enemy.Agent.speed = enemy.ThirdStageRunSpeed;
            enemy.AnimationHandler.SetSpeed(enemy.ThirdStageRunSpeed);
            enemy.Agent.SetDestination(enemy.SoundEventPosition.position);
        }
    }

    public void Exit(EnemyController enemy)
    {
        enemy.AnimationActivated = false;
    }
    
    private void UpdateSearchStage(EnemyController enemy)
    {
        // when the player should use the same sound again, the stage will be increased and the enemy will be more aggressive
        if (enemy.CurrentSoundStage < enemy.SoundBehaviourStage)
        {
            if (enemy.SoundBehaviourStage == 2)
            {
                enemy.Agent.speed = enemy.SecondStageRunSpeed;
                enemy.AnimationHandler.SetSpeed(enemy.SecondStageRunSpeed);
                enemy.Agent.SetDestination(enemy.SoundEventPosition.position);
            }
            if (enemy.SoundBehaviourStage >= 3)
            {
                enemy.Agent.speed = enemy.ThirdStageRunSpeed;
                enemy.AnimationHandler.SetSpeed(enemy.ThirdStageRunSpeed);
                enemy.Agent.SetDestination(enemy.SoundEventPosition.position);
            }

            enemy.CurrentSoundStage = enemy.SoundBehaviourStage;
        }
    }
    
}
