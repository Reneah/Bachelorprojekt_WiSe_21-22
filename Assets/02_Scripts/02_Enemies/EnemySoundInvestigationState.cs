using Enemy.Controller;
using Enemy.ShareInformation;
using UnityEngine;

namespace Enemy.States
{
    public class EnemySoundInvestigationState : IEnemyState
    {
        public IEnemyState Execute(EnemyController enemy)
        {
            // when the enemy is able to pull other enemies, the cooldown is running to deactivate the mechanic
            if (enemy.ActivateChasing)
            {
                enemy.ActivateChaseCooldown -= Time.deltaTime;

                if (enemy.ActivateChaseCooldown <= 0)
                {
                    enemy.ActivateChasing = false;
                    enemy.ActivateChaseCooldown = 0.1f;
                }
            }
            
            if (enemy.CanSeePlayer)
            {
                enemy.AnimationHandler.FinishedInvestigationAnimation = false;
                enemy.AnimationHandler.FinishedLookingAnimation = false;
                enemy.AnimationHandler.ResetInvestigatePoint();
                enemy.AnimationHandler.ResetLookingAround();
                
                return EnemyController.EnemyVisionChaseState;
            }
            
            UpdateSearchStage(enemy);
            enemy.DistanceToSoundEvent();
            
            if (enemy.DistanceToSoundEvent() <= 1 || EnemyShareInformation.ReachedNoisyItem)
            {
                EnemyShareInformation.ReachedNoisyItem = true;
                
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
                        
                        return EnemyController.EnemyNoisyItemSearchState;
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
                        
                        return EnemyController.EnemyNoisyItemSearchState;
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
                        
                        if (enemy.SearchArea.EnemySearchAmount < 2)
                        {
                            return EnemyController.EnemySearchState;
                        }
                        else
                        {
                            if (enemy.Guarding)
                            {
                                return EnemyController.EnemyGuardState;
                            }
                            else if (enemy.Patrolling)
                            {
                                return EnemyController.EnemyPatrolState;
                            }
                        }
                    }
                }
            }
            return this;
        }

        public void Enter(EnemyController enemy)
        {
            enemy.GetSoundOnce = false;
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
            EnemyShareInformation.ReachedNoisyItem = false;

            enemy.SoundNoticed = false;
            
            enemy.HighGroundViewCone.SetActive(false);
            enemy.LowGroundViewCone.SetActive(true);
        }
        
        private void UpdateSearchStage(EnemyController enemy)
        {
            // when the player should use the same sound again, the stage will be increased and the enemy will be more aggressive
            // when the footsteps of the player were heard, the destination will be updated
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
}

