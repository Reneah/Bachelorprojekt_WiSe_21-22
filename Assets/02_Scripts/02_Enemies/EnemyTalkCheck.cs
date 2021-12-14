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
        [SerializeField] private int _chanceToTalk;
        [Tooltip("the distance to stop in front of the other enemy")]
        [Range(1,5)]
        [SerializeField] private float _stopDistance = 0.5f;
        [Tooltip("the time the enemy talk to the other enemy")]
        [Range(3,15)]
        [SerializeField] private float _timeToTalk;
        [Tooltip("smooth the rotation towards the other enemy when talking")]
        [Range(1,10)]
        [SerializeField] private float _smoothRotation;
        
        // determines when the enemy run to each other
        private bool _talkable = false;
        public bool Talkable
        {
            get => _talkable;
            set => _talkable = value;
        }

        // the position of the other enemy
        private GameObject _talkableEnemy;
        // the chance that the enemy will pick another one to speak with him
        private float _chanceToSpeak;
        // the time the enemy talk to the other enemy
        private float _talkCooldown;
        // determines if the enemy has reached the other one to start the talking time
        private bool _countDown = false;
        // take the other enemy partner to talk, so that it is certain that the other one will react
        private bool _takeTalkPartner = false;
        
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
            if (_talkable)
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

                    // When teh enemy is finishing talking, he will go back to his routine and the values will be resetted
                    if (_talkCooldown <= 0)
                    {
                        _enemyAnimation.TalkToEnemy(false);
                        _countDown = false;
                        _talkable = false;
                        EnemyShareInformation.EnemyTalkingNumber = 0;
                        _talkCooldown = _timeToTalk;
                        _takeTalkPartner = false;
                    }
                }
            }
            // if the enemy sees or hears the player while the countdown, everything will be reset
            else if (!_talkable && _countDown)
            {
                _enemyAnimation.TalkToEnemy(false);
                _countDown = false;
                _talkable = false;
                EnemyShareInformation.EnemyTalkingNumber = 0;
                _talkCooldown = _timeToTalk;
                _takeTalkPartner = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // When he passes a talkable enemy, he is able to talk to him with a random chance
            if (other.CompareTag("Talkable"))
            {
                _chanceToSpeak = Random.value;
                
                // When the chance to talk is reached, the enemy is not already talking and is not looting, he can talk with the enemy
                if (_chanceToSpeak <= _chanceToTalk / 100 && EnemyShareInformation.EnemyTalkingNumber < 2 && !EnemyShareInformation.IsLooting || !_takeTalkPartner && EnemyShareInformation.EnemyTalkingNumber < 2 && !EnemyShareInformation.IsLooting)
                {
                    EnemyShareInformation.EnemyTalkingNumber++;
                    
                    if (EnemyShareInformation.EnemyTalkingNumber < 2)
                    {
                        _takeTalkPartner = true;
                    }
                    
                    _talkable = true;
                    _enemyController.AnimationHandler.SetSpeed(_enemyController.PatrolSpeed);
                    _talkableEnemy = other.GetComponentInParent<Transform>().gameObject;
                }
            }
        }
    }
}
