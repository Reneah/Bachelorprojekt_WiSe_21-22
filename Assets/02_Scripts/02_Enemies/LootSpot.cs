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
        [Tooltip("the cooldown when the enemy is able to loot the spot again")]
        [Range(5,60)]
        [SerializeField] private float _lootSpotCooldown;
        [Tooltip("the loot chance can't drop below the minimum loot chance")]
        [Range(0, 90)] 
        [SerializeField] private int _minimumLootChance;
        
        // the chance that the enemy will loot the spot
        private float _chanceToLoot;
        // the time, how long the enemy is looting at the spot
        private float _lootCooldown;
        // the cooldown when the enemy is able to loot the spot again
        private float _lootSpotTime;
        // reactivate the spot after a certain time
        private bool _reactivateLootSpot = false;

        // need this script to communicate with the current enemy nearby
        private EnemyController _enemyController;

        void Start()
        {
            _lootCooldown = _lootTime;
        }
        
        void Update()
        {
            // When the enemy reached the loot spot, the time will run how long the enemy will loot
            // Afterwards the variables will be reset
            if (_enemyController != null && _enemyController.ReachedLootSpot)
            {
                _lootCooldown -= Time.deltaTime;
                
                if (_lootCooldown <= 0)
                {
                    _enemyController.AnimationHandler.LootSpot(false);
                    EnemyShareInformation.IsLooting = false;
                    _enemyController.Loot = false;
                    _enemyController.ReachedLootSpot = false;
                    _lootCooldown = _lootTime;
                    _reactivateLootSpot = true;
                }
            }

            // when the spot is looted, the time will run to reactivate the loot spot
            if (_lootSpotTime >= 0 && _reactivateLootSpot)
            {
                _lootSpotTime -= Time.deltaTime;
                
                if (_lootSpotTime <= 0)
                {
                    _lootSpotTime = _lootSpotCooldown;
                    _reactivateLootSpot = false;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // When the enemy is in range, he has the chance to loot the spot
            if (other.CompareTag("Enemy"))
            {
                _chanceToLoot = Random.value;
                
                if (_chanceToLoot <= _lootChance && _lootChance > 0 && !EnemyShareInformation.IsLooting && !_reactivateLootSpot)
                {
                    _enemyController = other.GetComponent<EnemyController>();

                    // when the enemy is patrolling he is able to loot
                    if (!_enemyController.Patrolling)
                    {
                        return;
                    }
                    
                    _enemyController.Loot = true;
                    _enemyController.LootSpotTransform = transform;
                    _enemyController.SmoothRotation = _smoothRotation;
                    _enemyController.StopDistanceLootSpot = _stopDistance;
                    _lootChance -= _shrinkLootChance;
                    
                    // When the enemy ios looting, no other enemy will get to the spot as well
                    EnemyShareInformation.IsLooting = true;

                    //  the loot chance can't drop below the minimum loot chance or 0
                    if (_lootChance < _minimumLootChance)
                    {
                        _lootChance = _minimumLootChance;

                        if (_lootChance <= 0)
                        {
                            _lootChance = 0;
                        }
                    }
                }
            }
        }
    }
}
