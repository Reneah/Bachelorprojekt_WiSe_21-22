using System;
using System.Collections.Generic;
using BP._02_Scripts._03_Game;
using Enemy.AnimationHandler;
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

        // need this script to get the transform information and some methods of the player
        private PlayerController _player;

        // need this script so that the enemy knows on which ground the player is 
        private PlayerGroundDetection _playerGroundDetection;

        public PlayerGroundDetection PlayerGroundDetection
        {
            get => _playerGroundDetection;
            set => _playerGroundDetection = value;
        }

        public PlayerController Player => _player;

        private EnemyAnimationHandler _animationHandler;
        public EnemyAnimationHandler AnimationHandler => _animationHandler;

        private EnemyTalkCheck _enemyTalkCheck;

        private MissionScore _myMissionScore;

        public EnemyTalkCheck EnemyTalkCheck
        {
            get => _enemyTalkCheck;
            set => _enemyTalkCheck = value;
        }

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
        [Range(0.5f, 5)]
        [SerializeField] private float _lowGroundCatchDistance = 2;
        [Tooltip("set the distance to catch the player")]
        [Range(1, 5)]
        [SerializeField] private float _highGroundCatchDistance = 2;
        [Tooltip("the speed which the enemy will chase the player")]
        [Range(1,10)]
        [SerializeField] private float _chaseSpeed;
        // give the enemy the chance to chase the player again
        // otherwise at a quick turn of the player or other situations, the enemy can't see him anymore, but should have an awareness that the player is next to or behind him. That is more realistic
        private float _reminderTime = 0;
        
        // determines if the first enemy reached the destination of the player nearby so that the other ones can gather around
        private bool _firstEnemyReachedDestination;

        // when the enemy is near another enemy who has sighted the player he will chase him as well
        private bool _activateChasing = false;

        // activate and deactivate the chase activation to pull other nearby enemies to chase the player 
        private GameObject _chaseActivationObject;

        // the time window where this enemy can pull other enemies to chase the player nearby
        private float _activateChaseCooldown = 0.1f;

        // the position on the NavMesh around the current player position that is reachable
        private NavMeshHit _hit;

        // when the enemy spotted the player, the score for the player will change
        private bool _scoreCount;

        public NavMeshHit Hit
        {
            get => _hit;
            set => _hit = value;
        }

        public float ActivateChaseCooldown
        {
            get => _activateChaseCooldown;
            set => _activateChaseCooldown = value;
        }

        public GameObject ChaseActivationObject
        {
            get => _chaseActivationObject;
            set => _chaseActivationObject = value;
        }

        public bool ActivateChasing
        {
            get => _activateChasing;
            set => _activateChasing = value;
        }

        public float ReminderTime
        {
            get => _reminderTime;
            set => _reminderTime = value;
        }

        #endregion

        #region PatrolVariables
        
        [Header("Patrol Behaviour")]
        [Tooltip("the parent of the waypoints")]
        [SerializeField] private GameObject _patrollingRoute;
        [Tooltip("the time which the enemy is standing at a waypoint")]
        [Range(0,10)]
        [SerializeField] private float _dwellingTimer = 1;
        [Range(0,5)]
        [Tooltip("the distance where the enemy will stop when arriving at a waypoint")]
        [SerializeField] private float _stopDistance = 0.5f;
        [Tooltip("the patrol speed of the enemy")] 
        [Range(0,10)]
        [SerializeField] private float _patrolSpeed;
        
        // determines the current patrol points of the enemy
        private int _patrolPointsCounter = 0;
        // activates the dwelling time when arriving a waypoint
        private bool _reachedWaypoint = false;
        // the time which the enemy is standing at a waypoint
        private float _standingCooldown = 0;
        // the list of the enemy patrol points
        List<Transform> _patrolPoints = new List<Transform>();
        
        public bool Patrolling
        {
            get => _patrolling;
            set => _patrolling = value;
        }
        
        public float PatrolSpeed
        {
            get => _patrolSpeed;
            set => _patrolSpeed = value;
        }
        
        #endregion
        
        #region FieldOfViewVariables
        
        [Header("FieldOfView")]
        [Tooltip("the mask for the registration of the obstacle in the view field")]
        [SerializeField] private LayerMask _obstructionMask;
        [Tooltip("the enemy head Transform to have the origin for the view field")]
        [SerializeField] private Transform _enemyHead;
        [Tooltip("the raycast position to know obstacles")] 
        [SerializeField] private Transform _obstacleRaycastTransform;
        [Tooltip("the look position when the player is spotted")]
        [SerializeField] private Transform _lookPositionAtSpotted;
        [Tooltip("the delay time that the player get spotted in the view field")]
        [Range(0,10)]
        [SerializeField] private float _visionSecondsToSpott;
        [Tooltip("the delay time that the player get spotted in the hear radius")]
        [Range(0,10)]
        [SerializeField] private float _acousticSecondsToSpott;
        [SerializeField] private Image _spottedBar;
        [Tooltip("the distance where you get spotted instantly")]
        [Range(0,10)]
        [SerializeField] private float _spottedVisionDistance;
        [Tooltip("the distance where you get spotted instantly")]
        [Range(0,10)]
        [SerializeField] private float _spottedAcousticDistance;
        [Tooltip("the time which will still set the player as destination after out of sight to simulate the awareness that the player ran in the direction")]
        [Range(0,10)]
        [SerializeField] private float _lastChanceTime;
        [Tooltip("the view cone that will eb activated when the player is on high ground")]
        [SerializeField] private GameObject _highGroundViewCone;
        [Tooltip("the view cone that will be activated when the player is on low ground")]
        [SerializeField] private GameObject _lowGroundViewCone;

        // when the player is in the view field, the spotted time for the vision will be used
        private bool _playerInViewField = false;

        public bool PlayerInViewField
        {
            get => _playerInViewField;
            set => _playerInViewField = value;
        }

        // the time to spot the enemy when he is in the view field
        private float _spotTime = 0;
        
        // determines if the enemy is able to see the player or not, but he is not spotted
        private bool _canSeePlayer = false;

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
            get => _useSpottedBar;
            set => _useSpottedBar = value;
        }

        public bool PlayerSpotted
        {
            get => _playerSpotted;
            set => _playerSpotted = value;
        }

        public float SpotTime
        {
            get => _spotTime;
            set => _spotTime = value;
        }
        
        public Transform LookPositionAtSpotted
        {
            get => _lookPositionAtSpotted;
            set => _lookPositionAtSpotted = value;
        }

        public Transform ObstacleRaycastTransform
        {
            get => _obstacleRaycastTransform;
            set => _obstacleRaycastTransform = value;
        }

        public Transform EnemyHead
        {
            get => _enemyHead;
            set => _enemyHead = value;
        }
        
        public bool CanSeePlayer
        {
            get => _canSeePlayer;
            set => _canSeePlayer = value;
        }
        
        public LayerMask ObstructionMask
        {
            get => _obstructionMask;
            set => _obstructionMask = value;
        }

        private bool _inChaseState = false;
        public bool InChaseState
        {
            get => _inChaseState;
            set => _inChaseState = value;
        }
        
        public Image SpottedBar
        {
            get => _spottedBar;
            set => _spottedBar = value;
        }

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
        [Tooltip("the enemy run speed when he goes from point to point")]
        [SerializeField] private float _searchSpeed;
        [Range(1,5)]
        [Tooltip("the max amount of waypoint the enemy will pick around the throw position after checking the noisy item")]
        [SerializeField] private int _throwWaypointCounter;
        [Range(1,5)]
        [Tooltip("the max amount of waypoint the enemy will pick around the player position after chasing")]
        [SerializeField] private int _playerSearchWaypointCounter;
        [Tooltip("layer mask to block the acoustic through objects")]
        [SerializeField] private LayerMask _blockAcousticLayerMasks;
        // signalize when the enemy is finish with searching to go back to his routine
         private bool _finishChecking = false;
         // the closest waypoint to the current position of the throw position or player
         private float _closestWaypointDistance;
         // the transform of the closest current waypoint for the enemy
         private Transform _closestWaypoint;
         // the current waypoint distance when searching the closest waypoint of the player or throw position
         private float _currentwaypointDistance;
         // the current sound state of the item to update the behaviour of the enemy
         private int _currentSoundStage = 0;
         // prevent that the animation will be activated permanently in Update
         private bool _animationActivated = false;
         // update the agent destination of the enemy when the footsteps were heard
         private bool _heardFootsteps = false;
         // get all waypoints in the search area again after using them
         private bool _resetSearchWaypoints = false;
         // the max amount of waypoint the enemy will pick around the throw position after checking the noisy item
         private int _throwWaypointAmount;
         // the max amount of waypoint the enemy will pick around the player position
         private int _searchWaypointAmount;
         // the possible waypoint that the enemy can pick when he investigated the noisy item
         List<Transform> _noisyItemSearchPoints = new List<Transform>();
         // the closest waypoints of the throw position or near the noisy item
         List<Transform> _noisyItemSelectedPoints = new List<Transform>();
         // the possible waypoint that the enemy can pick when he searches the player
         List<Transform> _searchWaypoints = new List<Transform>();
         // the closest waypoints of the player position
         List<Transform> _searchSelectedPoints = new List<Transform>();
         // the amount of the waypoints that the enemy has when the player activates the noisy item in close range
         private int _usuableWaypointsAmount = 1;
         // the amount of the waypoints that the enemy has when the player activates the noisy item in long range
         private int _usuableWaypointsRangeAmount = 1;
         // the amount of the waypoints that the enemy has when the player search for the player
         private int _usuableSearchPointAmount = 1;
         // the current closest waypoint for the enemy when the player activate the item in close range
         private Transform _currentCloseNoisyItemWaypoint;
         // the current closest waypoint for the enemy when the enemy searches the player
         private Transform _currentSearchWaypoint;
         // get all waypoints in the search area again after using them
         private bool _resetNoisyItemWaypoints = false;
         
         // the enemy is able to investigate the noisy item when he heard the sound and the max enemy pull amount is not reached
         private bool _canInvestigate = false;
         // because the trigger enter gets more calls than one at the sound tag, I counter it with this bool
         private bool _getSoundOnce = false;
         // when the player enters the hear field, the spotted time will be activated
         private bool _playerInHearField = false;
         // the collider of the hear field so that when it is deactivated, the Trigger Exit function can be replaced to stop the spotted time
         private Collider _hearFieldPlayerCollider;
         // need this script to get the collider of the step sound 
         private PlayerStepsSound _playerStepsSound;
         
         public bool CanInvestigate
         {
             get => _canInvestigate;
             set => _canInvestigate = value;
         }

         public bool GetSoundOnce
         {
             get => _getSoundOnce;
             set => _getSoundOnce = value;
         }

         public bool ResetNoisyItemWaypoints
         {
             get => _resetNoisyItemWaypoints;
             set => _resetNoisyItemWaypoints = value;
         }
         
         public bool ResetSearchWaypoints
         {
             get => _resetSearchWaypoints;
             set => _resetSearchWaypoints = value;
         }

         public bool HeardFootsteps
         {
             get => _heardFootsteps;
             set => _heardFootsteps = value;
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

         public float SearchSpeed
         {
             get => _searchSpeed;
             set => _searchSpeed = value;
         }

         public bool FinishChecking
         {
             get => _finishChecking;
             set => _finishChecking = value;
         }

         public float FirstStageRunSpeed
        {
            get => _firstStageRunSpeed;
            set => _firstStageRunSpeed = value;
        }

        public float SecondStageRunSpeed
        {
            get => _secondStageRunSpeed;
            set => _secondStageRunSpeed = value;
        }

        public float ThirdStageRunSpeed
        {
            get => _thirdStageRunSpeed;
            set => _thirdStageRunSpeed = value;
        }

        // determines if the enemy heard a sound
        private bool _soundNoticed = false;
        // the current sound stage that influence the enemy behaviour
        private int _soundBehaviourStage = 0;
        // the position from where the sound came so that the enemy knows where to move
        private Transform _soundEventPosition;

        public Transform SoundEventPosition
        {
            get => _soundEventPosition;
            set => _soundEventPosition = value;
        }

        public bool SoundNoticed
        {
            get => _soundNoticed;
            set => _soundNoticed = value;
        }

        public int SoundBehaviourStage
        {
            get => _soundBehaviourStage;
            set => _soundBehaviourStage = value;
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
        [Tooltip("the route where the enemy should look")]
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
        public float StopGuardpointDistance => _stopGuardpointDistance;
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
        
        #endregion
        
        
        void Start()
        {
            _highGroundViewCone.SetActive(false);
            
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
            _chaseActivationObject = transform.Find("EnemyChaseActivation").GetComponent<ChaseActivation.ChaseActivation>().gameObject;
            _myMissionScore = FindObjectOfType<MissionScore>();
            
            // designer can choose between patrolling or guarding mode. The enemy will use only one mode as routine
            if (_patrolling)
            {
                SetUpPatrolBehaviour(); 
            }
            else if(_guarding)
            {
                SetUpGuardBehaviour(); 
            }

            // set the max amount of waypoint the enemy will pick around the throw position after checking the noisy item
            _throwWaypointAmount = _throwWaypointCounter;
            _searchWaypointAmount = _playerSearchWaypointCounter;
            
            // the dwelling time at a waypoint
            _standingCooldown = _dwellingTimer;
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
            
            PlayerVisionDetection();
            ActivateNoisyItemInvestigation();
        }
        
        #region ChaseBehaviour
        
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
  
            }

            if (_playerGroundDetection.HighGround)
            {
                // Bigger View Cone
                _lowGroundViewCone.SetActive(false);
                _highGroundViewCone.SetActive(true);
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
            // when the first enemy reached the destination, the enemy will be taken to signalize that the other have to stop around the destination 
            if (EnemyShareInformation.EnemyInstance != null && !EnemyShareInformation.EnemyInstance._agent.isStopped) 
            { 
                EnemyShareInformation.FirstEnemyReachedDestination = false; 
            } 
            
            // prevent that the run animation is playing when the agent can't go further in contrast to the player 
            // rotates the enemy towards the player position 
            // first if condition: first enemy reached the destination - second if condition: when more than one enemy reaches around the destination, they will stop 
            if (ClosestPlayerPosition(1)|| ClosestPlayerPosition(2.5f) && EnemyShareInformation.FirstEnemyReachedDestination && _playerGroundDetection.HighGround)
            {
                if (!EnemyShareInformation.FirstEnemyReachedDestination)
                {
                    EnemyShareInformation.EnemyInstance = this;
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
        /// set the standing cooldown and add the waypoints to the list
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
        /// set the first destination point patrol speed
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
            if (Vector3.Distance(_currentLookPosition.transform.position, _currentLookPoint.transform.position ) <= _stopGuardpointDistance && !_reachedLookPoint)
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
            return Vector3.Distance(transform.position, _guardPoint.transform.position) <= StopGuardpointDistance;
        }

        /// <summary>
        /// the desired standing position of the enemy at the guard point
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
        public void PlayerVisionDetection()
        {
            if (!_hearFieldPlayerCollider.enabled)
            {
                _useSpottedBar = false;
            }
            
            // when the enemy sees the player, he will get spotted in a fixed time when he stays in the view field
            if (_useSpottedBar)
            {
                float distance = Vector3.Distance(transform.position, _player.transform.position);

                // the time will run and will fill the bar until the player is spotted
                if (_spotTime < _visionSecondsToSpott && _playerInViewField)
                {
                    _spotTime += Time.deltaTime / _visionSecondsToSpott;
                    _spottedBar.fillAmount = _spotTime;
                }
                
                // NOTE: extra if condition, when the player get the footstep tag
                if (_playerInHearField && _spotTime < _acousticSecondsToSpott)
                {
                    _spotTime += Time.deltaTime / _acousticSecondsToSpott;
                    _spottedBar.fillAmount = _spotTime;
                }
                
                // when the player is to close to the enemy or to long in the view field, the player get spotted
                if (_spottedBar.fillAmount >= 1 || distance <= _spottedVisionDistance  || distance <= _spottedAcousticDistance)
                {
                    _spotTime = 1;
                    _spottedBar.fillAmount = _spotTime;
                    _playerSpotted = true;
                    _player.PlayerAnimationHandler.PlayerFlee(true);
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
        
        private void OnTriggerEnter(Collider other)
        {
            // when the enemy hears a sound, he will investigate it when the max enemy pull amount is not reached
            if (other.CompareTag("Sound") && !_getSoundOnce)
            {
                Vector3 _raycastDirection = new Vector3(_player.transform.position.x, _player.transform.position.y + 2.5f, _player.transform.position.z) - _enemyHead.position;
                // when a specific layer is hit that isolate sound, the enemy will hear nothing
                if (Physics.Raycast(_enemyHead.position, _raycastDirection, 10, _blockAcousticLayerMasks))
                {
                    return;
                }
                
                _getSoundOnce = true;
                
                // get the current noisy item script of the item
                _noisyItemScript = other.GetComponentInParent<NoisyItem>();
                // add the enemy to the list and start the cooldown to choose the closest enemies
                _noisyItemScript.EnemyList.Add(this);
                _noisyItemScript.StartPullCountdown = true;
            }
            
            // if the enemy gets in a new room, the old search points will be deleted and the new ones will be selected
            if (other.CompareTag("SearchPoints"))
            {
                _searchWaypoints.Clear();
                _noisyItemSearchPoints.Clear();

                foreach (Transform waypoints in other.transform)
                {
                    _searchWaypoints.Add(waypoints);
                    _noisyItemSearchPoints.Add(waypoints);
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            // when the enemy hears the footsteps of the player, he knows that he is nearby, so he is spotted and will run to the player position
            if (other.CompareTag("FootSteps"))
            {
                Vector3 _raycastDirection = new Vector3(_player.transform.position.x, _player.transform.position.y + 2.5f, _player.transform.position.z) - _enemyHead.position;
                // when a specific layer is hit that isolate sound, the enemy will hear nothing
                if (Physics.Raycast(_enemyHead.position, _raycastDirection,  10, _blockAcousticLayerMasks))
                {
                    _useSpottedBar = false;
                    return;
                }
            
                _playerInHearField = true;
                _useSpottedBar = true;
                
                if (_playerSpotted)
                {
                    _player.PlayerAnimationHandler.PlayerFlee(true);
                    _soundNoticed = true;
                    _soundBehaviourStage = 3;
                    _soundEventPosition = _player.transform;
                    _animationActivated = false;
                    _heardFootsteps = true;
                    _spottedBar.fillAmount = 1;
                
                    if (!_scoreCount)
                    {
                        // Counts up the mission score for the player to have been spotted
                        _myMissionScore.SpottedScoreCounter += 1;
                        _scoreCount = true;
                    }

                }

            }
            
            // if the enemy used the points in the room, all points will be added again because used points will be deleted during the search mode
            if (other.CompareTag("SearchPoints"))
            {

                if (_resetSearchWaypoints)
                {
                    _searchWaypoints.Clear();
                    
                    foreach (Transform waypoints in other.transform)
                    {
                        _searchWaypoints.Add(waypoints);
                    }

                    _resetSearchWaypoints = false;
                }
                
                if (_resetNoisyItemWaypoints)
                {
                    _noisyItemSearchPoints.Clear();
                    _noisyItemSelectedPoints.Clear();
                    
                    foreach (Transform waypoints in other.transform)
                    {
                        _noisyItemSearchPoints.Add(waypoints);
                    }

                    _resetNoisyItemWaypoints = false;
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
        
        #region SearchBehaviour

        public void PrepareSearchBehaviour()
        {
            // get the current closest point based on the player position
            _closestWaypointDistance = Mathf.Infinity;
            
            foreach (Transform waypoint in _searchWaypoints)
            {
                _currentwaypointDistance = Vector3.Distance(waypoint.transform.position, _player.transform.position);
                if (_currentwaypointDistance <= _closestWaypointDistance)
                {
                    _closestWaypointDistance = _currentwaypointDistance;
                    _closestWaypoint = waypoint;
                }
            }
            // when the fixed waypoints amount is reached, this method will be stopped
            if (_searchWaypointAmount > 0)
            {
                _searchSelectedPoints.Add(_closestWaypoint);
                _searchWaypoints.Remove(_closestWaypoint);
                _searchWaypointAmount--;
                PrepareSearchBehaviour();

                // reset the amount of desired waypoints
                if (_searchWaypointAmount <= 1)
                {
                    _searchWaypointAmount = _playerSearchWaypointCounter;
                }
            }
            
            // the amount of waypoints that can be used of the selected
            _usuableSearchPointAmount = _searchSelectedPoints.Count;
        }
        
        /// <summary>
        /// set the closest waypoint based on the player position
        /// </summary>
        public void StartSearchBehaviour()
        {
            // when all search points has been used, the enemy goes back to patrolling or guarding
            if (_usuableSearchPointAmount <= 0)
            {
                _resetSearchWaypoints = true;
                _finishChecking = true;
                return;
            }
            
            _currentSearchWaypoint = _searchSelectedPoints[Random.Range(0, _searchSelectedPoints.Count)];
            if (_currentSearchWaypoint == null)
            {
                StartSearchBehaviour();
                return;
            }
            _animationHandler.SetSpeed(_searchSpeed);
            _agent.SetDestination(_currentSearchWaypoint.position);
            _usuableSearchPointAmount--;
                
            _searchSelectedPoints.Remove(_currentSearchWaypoint);
        }
        
        /// <summary>
        /// loop through the search points
        /// </summary>
        public void UpdateSearchBehaviour()
        {
            // enemy has lost the player and reset the score count ability, so next time the player is spotted,
            // the score will be counted up again
            _scoreCount = false;
            
            // when the current search point position is reached, the standing time will count down and set the next search point
            if (Vector3.Distance(transform.position, _currentSearchWaypoint.position) <= _stopDistance && !_reachedWaypoint)
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
                    StartSearchBehaviour();
                }
            }
        }
        #endregion
        
        #region SearchNoisyItemBehaviour

        /// <summary>
        /// get the closest search points of the throw position of the player
        /// </summary>
        public void PrepareSearchNoisyItemBehaviour()
        {
            // when the enemy has thrown a stone, the closest waypoints of the throw position will be selected
            if (_player.PlayerThrowTrigger.PlayerThrew)
            {
                _closestWaypointDistance = Mathf.Infinity;
                foreach (Transform waypoint in _noisyItemSearchPoints)
                {
                    _currentwaypointDistance = Vector3.Distance(waypoint.transform.position, _player.PlayerThrowTrigger.ThrowPosition.transform.position);
                    if (_currentwaypointDistance <= _closestWaypointDistance)
                    {
                        _closestWaypoint = waypoint;
                        _closestWaypointDistance = _currentwaypointDistance;
                    }
                }
                // when the fixed waypoints amount is reached, this method will be stopped
                if (_throwWaypointAmount > 0)
                {
                    _noisyItemSelectedPoints.Add(_closestWaypoint);
                    _noisyItemSearchPoints.Remove(_closestWaypoint);
                    _throwWaypointAmount--;
                    PrepareSearchNoisyItemBehaviour();

                    // reset the amount of desired waypoints
                    if (_throwWaypointAmount <= 1)
                    {
                        _throwWaypointAmount = _throwWaypointCounter;
                    }
                }
                
                // the amount of waypoints that can be used of the selected
                _usuableWaypointsRangeAmount = Random.Range(1, _noisyItemSelectedPoints.Count);
            }
            
        }
        
        /// <summary>
        /// select the waypoint where the enemy has to go
        /// </summary>
        public void StartSearchNoisyItemBehaviour()
        {
            // when all points were used, the enemy will go back to his routine
            if (_usuableWaypointsAmount <= 0 || _usuableWaypointsRangeAmount <= 0)
            {
                _finishChecking = true;
                _resetNoisyItemWaypoints = true;
                return;
            }

            // when the enemy activated the item with a throw, the waypoints near the throw position will be used
            if (_player.PlayerThrowTrigger.PlayerThrew)
            {
                _currentCloseNoisyItemWaypoint = _noisyItemSelectedPoints[Random.Range(0, _noisyItemSelectedPoints.Count)];
                
                if (_currentCloseNoisyItemWaypoint == null)
                {
                    StartSearchNoisyItemBehaviour();
                    return;
                }
                
                _agent.SetDestination(_currentCloseNoisyItemWaypoint.position);
                _usuableWaypointsRangeAmount--;
                
                _noisyItemSelectedPoints.Remove(_currentCloseNoisyItemWaypoint);
            }

            // when the enemy activated the item in close range, selected waypoints in the inspector will be used
            if (!_player.PlayerThrowTrigger.PlayerThrew)
            {
                _currentCloseNoisyItemWaypoint = _noisyItemScript.CloseNoisyItemWaypoints[Random.Range(0, _noisyItemScript.CloseNoisyItemWaypoints.Length)];
                
                if (_currentCloseNoisyItemWaypoint == null)
                {
                    StartSearchNoisyItemBehaviour();
                    return;
                }
                
                _agent.SetDestination(_currentCloseNoisyItemWaypoint.position);
                _usuableWaypointsAmount--;
                
                _noisyItemSelectedPoints.Remove(_currentCloseNoisyItemWaypoint);
            }
            
            _animationHandler.SetSpeed(_firstStageRunSpeed);
        }
        
        /// <summary>
        /// loop through the noisy item search points
        /// </summary>
        public void UpdateSearchNoisyItemBehaviour()
        {
            // when the current search point position is reached, the standing time will count down and set the next search point
            if (Vector3.Distance(transform.position, _currentCloseNoisyItemWaypoint.position) <= _stopDistance && !_reachedWaypoint)
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
                    StartSearchNoisyItemBehaviour();
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
                _usuableWaypointsAmount = Random.Range(1,_noisyItemScript.CloseNoisyItemWaypoints.Length);
                
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
    }
}


