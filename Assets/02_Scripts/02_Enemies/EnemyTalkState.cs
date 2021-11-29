using Enemy.Controller;
using UnityEngine;

namespace Enemy.States
{
    public class EnemyTalkState : IEnemyState
    {
        public IEnemyState Execute(EnemyController enemy)
        {
            if (!enemy.EnemyTalkCheck.Talkable)
            {
                if (enemy.Patrolling)
                {
                    return EnemyController.EnemyPatrolState;
                }
                
                if (enemy.Guarding)
                {
                    return EnemyController.EnemyGuardState;
                }
              
            }
            return this;
        }

        public void Enter(EnemyController enemy)
        {
            
        }

        public void Exit(EnemyController enemy)
        {
            enemy.Agent.isStopped = false;
        }
    }
}
