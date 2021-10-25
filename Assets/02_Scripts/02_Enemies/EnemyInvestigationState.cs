using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInvestigationState : IEnemyState
{
    public IEnemyState Execute(EnemyController enemy)
    {

        return this;
    }

    public void Enter(EnemyController enemy)
    {
        if (enemy.SoundBehaviourStage == 1)
        {
            //set destination of the sound event
            // play the investigation animation
            // go back to patrol
            enemy.Agent.SetDestination(enemy.Player.transform.position);
        }
        if (enemy.SoundBehaviourStage == 2)
        {
            //set destination of the sound event
            // play the investigation animation
            // search a little bit around
        }
        if (enemy.SoundBehaviourStage == 3)
        {
            //set destination of the sound event
            // play the investigation animation
            // search agressive around
        }
    }

    public void Exit(EnemyController enemy)
    {
        
    }
}
