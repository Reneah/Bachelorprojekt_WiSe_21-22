using Enemy.Controller;
using UnityEngine;

namespace Enemy.States
{
    public class EnemyVisionChaseState : IEnemyState
    {
        public IEnemyState Execute(EnemyController enemy)
        {
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
                    if (enemy.PathEndPosition(0.5f))
                    {
                        enemy.AnimationHandler.SetSpeed(0);
                    }
                    
                    enemy.Agent.SetDestination(enemy.Player.transform.position);
                }
                // if the enemy still doesn't see the player, the search mode will be activated 
                if (enemy.ReminderTime <= 0)
                {
                    if (enemy.PathEndPosition(0.5f))
                    {
                        enemy.ReminderTime = 1;
                        return EnemyController.EnemySearchState;
                    }
                }
            }
            
            if (enemy.CatchPlayer())
            {
                enemy.InGameMenu.EnemyCatchedPlayer = true;
                enemy.AnimationHandler.FinalHit();
                enemy.Player.PlayerAnimationHandler.PlayerDeath();
            }
            
            return this;
        }
    
        public void Enter(EnemyController enemy)
        {
            enemy.ReminderTime = enemy.LastChanceTime;
        }
    
        public void Exit(EnemyController enemy)
        {
            
        }
    }
}

