using System;
using System.Collections;
using System.Collections.Generic;
using Enemy.Controller;
using Enemy.ShareInformation;
using UnityEngine;

namespace Enemy.ChaseActivation
{
    public class ChaseActivation : MonoBehaviour
    {
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Enemy") && EnemyShareInformation.PlayerLocalized)
            {
                Debug.Log("yes");
                other.GetComponent<EnemyController>().ActivateChasing = true;
            }
        }
    }
}
