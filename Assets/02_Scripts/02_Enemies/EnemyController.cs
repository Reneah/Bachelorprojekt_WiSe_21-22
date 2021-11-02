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

    [Header("Start Behaviour")] 
    [SerializeField] private bool _patrolling;

    [Header("Chase Behaviour")] 
    [Tooltip("set the distance to catch the player")]
    [SerializeField] private float _catchDistance = 2;
    [Tooltip("the speed which the enemy will chase the player")]
    [SerializeField] private float _chaseSpeed;

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
 //   [Tooltip("the radius of the view field")]
  //  [SerializeField] private float _radius;
 //   [Tooltip("the angle of the view field")]
  //  [Range(0,360)]
   // [SerializeField] private float _angle;
    //[Tooltip("the mask for the registration of the player in the view field")]
    [SerializeField] private LayerMask _targetMask;
    [Tooltip("the mask for the registration of the obstacle in the view field")]
    [SerializeField] private LayerMask obstructionMask;
   // [Tooltip("determine the wait time in seconds for every view field check")]
    //[SerializeField] float delay = 0.2f;
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

    void Start()
    {
        // start state machine with LookAroundState
        _currentState = EnemyIdleState;

        _agent = GetComponent<NavMeshAgent>();
        _animationHandler = GetComponent<EnemyAnimationHandler>();
        _player = FindObjectOfType<PlayerController>();
        
        SetUpPatrolBehaviour();
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

    // should do a void update method, which the bar will go dynamically back and forth
    // Then it will go automatically back and don't have the problem to set back the bar
    // but the player will be instantly spotted until the patrol state kicks in

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
/*
    #region FieldOfViewBehaviour
    
    /// <summary>
    /// checks the field of view every given seconds whether the player is in or not. This safe some performance
    /// </summary>
    /// <returns></returns>
    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(delay);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        // checks if the player is in the radius of the enemy
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, _radius, _targetMask);

        if (rangeChecks.Length != 0)
        {
            // there is only one player in the game, so the array can be set to 0
            Vector3 target = rangeChecks[0].transform.position;
            target = new Vector3 (target.x, 1.3f, target.z);
            // the direction from the enemy to the player
            Vector3 directionToTarget = (target - _enemyHead.position).normalized;

            // checks if the player is in the angle in front of the enemy
            bool playerIsVisible = Vector3.Angle(_enemyHead.forward, directionToTarget) < _angle / 2;
            if (playerIsVisible)
            {
                // the distance from the enemy to the player
                float distanceToTarget = Vector3.Distance(transform.position, target);
                
                // check if there is a obstacle in the way to see the player
                bool obstructedView = Physics.Raycast(_obstacleRaycastTransform.position, directionToTarget, distanceToTarget, obstructionMask);
                if (!obstructedView)
                {
                    _canSeePlayer = true;
                }
                else
                {
                    _canSeePlayer = false;
                }
            }
            else
            {
                _canSeePlayer = false;
            }
        }
        else if(_canSeePlayer)
        {
            _canSeePlayer = false;
        }
    }
    
    #endregion
*/
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
                Debug.Log("Stage: 0");
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
            }
            
            // deactivate the sound collider
            other.GetComponent<Collider>().gameObject.SetActive(false);
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
