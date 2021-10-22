using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyState
{
    IEnemyState Execute(EnemyController enemy);
    void Enter(EnemyController enemy);
    void Exit(EnemyController enemy);
}
