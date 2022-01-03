using Enemy.Controller;
using Enemy.ShareInformation;
using UnityEngine;
using UnityEngine.AI;
using untitledProject;

namespace Enemy.States
{
    public class EnemyVisionChaseState : IEnemyState
    {
        public IEnemyState Execute(EnemyController enemy)
        {
            // when the enemy is in chase mode, the spotted bar is permanently red
            enemy.SpottedBar.fillAmount = 1;
            // check if the player is on high or low ground to update the vision cone to help the enemy orientation
            enemy.CheckPlayerGround();
            
            if (enemy.CanSeePlayer)
            {
                enemy.ChasePlayer();
            }
            
            if (!enemy.CanSeePlayer)
            {
                enemy.LastChanceTime -= Time.deltaTime;

                if (enemy.LastChanceTime > 0)
                {
                    enemy.Agent.SetDestination(enemy.Player.transform.position);
                }
                
                // if the enemy still doesn't see the player, the search mode will be activated 
                // if the max amount of search enemies is reached, they will go into their main routine
                if (enemy.LastChanceTime <= 0)
                {
                    if (!enemy.Agent.hasPath || Vector3.Distance(enemy.Agent.pathEndPosition, enemy.Agent.destination) <= 1)
                    {
                        if (enemy.SearchArea.EnemySearchAmount < enemy.SearchArea.EnemySearchMaxAmount)
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

            // when the player is caught, the game will stop and the death menu pops up
            if (enemy.CatchPlayer())
            {
                enemy.InGameMenu.EnemyCatchedPlayer = true;
                enemy.AnimationHandler.FinalHit();
                enemy.Player.PlayerAnimationHandler.PlayerDeath();
                enemy.GetComponent<EnemyController>().enabled = false;
                enemy.AnimationHandler.enabled = false;
                enemy.EnemyTalkCheck.enabled = false;
                enemy.Agent.isStopped = true;
            }
            
            return this;
        }
    
        public void Enter(EnemyController enemy)
        {
            enemy.SoundNoticed = false;
            enemy.InChaseState = true;
            enemy.AbleToLoot = false;
            
            enemy.PullEnemyNearby();

            enemy.Agent.isStopped = false;
            
            enemy.ActivateChasing = false;
            enemy.PlayerSoundSpotted = false;
        }
    
        public void Exit(EnemyController enemy)
        {
            enemy.ActivateChasing = false;
            enemy.PlayerSoundSpotted = false;
            
            enemy.InChaseState = false;
            enemy.SoundNoticed = false;
            
            enemy.Agent.isStopped = false;
            
            enemy.HighGroundViewCone.SetActive(false);
            enemy.LowGroundViewCone.SetActive(true);
            
            enemy.PlayerSpotted = false;
            enemy.UseSpottedBar = false;
        }
    }
}

