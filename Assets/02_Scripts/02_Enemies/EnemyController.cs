using System.Collections.Generic;
using BP._02_Scripts._03_Game;
using Enemy.AnimationHandler;
using Enemy.SearchArea;
using Enemy.ShareInformation;
using Enemy.SoundItem;
using Enemy.States;
using Enemy.TalkCheck;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using untitledProject;
using Random = UnityEngine.Random;

namespace Enemy.Controller
{
    public class EnemyController : MonoBehaviour
    {
        private NavMeshAgent _agent;
        public NavMeshAgent Agent => _agent;
        
        // need this script to get the information of the sound activation
        private NoisyItem _noisyItemScript;
        public NoisyItem NoisyItemScript
        {
            get => _noisyItemScript;
            set => _noisyItemScript = value;
        }

        // need this script to get the transform information and some methods of the player
        private PlayerController _player;
        public PlayerController Player => _player;

        // need this script so that the enemy knows on which ground the player is 
        private PlayerGroundDetection _playerGroundDetection;
        public PlayerGroundDetection PlayerGroundDetection => _playerGroundDetection;

        // this script controls the animation of the enemy
        private EnemyAnimationHandler _animationHandler;
        public EnemyAnimationHandler AnimationHandler => _animationHandler;

        // need this script to use the bool values
        private EnemyTalkCheck _enemyTalkCheck;
        public EnemyTalkCheck EnemyTalkCheck => _enemyTalkCheck;

        // this script is for the score at the end of the game and counts some things
        private MissionScore _myMissionScore;
        
        // need this script to call the death scene
        private InGameMenu _inGameMenu;
        public InGameMenu InGameMenu
        {
            get => _inGameMenu;
            set => _inGameMenu = value;
        }

        // the current state of the enemy
        private IEnemyState _currentState;
        
        // the different states of the enemy
        public static readonly EnemyPatrolState EnemyPatrolState = new EnemyPatrolState();
        public static readonly EnemyIdleState EnemyIdleState = new EnemyIdleState();
        public static readonly EnemyVisionChaseState EnemyVisionChaseState = new EnemyVisionChaseState();
        public static readonly EnemySearchState EnemySearchState = new EnemySearchState();
        public static readonly EnemySoundInvestigationState EnemySoundInvestigationState = new EnemySoundInvestigationState();
        public static readonly EnemyGuardState EnemyGuardState = new EnemyGuardState();
        public static readonly EnemyNoisyItemSearchState EnemyNoisyItemSearchState = new EnemyNoisyItemSearchState();
        public static readonly EnemyTalkState EnemyTalkState = new EnemyTalkState();
        public static readonly EnemyLootState EnemyLootState = new EnemyLootState();

        [Header("Main Behaviour")]
        [Tooltip("the main task of the enemy is guarding - Patrolling will be not available")]
        [SerializeField] private bool _guarding;
        [Tooltip("the main task of the enemy is patrolling - Guarding will be not available")]
        [SerializeField] private bool _patrolling;

        public bool Guarding => _guarding;

        #region ChaseVariables

        [Header("Chase Behaviour")]
        [Tooltip("set the distance to catch the player")]
        [Range(0.5f, 3)]
        [SerializeField] private float _lowGroundCatchDistance = 2;
        [Tooltip("set the distance to catch the player")]
        [Range(1, 3)]
        [SerializeField] private float _highGroundCatchDistance = 2;
        [Tooltip("the speed which the enemy will chase the player")]
        [Range(1,7)]
        [SerializeField] private float _chaseSpeed;
        [Tooltip("the range to pull another enemy nearby when this enemy spotted the player")]
        [Range(1, 7)]
        [SerializeField] private float _pullDistance = 3;
        
        // determines if the first enemy reached the destination of the player nearby so that the other ones can gather around
        private bool _firstEnemyReachedDestination;

        // when the enemy is near another enemy, who has sighted the player, he will chase him as well
        private bool _activateChasing = false;
        
        // the position on the NavMesh around the current player position that is reachable
        private NavMeshHit _hit;

        // when the enemy spotted the player, the score for the player will change
        private bool _scoreCount;

        // need all enemies to pull them nearby when another one spotted the player
        private EnemyController[] _enemiesInWholeScene;
        
        // when the enemy hears the footstep of the player, he will go into the chase mode
        private bool _playerSoundSpotted = false;

        public bool PlayerSoundSpotted
        {
            get => _playerSoundSpotted;
            set => _playerSoundSpotted = value;
        }
        
        public bool ActivateChasing
        {
            get => _activateChasing;
            set => _activateChasing = value;
        }
        
        #endregion

        #region PatrolVariables
        
        [Header("Patrol Behaviour")]
        [Tooltip("the parent of the waypoints")]
        [SerializeField] private GameObject _patrollingRoute;
        [Tooltip("the time which the enemy is standing at a waypoint")]
        [Range(0,10)]
        [SerializeField] private float _dwellingTimer = 1;
        [Range(0,3)]
        [Tooltip("the distance where the enemy will stop when arriving at a waypoint")]
        [SerializeField] private float _stopDistance = 0.5f;
        [Tooltip("the patrol speed of the enemy")] 
        [Range(0,5)]
        [SerializeField] private float _patrolSpeed;
        
        // determines the current patrol points of the enemy
        private int _patrolPointsCounter = 0;
        // activates the dwelling time when arriving a waypoint
        private bool _reachedWaypoint = false;
        // the time which the enemy is standing at a waypoint
        private float _standingCooldown = 0;
        // the list of the enemy patrol points
        List<Transform> _patrolPoints = new List<Transform>();
        
        public bool Patrolling => _patrolling;

        public float PatrolSpeed => _patrolSpeed;

        #endregion
        
        #region FieldOfViewVariables
        
        [Header("FieldOfView")]
        [Tooltip("the mask to determine the obstacle in the view field")]
        [SerializeField] private LayerMask _obstructionMask;
        [Tooltip("the enemy head transform to have the origin for raycasts in his view field")]
        [SerializeField] private Transform _enemyHead;
        [Tooltip("the raycast position to know obstacles")] 
        [SerializeField] private Transform _obstacleRaycastTransform;
        [Tooltip("the look position when the player is spotted")]
        [SerializeField] private Transform _lookPositionAtSpotted;
        [Tooltip("the delay time that the player gets spotted in the view field")]
        [Range(0,5)]
        [SerializeField] private float _visionSecondsToSpot = 1.5f;
        [Tooltip("the delay time that the player gets spotted in the hear radius")]
        [Range(0,5)]
        [SerializeField] private float _acousticSecondsToSpot = 1.5f;
        [Tooltip("the spotted bar, which shows the spotted time above the enemy")]
        [SerializeField] private Image _spottedBar;
        [Tooltip("the distance where the player gets spotted instantly")]
        [Range(0,5)]
        [SerializeField] private float _spottedVisionDistance;
        [Tooltip("the distance where player get spotted instantly")]
        [Range(0,5)]
        [SerializeField] private float _spottedAcousticDistance;
        [Tooltip("the time which will still set the player destination after out of sight to simulate the awareness that the player is not in his mind anymore")]
        [Range(0,5)]
        [SerializeField] private float _reminderTimeLowGround;
        [Range(0,10)]
        [Tooltip("the time which will still set the player destination after out of sight to simulate the awareness that the player is not in his mind anymore")]
        [SerializeField] private float _reminderTimeHighGround;
        [Tooltip("the view cone that will be activated when the player is on high ground")]
        [SerializeField] private GameObject _highGroundViewCone;
        [Tooltip("the view cone that will be activated when the player is on low ground")]
        [SerializeField] private GameObject _lowGroundViewCone;
        
        // the delay time that the player gets spotted in the view field
        private float _visionTimeToSpot;
        //the delay time that the player gets spotted in the hear radius
        private float _acousticTimeToSpot;
        // the distance where the player gets spotted instantly
        private float _detectionAcousticDistance;

        public float ReminderTimeLowGround => _reminderTimeLowGround;

        public float ReminderTimeHighGround => _reminderTimeHighGround;
        
        public float DetectionAcousticDistance => _detectionAcousticDistance;

        public GameObject LowGroundViewCone => _lowGroundViewCone;

        public GameObject HighGroundViewCone => _highGroundViewCone;

        public float VisionTimeToSpot
        {
            get => _visionTimeToSpot;
            set => _visionTimeToSpot = value;
        }

        public float AcousticTimeToSpot
        {
            get => _acousticTimeToSpot;
            set => _acousticTimeToSpot = value;
        }

        public float AcousticSecondsToSpot => _acousticSecondsToSpot;

        public float VisionSecondsToSpot => _visionSecondsToSpot;

        // when the player is in the view field, the spotted time for the vision will be used
        private bool _playerInViewField = false;

        public bool PlayerInViewField
        {
            get => _playerInViewField;
            set => _playerInViewField = value;
        }

        public float SpottedAcousticDistance
        {
            get => _spottedAcousticDistance;
            set => _spottedAcousticDistance = value;
        }

        // the time to spot the enemy when he is in the view field
        private float _spotTime = 0;
        
        // determines if the enemy is able to see the player or not. Player is visible when he is spotted
        private bool _canSeePlayer = false;
        
        // give the enemy the chance to chase the player again
        // otherwise at a quick turn of the player or other situations, the enemy can't see him anymore, but should have an awareness that the player is next to or behind him. That is more realistic
        private float _lastChanceTime = 0;

        public float LastChanceTime
        {
            get => _lastChanceTime;
            set => _lastChanceTime = value;
        }
        
        // determines if the player is spotted or not
        private bool _playerSpotted = false;
        // the player is in sight and the spotTime can start to manipulate the bar
        private bool _useSpottedBar = false;

        public bool UseSpottedBar
        {
            set => _useSpottedBar = value;
        }

        public bool PlayerSpotted
        {
            get => _playerSpotted;
            set => _playerSpotted = value;
        }

        public float SpotTime
        {
            set => _spotTime = value;
        }
        
        public Transform LookPositionAtSpotted => _lookPositionAtSpotted;

        public Transform ObstacleRaycastTransform => _obstacleRaycastTransform;

        public Transform EnemyHead => _enemyHead;

        public bool CanSeePlayer
        {
            get => _canSeePlayer;
            set => _canSeePlayer = value;
        }
        
        public LayerMask ObstructionMask => _obstructionMask;

        private bool _inChaseState = false;
        public bool InChaseState
        {
            get => _inChaseState;
            set => _inChaseState = value;
        }
        
        public Image SpottedBar => _spottedBar;

        #endregion

        #region InvestigationVariables

        [Header("Investigation Behaviour")]
        [Tooltip("the enemy run speed to the sound event point at the first sound stage")]
        [Range(1, 5)]
        [SerializeField] private float _firstStageRunSpeed;
        [Range(2, 7)]
        [Tooltip("the enemy run speed to the sound event point at the second sound stage")]
        [SerializeField] private float _secondStageRunSpeed;
        [Range(2.5f, 10)]
        [Tooltip("the enemy run speed to the sound event point at the third sound stage")]
        [SerializeField] private float _thirdStageRunSpeed;
        [Range(1,10)]
        [Tooltip("the enemy run speed when he goes from point to point to search the player")]
        [SerializeField] private float _playerSearchSpeed;
        [Tooltip("layer mask to block the acoustic through objects")]
        [SerializeField] private LayerMask _blockAcousticLayerMasks;
         // the current sound state of the item to update the behaviour of the enemy
         private int _currentSoundStage = 0;
         // prevent that the animation will be activated permanently in Update
         private bool _animationActivated = false;
         // the enemy is able to investigate the noisy item when he heard the sound and the max enemy pull amount is not reached
         private bool _canInvestigate = false;
         // because the trigger enter gets more calls than one at the sound tag, I counter it with this bool
         private bool _getSoundOnce = false;
         // when the player enters the hear field, the spotted time will be activated
         private bool _playerInHearField = false;
         // when the collider is deactivated, the Trigger Exit function can be replaced to stop the spotted time
         private Collider _hearFieldPlayerCollider;
         // need this script to get the collider of the step sound 
         private PlayerStepsSound _playerStepsSound;
         // need this script to get the information of the search points
         private SearchAreaOverview _searchArea;
         // need this script to to get the information of the noisy item points
         private NoisyItemSearchArea _noisyItemSearchArea;
         // prevent that the if condition in update will run all the time
         private bool _hearFieldColliderActiv = true;
         // the speed when the enemy is searching the environment after investigating the noisy item
         private float _investigationRunSpeed;
         // determines if the enemy heard a sound
         private bool _soundNoticed = false;
         // the current sound stage that influence the enemy behaviour
         private int _soundBehaviourStage = 0;
         // the position from where the sound came so that the enemy knows where to move
         private Transform _soundEventPosition;

         public float InvestigationRunSpeed => _investigationRunSpeed;

         public NoisyItemSearchArea NoisyItemSearchArea => _noisyItemSearchArea;

         public SearchAreaOverview SearchArea => _searchArea;
         
         public float SearchSpeed => _playerSearchSpeed;

         public float FirstStageRunSpeed => _firstStageRunSpeed;

         public float SecondStageRunSpeed => _secondStageRunSpeed;

         public float ThirdStageRunSpeed => _thirdStageRunSpeed;
         
         public int SoundBehaviourStage => _soundBehaviourStage;
         
         public Transform SoundEventPosition => _soundEventPosition;
         
         public bool CanInvestigate
         {
             set => _canInvestigate = value;
         }

         public bool GetSoundOnce
         {
             set => _getSoundOnce = value;
         }
         
         public bool AnimationActivated
         {
             get => _animationActivated;
             set => _animationActivated = value;
         }

         public int CurrentSoundStage
         {
             get => _currentSoundStage;
             set => _currentSoundStage = value;
         }
         
         public bool SoundNoticed
        {
            get => _soundNoticed;
            set => _soundNoticed = value;
        }
         
        #endregion
        
        #region GuardVariables
        
        [Header("Guard Behaviour")]
        [Tooltip("the time to switch between the looking points")]
        [Range(0,10)]
        [SerializeField] private float _switchLookTime;
        [Tooltip("the speed how fast the invisible agent is moving")]
        [Range(0,10)]
        [SerializeField] private float _lookSwitchSpeed;
        [Tooltip("the parent of the route where the enemy should look")]
        [SerializeField] private GameObject _lookingRoute;
        [Tooltip("the point where the enemy is guarding")]
        [SerializeField] private Transform _guardPoint;
        [Tooltip("the distance to stop when reaching the guard point")]
        [Range(0,5)]
        [SerializeField] private float _stopGuardpointDistance = 0.5f;
        [Tooltip("the rotation of the enemy body when he is guarding")]
        [SerializeField] private Transform _desiredBodyRotation;
        [Tooltip("the look agent to look in specific directions over time")]
        [SerializeField] private Transform _currentLookPosition;
        [Range(0,20)]
        [Tooltip("smooth the rotation towards the desired Body Rotation")]
        [SerializeField] private float _smoothBodyRotation;
        // signalize if the enemy reached the guard point to start his look routine
        private bool _reachedGuardpoint = false;
        // The direction where the enemy should rotate while guarding
        private Quaternion _desiredDirection;
        
        public bool ReachedGuardpoint
        {
            get => _reachedGuardpoint;
            set => _reachedGuardpoint = value;
        }

        public Transform CurrentLookPosition => _currentLookPosition;
        public float SmoothBodyRotation => _smoothBodyRotation;
        public Transform DesiredBodyRotation => _desiredBodyRotation;
        public Transform GuardPoint => _guardPoint;

        // the list of the enemy look points
        private List<Transform> _lookpoints = new List<Transform>();
        // the cooldown how long the enemy should look on a point
        private float _lookCooldown = 0;
        // the current look point in the array
        private int _lookPointcounter = 0;
        // when the current look point was reached, choose the next one 
        private bool _reachedLookPoint = false;
        // signalize when the enemy is in guard mode to rotate the head in the desired look position 
        private bool _guardBehaviour = false;
        // The current look point where the enemy has to look at
        private Transform _currentLookPoint;
        
        public bool GuardBehaviour
        {
            get => _guardBehaviour;
            set => _guardBehaviour = value;
        }

        #endregion

        #region LootVariables
        
        // the distance to stop in front of the loot spot
        private float _stopDistanceLootSpot;

        public float StopDistanceLootSpot
        {
            get => _stopDistanceLootSpot;
            set => _stopDistanceLootSpot = value;
        }

        // smooth the enemy rotation towards the loot location
        private float _smoothRotation;

        public float SmoothRotation
        {
            get => _smoothRotation;
            set => _smoothRotation = value;
        }

        // determines if the enemy is looting to switch the enemy state
        private bool _loot = false;

        public bool Loot
        {
            get => _loot;
            set => _loot = value;
        }

        // determines if the loot spot is reached to start the loot time
        private bool _reachedLootSpot = false;

        public bool ReachedLootSpot
        {
            get => _reachedLootSpot;
            set => _reachedLootSpot = value;
        }

        // the position of the loot spot to know the destination for the agent
        private Transform _lootSpotTransform;

        public Transform LootSpotTransform
        {
            get => _lootSpotTransform;
            set => _lootSpotTransform = value;
        }

        // enemy should only be allowed to loot when he is patrolling
        private bool _ableToLoot = false;

        public bool AbleToLoot
        {
            get => _ableToLoot;
            set => _ableToLoot = value;
        }

        // reset the loot variables when it will be interuppted
        private bool _resetLootVariables = false;

        public bool ResetLootVariables
        {
            get => _resetLootVariables;
            set => _resetLootVariables = value;
        }
        
        #endregion
        
        void Start()
        {
            // start with low ground view cone
            _highGroundViewCone.SetActive(false);
            _lowGroundViewCone.SetActive(true);
            
            // start state machine with the idle
            _currentState = EnemyIdleState;
            
            _agent = GetComponent<NavMeshAgent>();
            _animationHandler = GetComponent<EnemyAnimationHandler>();
            _player = FindObjectOfType<PlayerController>();
            _inGameMenu = FindObjectOfType<InGameMenu>();
            _playerGroundDetection = FindObjectOfType<PlayerGroundDetection>();
            _enemyTalkCheck = GetComponentInChildren<EnemyTalkCheck>();
            _playerStepsSound = FindObjectOfType<PlayerStepsSound>();
            _hearFieldPlayerCollider = _playerStepsSound.GetComponent<Collider>();
            _myMissionScore = FindObjectOfType<MissionScore>();
            _enemiesInWholeScene = FindObjectsOfType<EnemyController>();

            // designer can choose between patrolling or guarding mode. The enemy will use only one mode as routine
            if (_patrolling)
            {
                SetUpPatrolBehaviour(); 
            }
            else if(_guarding)
            {
                SetUpGuardBehaviour(); 
            }
            
            // the dwelling time at a waypoint
            _standingCooldown = _dwellingTimer;

            // set the slowest investigation speed the enemy has. At the first and second stage the enemy doesn't expect the player, so he will search slowly
            _investigationRunSpeed = _firstStageRunSpeed;

            // set the detection time and distance
            _visionTimeToSpot = _visionSecondsToSpot;
            _acousticTimeToSpot = _acousticSecondsToSpot;
            _detectionAcousticDistance = _spottedAcousticDistance;
        }
        
        void Update()
        {
            var enemyState = _currentState.Execute(this);
            if (enemyState != _currentState)
            {
                _currentState.Exit(this);
                _currentState = enemyState;
                _currentState.Enter(this);
            }
            
            PlayerDetection();
            ActivateNoisyItemInvestigation();
        }
        
        #region ChaseBehaviour

        /// <summary>
        /// pulls enemies nearby when another enemy has spotted the player
        /// </summary>
        public void PullEnemyNearby()
        {
            for (int i = 0; i < _enemiesInWholeScene.Length; i++)
            {
                // when the enemy is in pull range, he will be pulled to chase the player as well
                if (Vector3.Distance(transform.position, _enemiesInWholeScene[i].transform.position) <= _pullDistance)
                {
                    if (!_enemiesInWholeScene[i].InChaseState)
                    {
                        _enemiesInWholeScene[i].ActivateChasing = true;
                        
                        // when the enemy will be pulled of another one, the enemy should not go instantly into the search mode. Should have the chance to follow the player
                        _enemiesInWholeScene[i].LastChanceTime = 5;
                        
                        _enemiesInWholeScene[i].UseSpottedBar = true;
                        _enemiesInWholeScene[i].SpottedBar.fillAmount = 1;
                        _enemiesInWholeScene[i].PlayerSpotted = true;
                        _enemiesInWholeScene[i].SpotTime = 10;
                    }
                }
            }
        }
        
        /// <summary>
        /// Check if the Player is on higher ground or not to modify the vision for better player recognizing
        /// </summary>
        public void CheckPlayerGround()
        {
            if (!_scoreCount)
            {
                // Counts up the mission score for the player to have been spotted
                _myMissionScore.SpottedScoreCounter += 1;
                _scoreCount = true;
            }
            
            if (_playerGroundDetection.LowGround)
            {             
                // Standard View Cone
                _lowGroundViewCone.SetActive(true);
                _highGroundViewCone.SetActive(false);

                // when player is visible, set the low ground reminder time
                if (_canSeePlayer)
                {
                    _lastChanceTime = ReminderTimeLowGround;
                }
            }

            if (_playerGroundDetection.HighGround)
            {
                // Bigger View Cone
                _lowGroundViewCone.SetActive(false);
                _highGroundViewCone.SetActive(true);

                // when player is visible, set the high ground reminder time
                if (_canSeePlayer)
                {
                    _lastChanceTime = ReminderTimeHighGround;
                }
            }
        }
        
        /// <summary>
        /// check if the enemy reached the last point that he is able to reach the player
        /// </summary>
        /// <returns></returns>
        public bool ClosestPlayerPosition(float _stopDistance)
        {
            // give a position around the player on the NavMesh that is reachable
            NavMesh.SamplePosition(_player.transform.position, out _hit,4, NavMesh.AllAreas);
            
            return Vector3.Distance(transform.position, _hit.position) <= _stopDistance;
        }
        
        /// <summary>
        /// When the enemy is in the chase state, the destination will be the player and the speed will be modified
        /// </summary>
        public void ChasePlayer()
        {
            // prevent that the run animation is playing when the agent can't go further in contrast to the player 
            // rotates the enemy towards the player position 
            // first if condition: first enemy reached the destination - second if condition: when more than one enemy reaches around the destination, they will stop 
            if (ClosestPlayerPosition(0.5f) || ClosestPlayerPosition(2.5f) && EnemyShareInformation.FirstEnemyReachedDestination && _playerGroundDetection.HighGround)
            {
                if (!EnemyShareInformation.FirstEnemyReachedDestination)
                {
                    EnemyShareInformation.FirstEnemyReachedDestination = true;
                }
                
                _agent.isStopped = true;
                _animationHandler.SetSpeed(0);
                
                if (Vector3.Dot(transform.TransformDirection(Vector3.forward), _player.transform.position - transform.position) <= 0.9f)
                {
                    _desiredDirection = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_player.transform.position - transform.position), 5 * Time.deltaTime);
                    transform.rotation = _desiredDirection;
                }
                return;
            }

            // when the first enemy reached the destination, the enemy will be taken to signalize that the other have to stop around the destination 
            EnemyShareInformation.FirstEnemyReachedDestination = false; 
            
            _agent.SetDestination(_player.transform.position);
            _agent.speed = _chaseSpeed;
            _animationHandler.SetSpeed(_chaseSpeed);
            _agent.isStopped = false;
        }
        
        /// <summary>
        /// Determines if the player is in catch range to end the game 
        /// </summary>
        /// <returns></returns>
        public bool CatchPlayer()
        {
            if (_playerGroundDetection.HighGround)
            {
               return  Vector3.Distance(transform.position, _player.transform.position) <= _highGroundCatchDistance;
            }
            else
            {
                return Vector3.Distance(transform.position, _player.transform.position) <= _lowGroundCatchDistance;
            }
        }
        
        #endregion
        
        #region PatrolBehaviour
        
        /// <summary>
        /// set the dwelling cooldown and add the waypoints to the list
        /// </summary>
        private void SetUpPatrolBehaviour()
        {
            _standingCooldown = _dwellingTimer;
            
            foreach (Transform waypoints in _patrollingRoute.transform)
            {
                _patrolPoints.Add(waypoints);
            }
        }
        
        /// <summary>
        /// set the first destination point and patrol speed
        /// </summary>
        public void StartPatrolBehaviour()
        {
            _agent.speed = _patrolSpeed;
            _agent.SetDestination(_patrolPoints[_patrolPointsCounter].position);
        }
        
        /// <summary>
        /// loop through the patrol points 
        /// </summary>
        public void UpdatePatrolBehaviour()
        {
            // when the enemy reached the current set waypoint, the standing time will count down and set the next patrol point
            if (Vector3.Distance(transform.position, _patrolPoints[_patrolPointsCounter].transform.position) <= _stopDistance && !_reachedWaypoint)
            {
                _reachedWaypoint = true;
            }

            if (_reachedWaypoint)
            {
                _animationHandler.SetSpeed(0);
                _standingCooldown -= Time.deltaTime;

                if (_standingCooldown <= 0)
                {
                    _patrolPointsCounter++;
                    _patrolPointsCounter %= _patrollingRoute.transform.childCount;
                    _standingCooldown = _dwellingTimer;
                    _agent.SetDestination(_patrolPoints[_patrolPointsCounter].position);
                    _reachedWaypoint = false;
                    _animationHandler.SetSpeed(_patrolSpeed);
                }
            }
        }
        
        #endregion

        #region GuardBehaviour
        
        /// <summary>
        /// set the look cooldown, add the waypoints to the list and set the current look position 
        /// </summary>
        public void SetUpGuardBehaviour()
        {
            _lookCooldown = _switchLookTime;
            
            foreach (Transform lookpoints in _lookingRoute.transform)
            {
                _lookpoints.Add(lookpoints);
            }
            
            _currentLookPoint = _lookpoints[_lookPointcounter].transform;
            _currentLookPosition.transform.position = _currentLookPoint.transform.position;
        }
        
        /// <summary>
        /// loop through the guard points
        /// </summary>
        public void UpdateGuardBehaviour()
        {
            // move the look position to the waypoint
            // the head of the enemy is following the current look position
            _currentLookPosition.position = Vector3.MoveTowards(_currentLookPosition.transform.position, _currentLookPoint.transform.position, Time.deltaTime * _lookSwitchSpeed);

            // when the current look position is reached, the look time will count down and set the next look point
            if (Vector3.Distance(_currentLookPosition.transform.position, _currentLookPoint.transform.position ) <= 0.5f && !_reachedLookPoint)
            {
                _reachedLookPoint = true;
            }
            
            if (_reachedLookPoint)
            {
                _lookCooldown -= Time.deltaTime;

                if (_lookCooldown <= 0)
                {
                    _lookPointcounter++;
                    _lookPointcounter %= _lookingRoute.transform.childCount;
                    _lookCooldown = _switchLookTime;
                    _currentLookPoint = _lookpoints[_lookPointcounter].transform;
                    _reachedLookPoint = false;
                }
            }
        }

        /// <summary>
        /// the distance to the guard point to start his look routine
        /// </summary>
        /// <returns></returns>
        public bool GuardPointDistance()
        {
            return Vector3.Distance(transform.position, _guardPoint.transform.position) <= _stopGuardpointDistance;
        }

        /// <summary>
        /// the desired standing direction of the enemy at the guard point
        /// </summary>
        public void DesiredStandingLookDirection()
        {
            _desiredDirection = Quaternion.Slerp(transform.rotation, DesiredBodyRotation.rotation, SmoothBodyRotation * Time.deltaTime);
            transform.rotation = _desiredDirection;
        }
        
        #endregion

        #region SpottedBar
        
        /// <summary>
        /// Bar control to show how much the player is spotted of the enemy
        /// </summary>
        public void PlayerDetection()
        {
            // when the trigger exit could not be used and the collider is still enabled, it will be deactivated
            if (!_hearFieldPlayerCollider.enabled && _hearFieldColliderActiv)
            {
                _hearFieldColliderActiv = false;
                _useSpottedBar = false;
                _playerInHearField = false;
            }
            
            // when the enemy sees the player, he will get spotted in a fixed time when he stays in the view or hear field
            if (_useSpottedBar)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);

                // the vision time will run and will fill the bar until the player is spotted
                if (_spotTime <= _visionTimeToSpot && _playerInViewField)
                {
                    _spotTime += Time.deltaTime / _visionTimeToSpot;
                    _spottedBar.fillAmount = _spotTime;
                }
                
                // the acoustic time will run and will fill the bar until the player is spotted
                if (_playerInHearField && _spotTime <= _acousticTimeToSpot)
                {
                    _spotTime += Time.deltaTime / _acousticTimeToSpot;
                    _spottedBar.fillAmount = _spotTime;
                }
                
                // when the player is to close to the enemy or too long in the view field, the player get spotted
                if (_spottedBar.fillAmount >= 1 || distanceToPlayer <= _spottedVisionDistance && _playerInViewField || distanceToPlayer <= _spottedAcousticDistance)
                {
                    _player.PlayerAnimationHandler.PlayerFlee(true);
                    _spotTime = 1;
                    _spottedBar.fillAmount = _spotTime;
                    _playerSpotted = true;
                }
            }
            
            else if(!_useSpottedBar && !_inChaseState)
            {
                if (_spotTime > 0)
                {
                    _spotTime -= Time.deltaTime;
                    _spottedBar.fillAmount = _spotTime;
                }

                if (_spotTime < 0)
                {
                    _spotTime = 0;
                }
            }
        }
        
        #endregion
        
        #region SearchBehaviour
        
        /// <summary>
        /// loop through the search points
        /// </summary>
        public void UpdateSearchBehaviour()
        {
            // enemy has lost the player and reset the score count ability, so next time the player is spotted
            // the score will be counted up again
            _scoreCount = false;
            
            // when the current search point position is reached, the standing time will count down and set the next search point
            if (Vector3.Distance(transform.position, _agent.destination) <= _stopDistance && !_reachedWaypoint)
            {
                _animationHandler.SetSpeed(0);
                _reachedWaypoint = true;
                _agent.enabled = false;
            }

            if (_reachedWaypoint)
            {
                _standingCooldown -= Time.deltaTime;
                
                if (_standingCooldown <= 0)
                {
                    _agent.enabled = true;
                    _standingCooldown = _dwellingTimer;
                    _reachedWaypoint = false;
                    _searchArea.StartSearchBehaviour(_agent, _animationHandler, _playerSearchSpeed);
                }
            }
        }
        #endregion
        
        #region SearchNoisyItemBehaviour
        
        /// <summary>
        /// loop through the noisy item search points
        /// </summary>
        public void UpdateSearchNoisyItemBehaviour()
        {
            // when the current search point position is reached, the standing time will count down and set the next search point
            if (Vector3.Distance(transform.position, _agent.destination) <= _stopDistance && !_reachedWaypoint)
            {
                _animationHandler.SetSpeed(0);
                _reachedWaypoint = true;
            }
            
            if (_reachedWaypoint)
            {
                _standingCooldown -= Time.deltaTime;
                
                if (_standingCooldown <= 0)
                {
                    _standingCooldown = _dwellingTimer;
                    _reachedWaypoint = false;
                    _noisyItemSearchArea.StartSearchNoisyItemBehaviour(_agent, _animationHandler, _investigationRunSpeed, _noisyItemScript);
                }
            }
        }

        /// <summary>
        /// activate the noisy item investigation when the enemy is chosen for this
        /// </summary>
        public void ActivateNoisyItemInvestigation()
        {
            if (_canInvestigate)
            {
                // the amount of the waypoints fo the enemy when the player activated the noisy item in close range
                _noisyItemSearchArea.UsuableWaypointsAmount = Random.Range(2,_noisyItemScript.CloseNoisyItemWaypoints.Length);
                
                // when the sound stage goes from 1 to 3, the sound will be noticed and the enemy will start to run towards it
                if(_noisyItemScript.Stage <= 3)
                {
                    _soundBehaviourStage = _noisyItemScript.Stage;
                    _soundEventPosition = _noisyItemScript.OffsetOrigin.transform;
                    _soundNoticed = true;
                }

                _canInvestigate = false;
            }
        }
        #endregion
        
        private void OnTriggerEnter(Collider other)
        {
            // when the enemy hears the noisy item sound, he will investigate it when the max enemy pull amount is not reached
            if (other.CompareTag("Sound") && !_getSoundOnce)
            {
                // get the current noisy item script of the item
                _noisyItemScript = other.GetComponentInParent<NoisyItem>();
                
                Vector3 _raycastDirection = new Vector3(_noisyItemScript.transform.position.x, _noisyItemScript.transform.position.y, _noisyItemScript.transform.position.z) - _enemyHead.position;
                // when a specific layer is hit that isolate sound, the enemy will hear nothing
                if (Physics.Raycast(_enemyHead.position, _raycastDirection, Vector3.Distance(_enemyHead.position , _noisyItemScript.transform.position), _blockAcousticLayerMasks))
                {
                    return;
                }
                
                _getSoundOnce = true;
                
                // add the enemy to the list and start the cooldown to choose the closest enemies
                _noisyItemScript.EnemyList.Add(this);
                _noisyItemScript.StartPullCountdown = true;
            }
            
            // get the scripts of the current search area
            if (other.CompareTag("SearchPoints"))
            {
                _searchArea = other.GetComponent<SearchAreaOverview>();
                _noisyItemSearchArea = other.GetComponent<NoisyItemSearchArea>();
            }
        }

        private void OnTriggerStay(Collider other)
        {
            // when the enemy hears the footsteps of the player, the spotted bar will be activated
            if (other.CompareTag("FootSteps"))
            {
                Vector3 _raycastDirection = new Vector3(_player.transform.position.x, _player.transform.position.y + 2.5f, _player.transform.position.z) - _enemyHead.position;
                // when a specific layer is hit that isolate sound, the enemy will hear nothing
                if (Physics.Raycast(_enemyHead.position, _raycastDirection,  Vector3.Distance(_enemyHead.position , _player.transform.position), _blockAcousticLayerMasks))
                {
                    _useSpottedBar = false;
                    return;
                }

                _hearFieldColliderActiv = true;
                _playerInHearField = true;
                _useSpottedBar = true;
                
                // when the player is spotted, the player will be chased
                if (_playerSpotted)
                {
                    CheckPlayerGround();
                    
                    // goes instantly in the chase vision mode to have the reminder time to chase the player
                    _playerSoundSpotted = true;
                    _lastChanceTime = 5;
                    
                    if (!_scoreCount)
                    {
                        // Counts up the mission score for the player to have been spotted
                        _myMissionScore.SpottedScoreCounter += 1;
                        _scoreCount = true;
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // when the enemy doesn't hear the footsteps anymore, the spotted bar will get down
            if (other.CompareTag("FootSteps"))
            {
                _playerInHearField = false;
                _useSpottedBar = false;
            }
        }

        /// <summary>
        /// the distance between the sound event and the enemy
        /// </summary>
        /// <returns></returns>
        public float DistanceToSoundEvent()
        {
            return Vector3.Distance(_agent.pathEndPosition, transform.position);
        }
        

    }
}


