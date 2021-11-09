using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGuardState : IEnemyState
{
    private bool _reachedGuardpoint = false;

    private Quaternion _desiredDirection;
    
    public IEnemyState Execute(EnemyController enemy)
    {
        if (enemy.CanSeePlayer)
        {
            return EnemyController.EnemyChaseState;
        }

        if (enemy.SoundNoticed)
        {
            return EnemyController.EnemySoundInvestigationState;
        }
        
        if (Vector3.Distance(enemy.transform.position, enemy.GuardPoint.transform.position) <= enemy.StopGuardpointDistance)
        {
            _reachedGuardpoint = true;
            enemy.AnimationHandler.SetSpeed(0);
            enemy.GuardBehaviour = true;
        }
        
        if (_reachedGuardpoint)
        {
            _desiredDirection = Quaternion.Slerp(enemy.transform.rotation, enemy.DesiredBodyRotation.rotation, enemy.SmoothBodyRotation * Time.deltaTime);
            enemy.transform.rotation = _desiredDirection;
             
            enemy.UpdateGuardBehaviour();
        }

        return this;
    }

    public void Enter(EnemyController enemy)
    {

        // only when the enemy enters the patrol or guard mode, the enemy will stop to see the player instantly, because he lost the orientation of him
        enemy.SpottedTime = 0;
        enemy.PlayerSpotted = false;
        
        enemy.Agent.SetDestination(enemy.GuardPoint.transform.position);
        enemy.AnimationHandler.SetSpeed(enemy.PatrolSpeed);
        
    }

    public void Exit(EnemyController enemy)
    {
        enemy.AnimationHandler.HeadRotationWeight = 0;
        enemy.GuardBehaviour = false;
        _reachedGuardpoint = false;
    }
}
