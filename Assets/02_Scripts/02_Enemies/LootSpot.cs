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
        [Range(1,15)]
        [SerializeField] private float _lootTime;
        [Tooltip("smooth the enemy rotation towards the loot location")]
        [Range(1,10)]
        [SerializeField] private float _smoothRotation;
        [Tooltip("the distance to stop in front of the loot spot")]
        [Range(1,5)]
        [SerializeField] private float _stopDistance;
        
        private float _chanceToLoot;
        private float _lootCooldown;

        private EnemyController _enemyController;

        void Start()
        {
            _lootCooldown = _lootTime;
        }
        
        void Update()
        {
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
            if (other.CompareTag("Enemy"))
            {
                _chanceToLoot = Random.value;

                if (_chanceToLoot <= _lootChance && _lootChance > 0 && !EnemyShareInformation.IsLooting)
                {
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
