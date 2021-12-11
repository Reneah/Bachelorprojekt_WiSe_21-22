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

                // there is only one player in the game, so the array can be set to 0
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
                }
                
                if (!obstructedView)
                {
                    _enemyController.UseSpottedBar = true;
                }
            
                // only sees the player when the time is 0 to spott the player in the view field
                // or if the player is on high ground the enemy is able to follow him
                if(!obstructedView && _enemyController.PlayerSpotted || _enemyController.Player.HighGround)
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
            }
        }
    }
}

