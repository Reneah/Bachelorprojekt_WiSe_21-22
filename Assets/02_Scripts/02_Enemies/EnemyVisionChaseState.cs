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
            enemy.SpottedBar.fillAmount = 1;
            // when the enemy is able to pull other enemies, the cooldown is running to deactivate the mechanic
            if (enemy.ChaseActivationObject.activeInHierarchy)
            {
                enemy.ActivateChaseCooldown -= Time.deltaTime;

                if (enemy.ActivateChaseCooldown <= 0)
                {
                    enemy.ActivateChasing = false;
                    enemy.ActivateChaseCooldown = 0.1f;
                    enemy.ChaseActivationObject.SetActive(false);
                }
            }
            
            enemy.CheckPlayerGround();
            
            if (enemy.CanSeePlayer)
            {
                enemy.ChasePlayer();
            }
            
            if (!enemy.CanSeePlayer)
            {
                enemy.LastChanceTime -= Time.deltaTime;
                Debug.Log(enemy.LastChanceTime);
                
                if (enemy.LastChanceTime > 0)
                {
                    enemy.Agent.SetDestination(enemy.Player.transform.position);
                }
                // if the enemy still doesn't see the player, the search mode will be activated 
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
            // when the enemy will be pulled of another one, the enemy should not go instantly into the search mode. Should have the chance to follow the player
            if (enemy.ActivateChasing)
            {
               // enemy.LastChanceTime = 5;
               // enemy.PlayerSpotted = true;
            }
            
            enemy.SoundNoticed = false;
            enemy.InChaseState = true;
            
            enemy.EnemyTalkCheck.Talkable = false;
            enemy.ChaseActivationObject.SetActive(true);

            enemy.Agent.isStopped = false;
        }
    
        public void Exit(EnemyController enemy)
        {
            enemy.InChaseState = false;
            enemy.SoundNoticed = false;
            
            enemy.PlayerSpotted = false;
            enemy.UseSpottedBar = false;
            
            enemy.Agent.isStopped = false;
            
            enemy.HighGroundViewCone.SetActive(false);
            enemy.LowGroundViewCone.SetActive(true);
        }
    }
}

