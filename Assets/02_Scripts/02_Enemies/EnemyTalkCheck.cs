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
        [Tooltip("the distance to stop before the other enemy")]
        [Range(1,5)]
        [SerializeField] private float _stopDistance = 0.5f;
        [Tooltip("the time the enemy talk to the other enemy")]
        [Range(3,15)]
        [SerializeField] private float _timeToTalk;
        [Tooltip("smooth the rotation towards the other enemy when talking")]
        [Range(0.5f,10)]
        [SerializeField] private float _smoothRoation;
        
        private bool _talkable = false;
        public bool Talkable => _talkable;

        private GameObject _talkableEnemy;
        private float _chanceToSpeak;
        private float _talkCooldown;
        private bool _countDown = false;
        private bool _takeTalkPartner = false;
        
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
                _enemyController.Agent.SetDestination(_talkableEnemy.transform.position);
                float distanceToEnemy = Vector3.Distance(_talkableEnemy.transform.position, transform.position);

                if (distanceToEnemy <= _stopDistance && !_countDown)
                {
                    _enemyController.Agent.isStopped = true;
                    _enemyAnimation.TalkToEnemy(true);
                    _countDown = true;
                }

                if (_countDown)
                {
                    _talkCooldown -= Time.deltaTime;
                    Quaternion _desiredDirection = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_talkableEnemy.transform.position - transform.position), _smoothRoation * Time.deltaTime);
                    _enemyController.transform.rotation = _desiredDirection;

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
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Talkable"))
            {
                _chanceToSpeak = Random.value;
                
                if (_chanceToSpeak <= _chanceToTalk / 100 && EnemyShareInformation.EnemyTalkingNumber <= 2 || !_takeTalkPartner && EnemyShareInformation.EnemyTalkingNumber <= 2)
                {
                    EnemyShareInformation.EnemyTalkingNumber++;
                    
                    if (EnemyShareInformation.EnemyTalkingNumber <= 2)
                    {
                        _takeTalkPartner = true;
                    }
                    
                    _talkable = true;
                    _talkableEnemy = other.GetComponentInParent<Transform>().gameObject;
                }
            }
        }
    }
}
