using Enemy.Controller;
using Enemy.States;
using UnityEngine;

namespace Enemy.States
{
    public class EnemyLootState : IEnemyState
    {
        public IEnemyState Execute(EnemyController enemy)
        {
            if (enemy.CanSeePlayer)
            {
                return EnemyController.EnemyVisionChaseState;
            }

            if (enemy.SoundNoticed)
            {
                return EnemyController.EnemySoundInvestigationState;
            }

            bool reachedLootSpot = Vector3.Distance(enemy.LootSpotTransform.position, enemy.transform.position) <= enemy.StopDistanceLootSpot;
            if (reachedLootSpot)
            {
                enemy.ReachedLootSpot = true;
                enemy.AnimationHandler.LootSpot(true);
                enemy.Agent.isStopped = true;
                
                // rotates the enemy towards the loot spot
                Quaternion _desiredDirection = Quaternion.Slerp(enemy.transform.rotation, Quaternion.LookRotation(enemy.LootSpotTransform.position - enemy.transform.position), enemy.SmoothRotation * Time.deltaTime);
                enemy.transform.rotation = _desiredDirection;
            }
            
            // When the enemy has finished looting, he will go back in the patrol state
            if (!enemy.Loot)
            {
                enemy.Agent.isStopped = false;
                enemy.AnimationHandler.LootSpot(false);
                return EnemyController.EnemyPatrolState;
                
        }
            
            return this;
        }

        public void Enter(EnemyController enemy)
        {
            enemy.Agent.SetDestination(enemy.LootSpotTransform.position);
        }

        public void Exit(EnemyController enemy)
        {
           
        }
    }
}
