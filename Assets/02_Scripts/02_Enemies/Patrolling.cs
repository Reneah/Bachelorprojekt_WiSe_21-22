using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patrolling : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
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
    
    private bool _startMoving = false;
    
    void Start()
    {
         _standingCooldown = _dwellingTimer;
        
        foreach (Transform waypoints in _patrollingRoute.transform)
        {
            _waypoints.Add(waypoints);
        }
        StartPatrolBehaviour();
    }

    private void Update()
    {
        if (_startMoving)
        {
            UpdatePatrolBehaviour();
        }
    }

    private void StartPatrolBehaviour()
    {
        if (!_agent.hasPath)
        {
            _agent.SetDestination(_waypoints[_waypointsCounter].position);

            _startMoving = true;
        }
    }

    private void UpdatePatrolBehaviour()
    {
        if (Vector3.Distance(transform.position, _waypoints[_waypointsCounter].transform.position) < _stopDistance && !_reachedWaypoint)
        {
            _reachedWaypoint = true;
        }

        if (_reachedWaypoint)
        {
            _dwellingTimer -= Time.deltaTime;

            if (_dwellingTimer <= 0)
            {
                _waypointsCounter++;
                _waypointsCounter %= _patrollingRoute.transform.childCount;
                _dwellingTimer = 2;
                _agent.SetDestination(_waypoints[_waypointsCounter].position);
                _reachedWaypoint = false;
            }
        }
    }
}
