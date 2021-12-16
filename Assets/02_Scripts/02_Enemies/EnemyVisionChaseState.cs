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
                enemy.ReminderTime = enemy.LastChanceTime;
                enemy.ChasePlayer();
            }
            
            if (!enemy.CanSeePlayer)
            {
                enemy.ReminderTime -= Time.deltaTime;
    
                if (enemy.ReminderTime > 0)
                {
                    // prevent that the run animation is playing when the agent can't go further in contrast to the player
                    if (enemy.ClosestPlayerPosition(0.5f))
                    {
                        enemy.AnimationHandler.SetSpeed(0);
                    }
                    
                    enemy.Agent.SetDestination(enemy.Player.transform.position);
                }
                // if the enemy still doesn't see the player, the search mode will be activated 
                if (enemy.ReminderTime <= 0)
                {
                    if (enemy.ClosestPlayerPosition(0.5f))
                    {
                        enemy.ReminderTime = enemy.LastChanceTime;
                        return EnemyController.EnemySearchState;
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
                enemy.GetComponent<NavMeshAgent>().enabled = false;
            }
            
            return this;
        }
    
        public void Enter(EnemyController enemy)
        {
            enemy.EnemyTalkCheck.Talkable = false;
            enemy.ChaseActivationObject.SetActive(true);
            
            enemy.ReminderTime = enemy.LastChanceTime;
            enemy.Agent.isStopped = false;
        }
    
        public void Exit(EnemyController enemy)
        {
            
        }
    }
}

