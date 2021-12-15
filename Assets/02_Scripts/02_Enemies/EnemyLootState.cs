using Enemy.Controller;
using UnityEngine;

namespace Enemy.States
{
    public class EnemyLootState : IEnemyState
    {
        public IEnemyState Execute(EnemyController enemy)
        {
            // When the enemy has finished looting, he will go back in the patrol state
            if (!enemy.Loot)
            {
                enemy.Agent.isStopped = false;
                enemy.AnimationHandler.LootSpot(false);
                return EnemyController.EnemyPatrolState;
            }
            
            if (enemy.CanSeePlayer)
            {
                enemy.Agent.isStopped = false;
                enemy.AnimationHandler.LootSpot(false);
                return EnemyController.EnemyVisionChaseState;
            }

            if (enemy.SoundNoticed)
            {
                enemy.Agent.isStopped = false;
                enemy.AnimationHandler.LootSpot(false);
                return EnemyController.EnemySoundInvestigationState;
            }

            bool reachedLootSpot = Vector3.Distance(enemy.LootSpotTransform.position, enemy.transform.position) <= enemy.StopDistanceLootSpot;
            if (reachedLootSpot)
            {
                enemy.ReachedLootSpot = true;
                enemy.AnimationHandler.LootSpot(true);
                enemy.Agent.isStopped = true;
                
                // rotates the enemy towards the loot spot
                Quaternion _desiredDirection = Quaternion.Slerp(enemy.transform.rotation, enemy.LootSpotTransform.rotation, enemy.SmoothRotation * Time.deltaTime);
                enemy.transform.rotation = _desiredDirection;
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
