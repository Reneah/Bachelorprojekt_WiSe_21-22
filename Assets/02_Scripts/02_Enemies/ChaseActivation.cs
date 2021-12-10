using Enemy.Controller;
using UnityEngine;

namespace Enemy.ChaseActivation
{
    public class ChaseActivation : MonoBehaviour
    {
        // when this game object is activated and a other enemy is nearby, he will be chase the player as well
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                other.GetComponent<EnemyController>().ActivateChasing = true;
            }
        }
    }
}
