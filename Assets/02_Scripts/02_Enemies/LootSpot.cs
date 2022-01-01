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
        [SerializeField]private float _lootChance;
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
        [Tooltip("when the chest for looting will be used")]
        [SerializeField] private bool _usingChest;
        
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

        // need the script to activate the chest animation
        private LootChestAnimationController _lootChestAnimation;
        
        // call the chest animation one time
        private bool _chestAnimation = false;

        // determines if the spot is occupied or not so that no other enemy can loot in the moment
        private bool _occupied = false;
        
        void Start()
        {
            if (_usingChest)
            {
                _lootChestAnimation = GetComponentInChildren<LootChestAnimationController>();
            }
            
            _lootCooldown = _lootTime;
            _lootSpotTime = _lootSpotCooldown;
        }
        
        void Update()
        {
            if (_enemyController != null && _enemyController.ResetLootVariables)
            {
                _enemyController.Loot = false;
                _enemyController.ReachedLootSpot = false;
                _reactivateLootSpot = false;
                _occupied = false;
                _lootCooldown = _lootTime;
                _enemyController.SpottedAcousticDistance = _enemyController.DetectionAcousticDistance;
                _enemyController.AcousticTimeToSpot = _enemyController.AcousticSecondsToSpot;
                
                if (_usingChest)
                {
                    _lootChestAnimation.OpenChest(false);
                    _chestAnimation = false;
                }

                _enemyController.ResetLootVariables = false;
                _lootSpotTime = _lootSpotCooldown;
                return;
            }
            
            // When the enemy reached the loot spot, the time will run how long the enemy will loot
            // Afterwards the variables will be reset
            if (_enemyController != null && _enemyController.ReachedLootSpot)
            {
                _lootCooldown -= Time.deltaTime;
                if (!_chestAnimation && _usingChest)
                {
                    _lootChestAnimation.OpenChest(true);
                    _chestAnimation = true;
                }
                
                if (_lootCooldown <= 0)
                {
                    _reactivateLootSpot = true;
                    _enemyController.AnimationHandler.LootSpot(false);
                    _enemyController.Loot = false;
                    _lootCooldown = _lootTime;
                    _enemyController.SpottedAcousticDistance = _enemyController.DetectionAcousticDistance;
                    _enemyController.AcousticTimeToSpot = _enemyController.AcousticSecondsToSpot;

                    if (_usingChest)
                    {
                        _lootChestAnimation.OpenChest(false);
                        _chestAnimation = false;
                    }

                    _occupied = false;
                    _enemyController.ReachedLootSpot = false;
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
            if (other.CompareTag("Enemy") && !_reactivateLootSpot && !_occupied)
            {
                _chanceToLoot = Random.value;
                
                if (_chanceToLoot <= _lootChance / 100 && _lootChance > 0)
                {
                    _enemyController = other.GetComponent<EnemyController>();
                    
                    // when the enemy is a patrolling enemy and is patrolling, he is able to loot
                    if (!_enemyController.Patrolling || !_enemyController.AbleToLoot)
                    {
                        return;
                    }
                    
                    _enemyController.Loot = true;
                    
                    // when the enemy is looting, he will concentrate on the looting and detect the enemy only in a small distance and longer time
                    _enemyController.SpottedAcousticDistance = 1;
                    _enemyController.AcousticTimeToSpot = 3;

                    _enemyController.LootSpotTransform = transform;
                    _enemyController.SmoothRotation = _smoothRotation;
                    _enemyController.StopDistanceLootSpot = _stopDistance;
                    _lootChance -= _shrinkLootChance;
                    
                    //  the loot chance can't drop below the minimum loot chance or 0
                    if (_lootChance < _minimumLootChance)
                    {
                        _lootChance = _minimumLootChance;

                        if (_lootChance <= 0)
                        {
                            _lootChance = 0;
                        }
                    }
                    _occupied = true;
                }
            }
        }
    }
}
