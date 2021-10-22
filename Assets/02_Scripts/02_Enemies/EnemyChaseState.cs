using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using untitledProject;

public class EnemyChaseState : IEnemyState
{
    // give the enemy the chance to chase the player again
    // otherwise at a quick turn of the player, the enemy can't see him anymore, but should have an awareness that the player is next to or behind him. That is more realistic
    // maybe just use the rotation to the player, otherwise the position of the player will be instantly set, although he can't see the player
    private float _reminderTime = 2;
    
    public IEnemyState Execute(EnemyController enemy)
    {
        if (enemy.CanSeePlayer)
        {
            _reminderTime = 2;
            enemy.HeadRotationTowardsPlayer();
            enemy.ChasePlayer();
        }

        if (!enemy.CanSeePlayer)
        {
            enemy.RotateToPlayer();
            _reminderTime -= Time.deltaTime;

            if (_reminderTime <= 0)
            {
                _reminderTime = 2;
                return EnemyController.EnemySearchState;
            }
            // set short reminder of the player like the last position
            // if the enemy still doesn't see the player, the search mode will be activated 
        }
   
        
        if (enemy.CatchPlayer() && enemy.CanSeePlayer)
        {
            Debug.Log("GAME OVER");
            enemy.Player.gameObject.SetActive(false);
        }
        
        return this;
    }

    public void Enter(EnemyController enemy)
    {
        
    }

    public void Exit(EnemyController enemy)
    {
        
    }
}
