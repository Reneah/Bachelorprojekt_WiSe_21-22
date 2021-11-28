using Enemy.Controller;

namespace Enemy.States
{
    public class EnemyIdleState : IEnemyState
    {
        public IEnemyState Execute(EnemyController enemy)
        {
            if (enemy.Patrolling)
            {
                return EnemyController.EnemyPatrolState;
            }
        
            if (enemy.Guarding)
            {
                return EnemyController.EnemyGuardState;
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
}

