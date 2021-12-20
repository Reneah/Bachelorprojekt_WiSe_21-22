using System;
using Enemy.AnimationHandler;
using Enemy.Controller;
using Enemy.ShareInformation;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy.TalkCheck
{
    public class EnemyTalkCheck : MonoBehaviour
    {
        [Tooltip("Chance to talk to him at passing the other enemy in percentage")]
        [Range(0,100)]
        [SerializeField] private float _chanceToTalk;
        [Tooltip("the distance to stop in front of the other enemy")]
        [Range(1,5)]
        [SerializeField] private float _stopDistance = 0.5f;
        [Tooltip("the time the enemy talk to the other enemy")]
        [Range(3,15)]
        [SerializeField] private float _timeToTalk;
        [Tooltip("smooth the rotation towards the other enemy when talking")]
        [Range(1,10)]
        [SerializeField] private float _smoothRotation;
        [Tooltip("how long the enemy can't talk after he has spoken to somebody")]
        [Range(0,60)]
        [SerializeField] private float _reusingTime;
        
        // start the talk state
        private bool _talk = false;
        public bool Talk
        {
            get => _talk;
            set => _talk = value;
        }

        // determines if the enemy is talkable
        private bool _talkable = true;

        public bool Talkable
        {
            get => _talkable;
            set => _talkable = value;
        }
        
        // the script of the other enemy to talk to them and know the position
        private EnemyTalkCheck _talkableEnemy;
        // the chance that the enemy will pick another one to speak with him
        private float _chanceToSpeak;
        // the time the enemy talk to the other enemy
        private float _talkCooldown;
        // determines if the enemy has reached the other one to start the talking time
        private bool _countDown = false;
        // take the other enemy partner to talk, so that it is certain that the other one will react
        private bool _takeTalkPartner = false;
        
        public bool TakeTalkPartner
        {
            get => _takeTalkPartner;
            set => _takeTalkPartner = value;
        }

        private float _talkReuseingTime;

        public float TalkReuseingTime
        {
            get => _talkReuseingTime;
            set => _talkReuseingTime = value;
        }

        private bool _startCooldown;
        
        // need those script to communicate with the enemy
        private EnemyController _enemyController;
        private EnemyAnimationHandler _enemyAnimation;
        
        void Start()
        {
            _talkCooldown = _timeToTalk;
            
            _enemyController = GetComponentInParent<EnemyController>();
            _enemyAnimation = GetComponentInParent<EnemyAnimationHandler>();
        }
        
        void Update()
        {
            if (_talk)
            {
                // set the destination of the talk partner
                _enemyController.Agent.SetDestination(_talkableEnemy.transform.position);
                float distanceToEnemy = Vector3.Distance(_talkableEnemy.transform.position, transform.position);

                // When the talk partner is reached, he will start to play the talk animation
                if (distanceToEnemy <= _stopDistance && !_countDown)
                {
                    _enemyController.Agent.isStopped = true;
                    _enemyAnimation.TalkToEnemy(true);
                    _countDown = true;
                }

                // When the enemy is talking, he is rotating to his partner
                if (_countDown)
                {
                    _talkCooldown -= Time.deltaTime;
                    Quaternion _desiredDirection = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_talkableEnemy.transform.position - transform.position), _smoothRotation * Time.deltaTime);
                    _enemyController.transform.rotation = _desiredDirection;

                    // When the enemy is finishing talking, he will go back to his routine and the values will be reset
                    if (_talkCooldown <= 0)
                    {
                        _enemyAnimation.TalkToEnemy(false);
                        _countDown = false;
                        _talk = false;
                        _talkCooldown = _timeToTalk;
                        _takeTalkPartner = false;
                        _talkable = true;
                        _startCooldown = true;
                    }
                }
            }
            // if the enemy sees or hears the player while the countdown, everything will be reset
            else if (!_talk && _countDown)
            {
                _enemyAnimation.TalkToEnemy(false);
                _countDown = false;
                _talk = false;
                _talkCooldown = _timeToTalk;
                _takeTalkPartner = false;
                _talkable = true;
            }
            else if(_startCooldown)
            {
                _talkReuseingTime -= Time.deltaTime;

                if (_talkReuseingTime <= 0)
                {
                    _startCooldown = false;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // When he passes a talkable enemy, he is able to talk to him with a random chance
            if (other.CompareTag("Talkable") && _talkReuseingTime <= 0)
            {
                _talkableEnemy = other.GetComponent<EnemyTalkCheck>();
                _chanceToSpeak = Random.value;
                
                // When the chance to talk is reached, the enemy is not already talking, is blocked or the cooldown is over 0, he can talk with the enemy
                if (_chanceToSpeak <= _chanceToTalk / 100 && _talkable && !_takeTalkPartner && _talkableEnemy.Talkable && _talkableEnemy.TalkReuseingTime <= 0)
                {
                    _talkableEnemy.TakeTalkPartner = true;
                    _talkable = false;
                    _talk = true;
                    _enemyController.AnimationHandler.SetSpeed(_enemyController.PatrolSpeed);
                    _talkReuseingTime = _reusingTime;
                    
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            // this is for the partner, that he can get the enemy who wants to talk to him
            if (other.CompareTag("Talkable") && _takeTalkPartner && _talkable)
            {
                _talkable = false;
                _talk = true;
                _enemyController.AnimationHandler.SetSpeed(_enemyController.PatrolSpeed);
            }
        }
    }
}
