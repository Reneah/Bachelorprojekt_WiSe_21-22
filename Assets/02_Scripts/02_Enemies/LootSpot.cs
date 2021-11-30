using System;
using System.Collections;
using System.Collections.Generic;
using Enemy.Controller;
using Enemy.ShareInformation;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy.LootSpot
{
    public class LootSpot : MonoBehaviour
    {
        [Tooltip("the chance that the enemy will loot the spot")]
        [Range(0,100)]
        [SerializeField]private int _lootChance;
        [Tooltip("Every time the spot is looted, the chance will shrink to loot the place again")]
        [Range(1,100)]
        [SerializeField] private int _shrinkLootChance;
        [Tooltip("the time, how long the enemy is looting at the spot")]
        [Range(1,15)]
        [SerializeField] private float _lootTime;
        [Tooltip("smooth the enemy rotation towards the loot location")]
        [Range(1,10)]
        [SerializeField] private float _smoothRotation;
        [Tooltip("the distance to stop in front of the loot spot")]
        [Range(1,5)]
        [SerializeField] private float _stopDistance;
        
        // the chance that the enemy will loot the spot
        private float _chanceToLoot;
        // the time, how long the enemy is looting at the spot
        private float _lootCooldown;

        // need this script to communicate with the current enemy nearby
        private EnemyController _enemyController;

        void Start()
        {
            _lootCooldown = _lootTime;
        }
        
        void Update()
        {
            // When the enemy reached the loot spot, the time will run how long the enemy will loot
            // Afterwards the variables will be resetted
            if (_enemyController.ReachedLootSpot)
            {
                _lootCooldown -= Time.deltaTime;
                
                if (_lootCooldown <= 0)
                {
                    _enemyController.AnimationHandler.LootSpot(false);
                    EnemyShareInformation.IsLooting = false;
                    _enemyController.Loot = false;
                    _enemyController.ReachedLootSpot = false;
                    _lootCooldown = _lootTime;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // When the enemy is in range, he has the chance to loot the spot
            if (other.CompareTag("Enemy"))
            {
                _chanceToLoot = Random.value;
                
                if (_chanceToLoot <= _lootChance && _lootChance > 0 && !EnemyShareInformation.IsLooting)
                {
                    // When the enemy ios looting, no other enemy will get to the spot as well
                    EnemyShareInformation.IsLooting = true;
                    
                    _enemyController = other.GetComponent<EnemyController>();
                    _enemyController.Loot = true;
                    _enemyController.LootSpotTransform = transform;
                    _enemyController.SmoothRotation = _smoothRotation;
                    _enemyController.StopDistanceLootSpot = _stopDistance;
                    _lootChance -= _shrinkLootChance;
                }
            }
        }
    }
}
