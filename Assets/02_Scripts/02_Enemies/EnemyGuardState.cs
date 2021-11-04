using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGuardState : IEnemyState
{
    private bool _reachedGuardpoint = false;
    
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
            
            enemy.StartGuardBehaviour();
            enemy.AnimationHandler.SetSpeed(0);
            enemy.GuardBehaviour = true;
            return this;
        }

        if (_reachedGuardpoint)
        {
            Quaternion desiredDirection = Quaternion.Slerp(enemy.transform.rotation, Quaternion.LookRotation(enemy.DesiredBodyRotation.position), enemy.SmoothBodyRotation * Time.deltaTime);
            desiredDirection.y = 0;
            enemy.transform.rotation = desiredDirection;
            
            enemy.UpdateGuardBehaviour();
        }

        return this;
    }

    public void Enter(EnemyController enemy)
    {
        // only when the enemy enters the patrol or guard mode, the enemy will stop to see the player instantly, because he lost the orientation of him
        enemy.SpottedTime = 0;
        enemy.PlayerSpotted = false;
        
        enemy.Agent.SetDestination(enemy.CurrentLookpoint.transform.position);
        enemy.AnimationHandler.SetSpeed(enemy.PatrolSpeed);

    }

    public void Exit(EnemyController enemy)
    {
        enemy.GuardBehaviour = false;
        _reachedGuardpoint = false;
    }
}
