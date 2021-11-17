using System.Collections;
using System.Collections.Generic;
using System.Net.Configuration;
using UnityEngine;

public class EnemySoundInvestigationState : IEnemyState
{
    // prevent that the animation will be activated permanently in Update
    private bool _animationActivated = false;

    // the current sound state of the item to update the behaviour of the enemy
    private int _currentSoundStage = 0;
    
    public IEnemyState Execute(EnemyController enemy)
    {
        Debug.Log(_currentSoundStage);
        Debug.Log(enemy.SoundBehaviourStage);
        // when the player should use the same sound again, the stage will be increased and the enemy will be more aggressive
        if (_currentSoundStage < enemy.SoundBehaviourStage)
        {
            
            if (enemy.SoundBehaviourStage <= 2)
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

            _currentSoundStage = enemy.SoundBehaviourStage;
        }
        
        if (enemy.CanSeePlayer)
        {
            enemy.AnimationHandler.FinishedInvestigationAnimation = false;
            enemy.AnimationHandler.FinishedLookingAnimation = false;
            enemy.AnimationHandler.ResetInvestigatePoint();
            enemy.AnimationHandler.ResetLookingAround();
            
            return EnemyController.EnemyChaseState;
        }
        
        // the distance between the sound event and the enemy
        float distance = Vector3.Distance(enemy.SoundEventPosition.position, enemy.transform.position);

        if (distance <= 1)
        {
            // prevent that the walking animation will be played
            enemy.AnimationHandler.SetSpeed(0);
            
            if (enemy.SoundBehaviourStage == 1)
            {
                // plays the investigation animation & when the Animation is finished, the enemy will patrol again
                if (!_animationActivated)
                {
                    enemy.AnimationHandler.InvestigatePoint();
                    _animationActivated = true;
                }
                
                if (enemy.AnimationHandler.FinishedInvestigationAnimation)
                {
                    enemy.AnimationHandler.FinishedInvestigationAnimation = false;
                    enemy.AnimationHandler.ResetInvestigatePoint();
                    
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
                if (!_animationActivated)
                {
                    enemy.AnimationHandler.InvestigatePoint();
                    enemy.AnimationHandler.LookingAround();
                    _animationActivated = true;
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
                if (!_animationActivated)
                {
                    enemy.AnimationHandler.InvestigatePoint();
                    _animationActivated = true;
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
        _currentSoundStage = enemy.SoundBehaviourStage;
        
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
        _animationActivated = false;
    }
}
