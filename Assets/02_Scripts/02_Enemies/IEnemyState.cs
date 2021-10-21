using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyState
{
    IEnemyState Execute(EnemyController player);
    void Enter(EnemyController player);
    void Exit(EnemyController player);
}
