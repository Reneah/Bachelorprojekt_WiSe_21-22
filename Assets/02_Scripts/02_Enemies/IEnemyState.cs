using Enemy.Controller;

namespace Enemy.States
{
    public interface IEnemyState
    {
        IEnemyState Execute(EnemyController enemy);
        void Enter(EnemyController enemy);
        void Exit(EnemyController enemy);
    }
}

