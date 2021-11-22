using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using untitledProject;

public class EnemyController : MonoBehaviour
{
    private NavMeshAgent _agent;

    public NavMeshAgent Agent
    {
        get => _agent;
        set => _agent = value;
    }

    private PlayerController _player;

    public PlayerController Player
    {
        get => _player;
        set => _player = value;
    }

    private EnemyAnimationHandler _animationHandler;

    public EnemyAnimationHandler AnimationHandler
    {
        get => _animationHandler;
        set => _animationHandler = value;
    }
    
    // the current state of the player
    private IEnemyState _currentState;
    public static readonly EnemyPatrolState EnemyPatrolState = new EnemyPatrolState();
    public static readonly EnemyIdleState EnemyIdleState = new EnemyIdleState();
    public static readonly EnemyChaseState EnemyChaseState = new EnemyChaseState();
    public static readonly EnemySearchState EnemySearchState = new EnemySearchState();
    public static readonly EnemySoundInvestigationState EnemySoundInvestigationState = new EnemySoundInvestigationState();
    public static readonly EnemyGuardState EnemyGuardState = new EnemyGuardState();

    [Header("Choose ONE of the Behaviour")] 
    [SerializeField] private bool _patrolling;
    [SerializeField] private bool _guarding;

    public bool Guarding
    {
        get => _guarding;
        set => _guarding = value;
    }

    #region ChaseBehaviour

    [Header("Chase Behaviour")]
    [Tooltip("set the distance to catch the player")]
    [SerializeField] private float _catchDistance = 2;
    [Tooltip("the speed which the enemy will chase the player")]
    [SerializeField] private float _chaseSpeed;
    // give the enemy the chance to chase the player again
    // otherwise at a quick turn of the player, the enemy can't see him anymore, but should have an awareness that the player is next to or behind him. That is more realistic
    private float _reminderTime = 0;
    
    public float ReminderTime
    {
        get => _reminderTime;
        set => _reminderTime = value;
    }

    #endregion
    
    #region PatrolVariables
    
    [Header("Patrol Skill")]
    [Tooltip("the parent of the waypoints")]
    [SerializeField] private GameObject _patrollingRoute;
    [Tooltip("the time which the enemy is standing at a waypoint")]
    [SerializeField] private float _dwellingTimer = 1;
    [Tooltip("the distance where the enemy will stop when arriving at a waypoint")]
    [SerializeField] private float _stopDistance = 0.5f;
    [Tooltip("the patrol speed of the enemy")] 
    [SerializeField] private float _patrolSpeed;
    
    // determines the current waypoint of the enemy
    private int _waypointsCounter = 0;
    // activates the dwelling time when arriving a waypoint
    private bool _reachedWaypoint = false;
    // the time which the enemy is standing at a waypoint
    private float _standingCooldown = 0;
    // the list of the enemy waypoints
    List<Transform> _waypoints = new List<Transform>();
    
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

    [SerializeField] private LayerMask _targetMask;
    [Tooltip("the mask for the registration of the obstacle in the view field")]
    [SerializeField] private LayerMask obstructionMask;
    [Tooltip("the enemy head Transform to have the origin for the view field")]
    [SerializeField] private Transform _enemyHead;
    [Tooltip("the raycast position to know obstacles")] 
    [SerializeField] private Transform _obstacleRaycastTransform;
    [Tooltip("the look position when the player is spotted")]
    [SerializeField] private Transform _lookPositionAtSpotted;

    [Tooltip("the delay time that the player get spotted in the view field")]
    [Range(0,10)]
    [SerializeField] private float _secondsToSpott;
    private float _spottedTime = 0;
    [SerializeField] private Image _spottedBar;
    [Tooltip("the distance where you get spotted instantly")]
    [SerializeField] private float _spottedDistance;
    [Tooltip("the time which will still set the player as destination after out of sight to simulate the awareness that the player ran in the direction")]
    [SerializeField] private float _lastChanceTime;

    public float LastChanceTime
    {
        get => _lastChanceTime;
        set => _lastChanceTime = value;
    }

    private bool _playerSpotted = false;
    private bool _seesObject = false;

    public bool SeesObject
    {
        get => _seesObject;
        set => _seesObject = value;
    }

    public bool PlayerSpotted
    {
        get => _playerSpotted;
        set => _playerSpotted = value;
    }

    public float SpottedTime
    {
        get => _spottedTime;
        set => _spottedTime = value;
    }

    public Image SpottedBar
    {
        get => _spottedBar;
        set => _spottedBar = value;
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

    private bool _canSeePlayer = false;
    
    public bool CanSeePlayer
    {
        get => _canSeePlayer;
        set => _canSeePlayer = value;
    }
    /*
    public float Radius
    {
        get => _radius;
        set => _radius = value;
    }
    
    public float Angle
    {
        get => _angle;
        set => _angle = value;
    }
    
    */

    public LayerMask TargetMask
    {
        get => _targetMask;
        set => _targetMask = value;
    }

    public LayerMask ObstructionMask
    {
        get => obstructionMask;
        set => obstructionMask = value;
    }
    
    #endregion

    #region SearchVariables
    
    [Header("Investigation Behaviour")]
    [Tooltip("the enemy run speed to the sound event point at the first sound stage")]
    [SerializeField] private float _firstStageRunSpeed;
    [Tooltip("the enemy run speed to the sound event point at the second sound stage")]
    [SerializeField] private float _secondStageRunSpeed;
    [Tooltip("the enemy run speed to the sound event point at the third sound stage")]
    [SerializeField] private float _thirdStageRunSpeed;
    [Tooltip("the enemy run speed when he goes from point to point")]
    [SerializeField] private float _searchSpeed;
    List<Transform> _searchWaypoints = new List<Transform>();
     private int _searchWaypointCounter = 0;
     private bool _finishChecking = false;
     private float _nearestWaypoint;
     private Transform _closestWaypoint;
     private float _waypointDistance;
     // the current sound state of the item to update the behaviour of the enemy
     private int _currentSoundStage = 0;
     // prevent that the animation will be activated permanently in Update
     private bool _animationActivated = false;
     // update the agent destination of the enemy when the footsteps were heard
     private bool _heardFootsteps = false;

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

    private bool _soundNoticed = false;
    private int _soundBehaviourStage = 0;
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
    [SerializeField] private float _switchLookTime;
    [Tooltip("the speed how fast the invisible agent is moving")]
    [SerializeField] private float _lookSwitchSpeed;
    [Tooltip("the route where the enemy should look")]
    [SerializeField] private GameObject _lookingRoute;
    [Tooltip("the point where the enemy is guarding")]
    [SerializeField] private Transform _guardPoint;
    [Tooltip("the distance to stop when reaching the guard point")]
    [SerializeField] private float _stopGuardpointDistance = 0.5f;
    [Tooltip("the rotation of the enemy body when he is guarding")]
    [SerializeField] private Transform _desiredBodyRotation;
    [Tooltip("the look agent to look in specific directions over time")]
    [SerializeField] private Transform _currentLookPosition;
    [Tooltip("smooth the rotation towards the desired Body Rotation")]
    [SerializeField] private float _smoothBodyRotation;
    private bool _reachedGuardpoint = false;
    private Quaternion _desiredDirection;
    
    public bool ReachedGuardpoint
    {
        get => _reachedGuardpoint;
        set => _reachedGuardpoint = value;
    }

    public Transform CurrentLookPosition
    {
        get => _currentLookPosition;
        set => _currentLookPosition = value;
    }

    public float SmoothBodyRotation
    {
        get => _smoothBodyRotation;
        set => _smoothBodyRotation = value;
    }

    public Transform DesiredBodyRotation
    {
        get => _desiredBodyRotation;
        set => _desiredBodyRotation = value;
    }


    public float StopGuardpointDistance
    {
        get => _stopGuardpointDistance;
        set => _stopGuardpointDistance = value;
    }

    public Transform GuardPoint
    {
        get => _guardPoint;
        set => _guardPoint = value;
    }
    
    // the list of the enemy waypoints
    private List<Transform> _lookpoints = new List<Transform>();
    private float _lookCooldown = 0;
    private int _lookPointcounter = 0;
    private bool _reachedLookpoint = false;
    private bool _guardBehaviour = false;
    private Transform _currentLookpoint;

    public List<Transform> Lookpoints
    {
        get => _lookpoints;
        set => _lookpoints = value;
    }
    
    public bool GuardBehaviour
    {
        get => _guardBehaviour;
        set => _guardBehaviour = value;
    }

    #endregion

    void Start()
    {
        // start state machine with LookAroundState
        _currentState = EnemyIdleState;

        _agent = GetComponent<NavMeshAgent>();
        _animationHandler = GetComponent<EnemyAnimationHandler>();
        _player = FindObjectOfType<PlayerController>();

        if (_patrolling)
        {
            SetUpPatrolBehaviour(); 
        }
        else if(_guarding)
        {
            SetUpGuardBehaviour(); 
        }

        //StartCoroutine(FOVRoutine());
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
        
        PlayerDetected();
    }
    
    /// <summary>
    /// should rotate to the player
    /// </summary>
    public void RotateToPlayer()
    {
        Vector3 lookToPlayer = (_player.transform.position - _enemyHead.position).normalized;
        EnemyHead.rotation = Quaternion.Slerp(EnemyHead.rotation,Quaternion.LookRotation(lookToPlayer), Time.deltaTime * 5);
        transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(lookToPlayer), Time.deltaTime * 3);
    }
    
    public void ChasePlayer()
    {
        _agent.isStopped = false;
        _agent.SetDestination(_player.transform.position);
        _agent.speed = _chaseSpeed;
        _animationHandler.SetSpeed(_chaseSpeed);
    }

    public void HeadRotationTowardsPlayer()
    {
       // Vector3 lookToPlayer = (_player.transform.position - EnemyHead.position).normalized;
       // EnemyHead.rotation = Quaternion.Slerp(EnemyHead.rotation,Quaternion.LookRotation(lookToPlayer), Time.deltaTime * 5);
    }
    
    public bool CatchPlayer()
    {
        return Vector3.Distance(transform.position, _player.transform.position) <= _catchDistance;
    }
    
    #region PatrolBehaviour
    
    private void SetUpPatrolBehaviour()
    {
        _standingCooldown = _dwellingTimer;
        
        foreach (Transform waypoints in _patrollingRoute.transform)
        {
            _waypoints.Add(waypoints);
        }
    }
    
    public void StartPatrolBehaviour()
    {
        _agent.speed = _patrolSpeed;
        _agent.SetDestination(_waypoints[_waypointsCounter].position);
    }
    
    public void UpdatePatrolBehaviour()
    {
        if (Vector3.Distance(transform.position, _waypoints[_waypointsCounter].transform.position) <= _stopDistance && !_reachedWaypoint)
        {
            _reachedWaypoint = true;
        }

        if (_reachedWaypoint)
        {
            _animationHandler.SetSpeed(0);
            _standingCooldown -= Time.deltaTime;

            if (_standingCooldown <= 0)
            {
                _waypointsCounter++;
                _waypointsCounter %= _patrollingRoute.transform.childCount;
                _standingCooldown = _dwellingTimer;
                _agent.SetDestination(_waypoints[_waypointsCounter].position);
                _reachedWaypoint = false;
                _animationHandler.SetSpeed(_patrolSpeed);
            }
        }
    }
    
    #endregion

    #region GuardBehaviour
    
    public void SetUpGuardBehaviour()
    {
        _lookCooldown = _switchLookTime;
        
        foreach (Transform lookpoints in _lookingRoute.transform)
        {
            _lookpoints.Add(lookpoints);
        }
        
        _currentLookpoint = _lookpoints[_lookPointcounter].transform;
        _currentLookPosition.transform.position = _currentLookpoint.transform.position;
    }
    
    public void UpdateGuardBehaviour()
    {
        _currentLookPosition.position = Vector3.MoveTowards(_currentLookPosition.transform.position, _currentLookpoint.transform.position, Time.deltaTime * _lookSwitchSpeed);

        if (Vector3.Distance(_currentLookPosition.transform.position, _currentLookpoint.transform.position ) <= _stopGuardpointDistance && !_reachedLookpoint)
        {
            _reachedLookpoint = true;
        }
        
        if (_reachedLookpoint)
        {
            _lookCooldown -= Time.deltaTime;
            
            if (_lookCooldown <= 0)
            {
                _lookPointcounter++;
                _lookPointcounter %= _lookingRoute.transform.childCount;
                _lookCooldown = _switchLookTime;
                _currentLookpoint = _lookpoints[_lookPointcounter].transform;
                _reachedLookpoint = false;
            }
        }
    }

    public bool GuardPointDistance()
    {
        return Vector3.Distance(transform.position, _guardPoint.transform.position) <= StopGuardpointDistance;
    }

    public void DesiredStandingLookDirection()
    {
        _desiredDirection = Quaternion.Slerp(transform.rotation, DesiredBodyRotation.rotation, SmoothBodyRotation * Time.deltaTime);
        transform.rotation = _desiredDirection;
    }
    
    #endregion
    
    public void PlayerDetected()
    {
        if (_seesObject)
        {
            float distance = Vector3.Distance(transform.position, _player.transform.position);

            if (_spottedTime <= _secondsToSpott)
            {
                _spottedTime += Time.deltaTime;
                _spottedBar.fillAmount = _spottedTime;
            }
            
            if (_spottedTime >= _secondsToSpott || distance <= _spottedDistance)
            {
                _spottedBar.fillAmount = 1;
                _playerSpotted = true;
            }
        }
        else
        {
            if (_spottedTime > 0)
            {
                _spottedTime -= Time.deltaTime;
                _spottedBar.fillAmount = _spottedTime;
            }

            if (_spottedTime <= 0)
            {
                _spottedTime = 0;
                _spottedBar.fillAmount = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sound"))
        {
            SoundItem _soundItemScript = other.GetComponentInParent<SoundItem>();
            float _distance = Vector3.Distance(transform.position, other.transform.position);
            
            RaycastHit hit;
            Physics.Raycast(other.transform.position, transform.position - other.transform.position, out hit, _distance);

            if (hit.collider.CompareTag("Wall"))
            {
                _soundItemScript.Stage--;

                if (_soundItemScript.Stage <= 0)
                {
                    _soundItemScript.Stage = 0;
                    return;
                }
            }

            if(_soundItemScript.Stage <= 3)
            {
                _soundBehaviourStage = _soundItemScript.Stage;
                _soundEventPosition = _soundItemScript.transform;
                _soundNoticed = true;
                _spottedBar.fillAmount = 1;
            }
        }

        if (other.CompareTag("FootSteps"))
        {
            _soundNoticed = true;
            _soundBehaviourStage = 3;
            _soundEventPosition = _player.transform;
            _animationActivated = false;
            _heardFootsteps = true;
        }
        
        // if the enemy get in a new room the new search points will be selected
        if (other.CompareTag("SearchPoints"))
        {
            _searchWaypoints.Clear();
            
            foreach (Transform waypoints in other.transform)
            {
                // have to delete the list after using
                _searchWaypoints.Add(waypoints);
            }
        }
    }
    /// <summary>
    /// the distance between the sound event and the enemy
    /// </summary>
    /// <returns></returns>
    public float DistanceToSoundEvent()
    {
        return Vector3.Distance(SoundEventPosition.position, transform.position);
    }
    
    #region Search Behaviour
    
    public void StartSearchBehaviour()
    {
        if (_searchWaypoints.Count <= 0)
        {
            _finishChecking = true;
            return;
        }
        
        _nearestWaypoint = Mathf.Infinity;
        foreach (Transform waypoint in _searchWaypoints)
        {
             _waypointDistance = Vector3.Distance(waypoint.transform.position, _player.transform.position);
            if (_waypointDistance <= _nearestWaypoint)
            {
                _nearestWaypoint = _waypointDistance;
                _closestWaypoint = waypoint;
            }
        }
        _animationHandler.SetSpeed(_searchSpeed);
        _agent.SetDestination(_closestWaypoint.position);
    }
    
    public void UpdateSearchBehaviour()
    {
        if (Vector3.Distance(transform.position, _closestWaypoint.position) <= _stopDistance && !_reachedWaypoint)
        {
            _animationHandler.SetSpeed(0);
            _reachedWaypoint = true;
        }

        if (_reachedWaypoint)
        {
            //kick out the waypoint which was already used
            _searchWaypoints.Remove(_closestWaypoint); 
            
            //plays investigation animation, after it or certain time he will go to the next point nearby
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
}
