using System.Collections.Generic;
using Enemy.AnimationHandler;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.SearchArea
{
    public class NoisyItemSearchArea : MonoBehaviour
    {
        [Range(1,5)]
        [Tooltip("the max amount of waypoints the enemy will pick around the throw position after checking the noisy item")]
        [SerializeField] private int _throwWaypointCounter;
        
        // the closest waypoint to the current position of the throw position
        private float _closestWaypointDistance;
        // the transform of the closest current waypoint for the enemy
        private Transform _closestWaypoint;
        // the current waypoint distance when searching the closest waypoint of the throw position
        private float _currentWaypointDistance;
        // the amount of the waypoints that the enemy has when the player activates the noisy item in long range
        private int _usuableWaypointsRangeAmount = 1;
        // the current closest waypoint for the enemy when the player activate the item in close range
        private Transform _currentCloseNoisyItemWaypoint;
        // get all waypoints in the search area again after using them
        private bool _resetNoisyItemWaypoints = false;
        // the max amount of waypoints the enemy will pick around the throw position after checking the noisy item
        private int _throwWaypointAmount;
        // the available waypoints that the enemy can pick when he investigated the noisy item
        List<Transform> _noisyItemSearchPoints = new List<Transform>();
        // the closest waypoints of the throw position
        List<Transform> _noisyItemSelectedPoints = new List<Transform>();
        // the amount of the waypoints that the enemy has when the player activates the noisy item in close range
        private int _usableWaypointsAmount = 1;
        // signalizes when the enemy is finished with searching to go back to his routine
        private bool _finishChecking = false;
        // pretend that the method will be activated multiple times of other enemies when they start to search after the player
        private bool _preparedSearchPoints = false;
        // need this script to get information of the player throw
        private PlayerController _playerController;

        public bool PreparedSearchPoints
        {
            get => _preparedSearchPoints;
            set => _preparedSearchPoints = value;
        }

        public int UsuableWaypointsAmount
        {
            get => _usableWaypointsAmount;
            set => _usableWaypointsAmount = value;
        }

        public bool FinishChecking
        {
            get => _finishChecking;
            set => _finishChecking = value;
        }
        
        void Start()
        {
            _playerController = FindObjectOfType<PlayerController>();
            
            // set the max amount of waypoints the enemy will pick around the throw position after checking the noisy item
            _throwWaypointAmount = _throwWaypointCounter;
            
            GetSearchPoints();
        }
        
        /// <summary>
        /// get all current search points in the area
        /// </summary>
        public void GetSearchPoints()
        {
            _noisyItemSearchPoints.Clear();
            _noisyItemSelectedPoints.Clear();
            
            foreach (Transform waypoints in transform)
            {
                _noisyItemSearchPoints.Add(waypoints);
            }
        }
        
          /// <summary>
        /// get the closest search points of the player throw position 
        /// </summary>
        public void PrepareSearchNoisyItemBehaviour()
        {
            _preparedSearchPoints = true;
            
            // when the enemy has thrown a stone, the closest waypoints of the throw position will be selected
            if (_playerController.PlayerThrowTrigger.PlayerThrew)
            {
                _closestWaypointDistance = Mathf.Infinity;
                foreach (Transform waypoint in _noisyItemSearchPoints)
                {
                    _currentWaypointDistance = Vector3.Distance(waypoint.transform.position, _playerController.PlayerThrowTrigger.ThrowPosition.transform.position);
                    if (_currentWaypointDistance <= _closestWaypointDistance)
                    {
                        _closestWaypoint = waypoint;
                        _closestWaypointDistance = _currentWaypointDistance;
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
                _usuableWaypointsRangeAmount = Random.Range(2, _noisyItemSelectedPoints.Count);
            }
        }
        
        /// <summary>
        /// select the waypoint where the enemy has to go
        /// </summary>
        public void StartSearchNoisyItemBehaviour(NavMeshAgent _agent, EnemyAnimationHandler _animationHandler, float _searchSpeed, SoundItem.NoisyItem _noisyItemScript)
        {
            // when all points were used, the enemy will go back to his routine
            if (_usableWaypointsAmount <= 0 || _usuableWaypointsRangeAmount <= 0)
            {
                _finishChecking = true;
                GetSearchPoints();
                _preparedSearchPoints = false;
                return;
            }

            // when the enemy activated the item with a throw, the waypoints near the throw position will be used
            if (_playerController.PlayerThrowTrigger.PlayerThrew)
            {
                _currentCloseNoisyItemWaypoint = _noisyItemSelectedPoints[Random.Range(0, _noisyItemSelectedPoints.Count)];
                
                if (_currentCloseNoisyItemWaypoint == null)
                {
                    StartSearchNoisyItemBehaviour(_agent, _animationHandler, _searchSpeed, _noisyItemScript);
                    return;
                }
                
                _agent.SetDestination(_currentCloseNoisyItemWaypoint.position);
                _usuableWaypointsRangeAmount--;
                
                _noisyItemSelectedPoints.Remove(_currentCloseNoisyItemWaypoint);
            }

            // when the enemy activated the item in close range, selected waypoints in the inspector will be used
            if (!_playerController.PlayerThrowTrigger.PlayerThrew)
            {
                _currentCloseNoisyItemWaypoint = _noisyItemScript.CloseNoisyItemWaypoints[Random.Range(0, _noisyItemScript.CloseNoisyItemWaypoints.Length)];
                
                if (_currentCloseNoisyItemWaypoint == null)
                {
                    StartSearchNoisyItemBehaviour(_agent, _animationHandler, _searchSpeed, _noisyItemScript);
                    return;
                }
                
                _agent.SetDestination(_currentCloseNoisyItemWaypoint.position);
                _usableWaypointsAmount--;
                
                _noisyItemSelectedPoints.Remove(_currentCloseNoisyItemWaypoint);
            }
            
            _animationHandler.SetSpeed(_searchSpeed);
        }
    }
}
