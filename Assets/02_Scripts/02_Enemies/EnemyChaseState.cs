using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using untitledProject;

public class EnemyChaseState : IEnemyState
{
    // give the enemy the chance to chase the player again
    // otherwise at a quick turn of the player, the enemy can't see him anymore, but should have an awareness that the player is next to or behind him. That is more realistic
    // maybe just use the rotation to the player, otherwise the position of the player will be instantly set, although he can't see the player
    private float _reminderTime = 0;
    
    public IEnemyState Execute(EnemyController enemy)
    {
        if (enemy.CanSeePlayer)
        {
            _reminderTime = enemy.LastChanceTime;
            enemy.HeadRotationTowardsPlayer();
            enemy.ChasePlayer();
        }

        if (!enemy.CanSeePlayer)
        {
            
            _reminderTime -= Time.deltaTime;

            if (_reminderTime > 0)
            {
                enemy.Agent.SetDestination(enemy.Player.transform.position);
            }
            
            // if the enemy still doesn't see the player, the search mode will be activated 
            if (_reminderTime <= 0)
            {
                if (Vector3.Distance(enemy.transform.position, enemy.Agent.pathEndPosition) <= 0.5f)
                {
                    _reminderTime = 1;
                    return EnemyController.EnemySearchState;
                }
            }
        }
        
        if (enemy.CatchPlayer())
        {
            enemy.Player.GetComponent<Death>().EnemyCatchedPlayer = true;
            enemy.AnimationHandler.FinalHit();
            enemy.Player.gameObject.SetActive(false);
        }
        
        return this;
    }

    public void Enter(EnemyController enemy)
    {
        _reminderTime = enemy.LastChanceTime;
    }

    public void Exit(EnemyController enemy)
    {
        
    }
}
