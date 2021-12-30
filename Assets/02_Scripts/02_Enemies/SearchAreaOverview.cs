using System;
using System.Collections;
using System.Collections.Generic;
using Enemy.AnimationHandler;
using Enemy.Controller;
using UnityEngine;
using UnityEngine.AI;
using untitledProject;
using Random = UnityEngine.Random;

namespace Enemy.SearchArea
{
    public class SearchAreaOverview : MonoBehaviour
    {
        [Range(1,5)]
        [Tooltip("the amount of enemies that can search at once after the player")]
        [SerializeField] private int _enemySearchMaxAmount;
        [Range(1,10)]
        [Tooltip("the max amount of waypoint the enemy will pick around the player position after chasing")]
        [SerializeField] private int _playerSearchWaypointCounter;
        
        // the closest waypoint to the current position of the throw position or player
        private float _closestWaypointDistance;
        // the transform of the closest current waypoint for the enemy
        private Transform _closestWaypoint;
        // the current waypoint distance when searching the closest waypoint of the player or throw position
        private float _currentWaypointDistance;
        // the max amount of waypoint the enemy will pick around the player position
        private int _searchWaypointAmount;
        // the possible waypoint that the enemy can pick when he searches the player
        List<Transform> _searchWaypoints = new List<Transform>();
        // the closest waypoints of the player position
        List<Transform> _searchSelectedPoints = new List<Transform>();
        // the amount of the waypoints that the enemy has when the player search for the player
        private int _usuableSearchPointAmount = 1;
        // signalize when the enemy is finish with searching to go back to his routine
        private bool _finishChecking = false;
        // pretend that the method will be activated multiple times of other enemies when they start to search after the player
        private bool _preparedSearchPoints = false;
        // the current closest waypoint for the enemy when the enemy searches the player
        private Transform _currentSearchWaypoint;

        // need this script to get the player position
        private PlayerController _playerController;

        public bool FinishChecking
        {
            get => _finishChecking;
            set => _finishChecking = value;
        }

        public bool PreparedSearchPoints
        {
            get => _preparedSearchPoints;
            set => _preparedSearchPoints = value;
        }

        public int EnemySearchMaxAmount
        {
            get => _enemySearchMaxAmount;
            set => _enemySearchMaxAmount = value;
        }

        // the amount of enemies that are searching currently
        private int _enemySearchAmount = 0;

        public int EnemySearchAmount
        {
            get => _enemySearchAmount;
            set => _enemySearchAmount = value;
        }
        
        private void Start()
        {
            _playerController = FindObjectOfType<PlayerController>();
            _searchWaypointAmount = _playerSearchWaypointCounter;
            
            GetSearchPoints();
        }

        /// <summary>
        /// get all current search points in the area
        /// </summary>
        public void GetSearchPoints()
        {
            _searchWaypoints.Clear();
            _searchSelectedPoints.Clear();
            
            foreach (Transform waypoints in transform)
            {
                _searchWaypoints.Add(waypoints);
            }
        }
        
        /// <summary>
        /// set the closest waypoint based on the player position
        /// </summary>
        public void StartSearchBehaviour(NavMeshAgent _agent, EnemyAnimationHandler _animationHandler, float _searchSpeed)
        {
            // when all search points has been used, the enemy goes back to patrolling or guarding
            if (_usuableSearchPointAmount <= 0)
            {
                GetSearchPoints();
                _finishChecking = true;
                _preparedSearchPoints = false;
                return;
            }
            
            _currentSearchWaypoint = _searchSelectedPoints[Random.Range(0, _searchSelectedPoints.Count)];
            if (_currentSearchWaypoint == null)
            {
                StartSearchBehaviour(_agent, _animationHandler, _searchSpeed);
                return;
            }
            _animationHandler.SetSpeed(_searchSpeed);
            _agent.SetDestination(_currentSearchWaypoint.position);
            _usuableSearchPointAmount--;
                
            _searchSelectedPoints.Remove(_currentSearchWaypoint);
        }
        
        /// <summary>
        /// search out the closest search points to the player
        /// </summary>
        public void PrepareSearchBehaviour()
        {
            _preparedSearchPoints = true;
            // get the current closest point based on the player position
            _closestWaypointDistance = Mathf.Infinity;
        
            foreach (Transform waypoint in _searchWaypoints)
            {
                _currentWaypointDistance = Vector3.Distance(waypoint.transform.position, _playerController.transform.position);
                if (_currentWaypointDistance <= _closestWaypointDistance)
                {
                    _closestWaypointDistance = _currentWaypointDistance;
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
    }
}
