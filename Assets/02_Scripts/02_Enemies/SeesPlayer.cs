using Enemy.Controller;
using UnityEngine;

namespace Enemy.ViewField
{
    public class SeesPlayer : MonoBehaviour
    {
        [SerializeField] private EnemyController _enemyController;
    
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // the raycast target on the player
                Vector3 target = other.transform.position;
                target = new Vector3 (target.x, target.y + 0.5f, target.z);
            
                // the direction from the enemy to the player
                Vector3 directionToTarget = (target - _enemyController.EnemyHead.position).normalized;
            
                // the distance from the enemy to the player
                float distanceToTarget = Vector3.Distance(_enemyController.EnemyHead.position, target);
                
                // check if there is a obstacle in the way to see the player 
                bool obstructedView = Physics.Raycast(_enemyController.ObstacleRaycastTransform.position, directionToTarget, distanceToTarget, _enemyController.ObstructionMask);
            
                if (obstructedView)
                {
                    _enemyController.CanSeePlayer = false; 
                    _enemyController.UseSpottedBar = false;
                    _enemyController.PlayerInViewField = false;
                }
                
                if (!obstructedView)
                {
                    // start to use the bar to spot the player
                    _enemyController.UseSpottedBar = true;
                    
                    _enemyController.PlayerInViewField = true;
                }
            
                // only sees the player when the spotted bar is filled up
                // or if the player is on high ground and spotted the enemy is able to follow him
                if(!obstructedView && _enemyController.PlayerSpotted || _enemyController.PlayerGroundDetection.HighGround && _enemyController.PlayerSpotted)
                {
                    Debug.DrawRay(_enemyController.ObstacleRaycastTransform.position, directionToTarget * distanceToTarget, Color.green);
                    _enemyController.CanSeePlayer = true; 
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _enemyController.CanSeePlayer = false;
                _enemyController.UseSpottedBar = false;
                _enemyController.PlayerInViewField = false;
            }
        }
    }
}

