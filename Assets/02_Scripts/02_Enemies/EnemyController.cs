using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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
    
    // the current state of the player
    private IEnemyState _currentState;
    public static readonly EnemyPatrolState EnemyPatrolState = new EnemyPatrolState();
    public static readonly EnemyIdleState EnemyIdleState = new EnemyIdleState();

    [Header("Start Behaviour")] 
    [SerializeField] private bool _patrolling;

    #region PatrolVariables
    
    [Header("Patrol Skill")]
    [Tooltip("the parent of the waypoints")]
    [SerializeField] private GameObject _patrollingRoute;
    [Tooltip("the time which the enemy is standing at a waypoint")]
    [SerializeField] private float _dwellingTimer = 1;
    [Tooltip("the distance where the enemy will stop when arriving at a waypoint")]
    [SerializeField] private float _stopDistance = 0.5f;
    
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
    
    #endregion
    
    #region FieldOfViewVariables
    
    [Header("FieldOfView")]
    [Tooltip("the radius of the view field")]
    [SerializeField] private float _radius;
    [Tooltip("the angle of the view field")]
    [Range(0,360)]
    [SerializeField] private float _angle;
    [Tooltip("the reference of the player")]
    [SerializeField] private GameObject _playerRef;
    [Tooltip("the mask for the registration of the player in the view field")]
    [SerializeField] private LayerMask _targetMask;
    [Tooltip("the mask for the registration of the obstacle in the view field")]
    [SerializeField] private LayerMask obstructionMask;
    [Tooltip("determine the wait time in seconds for every view field check")]
    [SerializeField] float delay = 0.2f;
    private bool _canSeePlayer;
    
    
    public bool CanSeePlayer
    {
        get => _canSeePlayer;
        set => _canSeePlayer = value;
    }
    
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
    public GameObject PlayerRef
    {
        get => _playerRef;
        set => _playerRef = value;
    }
    
    #endregion
    
    void Start()
    {
        // start state machine with LookAroundState
        _currentState = EnemyIdleState;

        _agent = GetComponent<NavMeshAgent>();
        _player = GetComponent<PlayerController>();
        
        SetUpPatrolBehaviour();
        StartCoroutine(FOVRoutine());
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
    }
    
    /// <summary>
    /// should rotate to the player
    /// </summary>
    public void RotateToPlayer()
    {
        Vector3 lookToPlayer = (_player.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(lookToPlayer), Time.deltaTime * 5);
    }
    
    public void RunTowardsPlayer()
    {
        _agent.isStopped = false;
        _agent.SetDestination(_player.transform.position);
        //_enemyAnimationController.RunAnimation();
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
        if (!_agent.hasPath)
        {
            _agent.SetDestination(_waypoints[_waypointsCounter].position);
        }
    }
    
    public void UpdatePatrolBehaviour()
    {
        if (Vector3.Distance(transform.position, _waypoints[_waypointsCounter].transform.position) < _stopDistance && !_reachedWaypoint)
        {
            _reachedWaypoint = true;
        }

        if (_reachedWaypoint)
        {
            _standingCooldown -= Time.deltaTime;

            if (_standingCooldown <= 0)
            {
                _waypointsCounter++;
                _waypointsCounter %= _patrollingRoute.transform.childCount;
                _standingCooldown = _dwellingTimer;
                _agent.SetDestination(_waypoints[_waypointsCounter].position);
                _reachedWaypoint = false;
            }
        }
    }
    
    #endregion

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
            Transform target = rangeChecks[0].transform;
            // the direction from the enemy to the player
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            // checks if the player is in the angle in front of the enemy
            bool playerIsVisible = Vector3.Angle(transform.forward, directionToTarget) < _angle / 2;
            if (playerIsVisible)
            {
                // the distance from the enemy to the player
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                bool obstructedView = Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask);
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
}
