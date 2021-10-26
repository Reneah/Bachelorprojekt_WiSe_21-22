using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNearDetection : MonoBehaviour
{
    private EnemyController _enemyController;
    
    void Start()
    {
        _enemyController = GetComponentInParent<EnemyController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            float _distance = Vector3.Distance(transform.position, other.transform.position);
            
            RaycastHit hit;
            Physics.Raycast(other.transform.position, transform.position - other.transform.position, out hit, _distance);

            if (hit.collider.CompareTag("Wall"))
            {
                return;
            }

            _enemyController.CanSeePlayer = true;
        }
    }
}
