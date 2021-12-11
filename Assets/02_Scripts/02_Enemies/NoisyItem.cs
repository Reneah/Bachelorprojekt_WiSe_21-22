using System;
using System.Collections.Generic;
using System.Linq;
using DarkTonic.MasterAudio;
using Enemy.Controller;
using TMPro;
using UnityEngine;

namespace Enemy.SoundItem
{
    public class NoisyItem : MonoBehaviour
    { 
        [Header("Item")]
        [Tooltip("the item sprite that will show up when the player is in range, hovers over the item and when it is available")]
        [SerializeField] private GameObject _collectibleSprite;
        [Tooltip("the item sprite that will show up when the player hovers over the item and it is not available")]
        [SerializeField] private GameObject _negativeSprite;
        [Tooltip("mark the close range")]
        [SerializeField] private GameObject _closeActivationRadius;
        [Tooltip("is the noisy item reusable or not")]
        [SerializeField] private bool _reusable;
        [Tooltip("the offset of the noisy item origin so that the enemy is able to reach the item")]
        [SerializeField] private GameObject _offsetOrigin;
        [Tooltip("the key to save the used status of the item")]
        [SerializeField] private string _playerPrefsKey;

        public GameObject OffsetOrigin
        {
            get => _offsetOrigin;
            set => _offsetOrigin = value;
        }
        
        [Header("Sound Stage")]
        [Tooltip("First Stage: the enemy will walk to the point of interest" +
                 " Second Stage: the enemy will run to the point of interest" +
                 " Third Stage: the enemy runs to the point of interest, knows that the player is nearby and start searching")]
        [Range(1,3)]
        [SerializeField] private int _stage;

        // [Tooltip("modify the text position at the mouse position")]
        private Vector2 _textOffset;

        [Header("Sound Collider")]
        [Tooltip("the collider, which shows the sound range of the item")]
        [SerializeField] private GameObject _soundRangeCollider;
        
        // deactivate the sound collider after a fixed time
        private float _deactivationTime = 0.1f;

        public GameObject SoundRangeCollider
        {
            get => _soundRangeCollider;
            set => _soundRangeCollider = value;
        }

        private PlayerThrowTrigger _playerThrowTrigger;

        // when the player is able to reuse the sound, the alert stage of the enemy rise
        private bool _oneTimeUsed = false;
        
        [Tooltip("the waypoints the enemy will run down when the player activating the item in close distance")]
        [SerializeField] private Transform[] _closeNoisyItemWaypoints;
        [Tooltip("the max pull amount for the noisy item")]
        [SerializeField] private float _enemyPullAmount;

         // the final closest enemy distance to the noisy item
         private float _closestEnemyDistance;
         // the max pull amount for the noisy item
         private float _enemyAmountToPull;
         // the current closest enemy distance to the noisy item
        private float _currentclosestEnemyDistance;
        // need this script to activate the noisy item investigation
        private EnemyController _closestEnemy;
        // the enemies who heard the noisy item
        private List<EnemyController> _enemyList = new List<EnemyController>();
        // the cooldown how long to wait to get all enemies in sound range. Otherwise the first enemy would start the method and no one else could be added to investigate
        private float _pullEnemyCooldown = 0.1f;

        public List<EnemyController> EnemyList
        {
            get => _enemyList;
            set => _enemyList = value;
        }
        
        public Transform[] CloseNoisyItemWaypoints
        {
            get => _closeNoisyItemWaypoints;
            set => _closeNoisyItemWaypoints = value;
        }

        public bool OneTimeUsed
        {
            get => _oneTimeUsed;
            set => _oneTimeUsed = value;
        }

        public int Stage
        {
            get => _stage;
            set => _stage = value;
        }
        
        private bool _itemUsed = false;

        public bool ItemUsed
        {
            get => _itemUsed;
            set => _itemUsed = value;
        }

        private bool _itemUsable = false;

        private bool _safeState = false;

        public bool SafeState
        {
            get => _safeState;
            set => _safeState = value;
        }

        // start the pull countdown to be certain to get all enemies in the sound range
        private bool startPullCountdown = false;

        public bool StartPullCountdown
        {
            get => startPullCountdown;
            set => startPullCountdown = value;
        }

        void Start()
        {
            _itemUsed = System.Convert.ToBoolean(PlayerPrefs.GetInt(_playerPrefsKey, 0));
            
            _collectibleSprite.gameObject.SetActive(false);
            _playerThrowTrigger = FindObjectOfType<PlayerThrowTrigger>();
        }

        private void Update()
        {
            _textOffset.x = 50;
            _textOffset.y = -40;
            _collectibleSprite.transform.position = new Vector3(_textOffset.x, _textOffset.y, 0) + Input.mousePosition;
            _negativeSprite.transform.position = new Vector3(_textOffset.x, _textOffset.y, 0) + Input.mousePosition;

            if (_safeState)
            {
                PlayerPrefs.SetInt(_playerPrefsKey, _itemUsed.GetHashCode());
                _safeState = false;
            }
            
            ItemActivation();
            ItemExecution();

            PullCountdown();
        }

        private void ItemActivation()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit _hit;

            if (Physics.Raycast(ray, out _hit, Mathf.Infinity, LayerMask.GetMask("NoisyItem")) && _playerThrowTrigger.Close)
            {
                if (_itemUsed && !_itemUsable)
                {
                    _negativeSprite.gameObject.SetActive(true);
                }
                
                else if (!_itemUsed && _itemUsable)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        _playerThrowTrigger.PlayerThrew = false;
                        if (_oneTimeUsed)
                        {
                            _stage++;

                            if (_stage >= 3)
                            {
                                _stage = 3;
                            }
                        }
                
                        _closeActivationRadius.SetActive(false);
                        _collectibleSprite.gameObject.SetActive(false);
                        _negativeSprite.gameObject.SetActive(true);
                        _soundRangeCollider.SetActive(true);

                        _itemUsable = false;
                        _itemUsed = true;
                        return;
                    }
                    
                    _closeActivationRadius.SetActive(true);
                    _collectibleSprite.gameObject.SetActive(true);
                    _negativeSprite.gameObject.SetActive(false);
                }
            }
            else
            {
                _collectibleSprite.gameObject.SetActive(false);
                _negativeSprite.gameObject.SetActive(false);
                _closeActivationRadius.SetActive(false);
            }
        }
        
        /// <summary>
        /// decides if the item can be reused or not after a certain time
        /// </summary>
        private void ItemExecution()
        {
            if (_itemUsed)
            {
                _deactivationTime -= Time.deltaTime;

                if (_deactivationTime <= 0)
                {
                    if (_reusable)
                    {
                        _deactivationTime = 0.1f;
                        _soundRangeCollider.SetActive(false);
                        _itemUsable = true;
                        _itemUsed = false;
                        _oneTimeUsed = true;
                        MasterAudio.PlaySound("ShatterVase");
                    }
                    else
                    {
                        _negativeSprite.gameObject.SetActive(false);
                        _soundRangeCollider.SetActive(false);
                        Destroy(this);
                        MasterAudio.PlaySound("ShatterVase");
                    }
                }
            }
        }

        /// <summary>
        /// the countdown to choose the enemies, who should be pulled
        /// </summary>
        private void PullCountdown()
        {
            if (startPullCountdown)
            {
                _pullEnemyCooldown -= Time.deltaTime;

                if (_pullEnemyCooldown <= 0)
                {
                    _pullEnemyCooldown = 0.1f;
                    _enemyAmountToPull = _enemyPullAmount;
                    startPullCountdown = false;
                    PullEnemyToNoisyItem();
                }
            }
        }
        
        /// <summary>
        /// search the closest enemies to investigate the noisy item - there is a max amount to pull the enemies
        /// </summary>
        public void PullEnemyToNoisyItem()
        {
            _closestEnemyDistance = Mathf.Infinity;
            foreach (EnemyController enemy in _enemyList)
            {
                _currentclosestEnemyDistance = Vector3.Distance(transform.position, enemy.transform.position);
                if (_currentclosestEnemyDistance <= _closestEnemyDistance)
                {
                    _closestEnemy = enemy;
                    _closestEnemyDistance = _currentclosestEnemyDistance;
                }
            }
            // when the max enemy pull amount is reached, this method will be stopped
            if (_enemyAmountToPull > 0)
            {
                _closestEnemy.CanInvestigate = true;
                _enemyList.Remove(_closestEnemy);
                _enemyAmountToPull--;
                PullEnemyToNoisyItem();

                // reset the amount of desired enemies
                if (_enemyAmountToPull <= 1)
                {
                    _enemyAmountToPull = _enemyPullAmount;
                }
                return;
            }
            
            _enemyList.Clear();
        }
        
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player") && !_itemUsed)
            {
                _playerThrowTrigger.Close = true;
                _itemUsable = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (other.CompareTag("Player") && !_itemUsed)
                {
                    _itemUsable = false;
                    _playerThrowTrigger.Close = false;
                    _collectibleSprite.gameObject.SetActive(false);
                    _closeActivationRadius.SetActive(false);
                }
            }
        }
    }
}