using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The Source of the code: https://www.youtube.com/watch?v=j1-OyLo77ss&t=973s
public class FieldOfView : MonoBehaviour
{
    [Tooltip("the radius of the view field")]
    [SerializeField] private float _radius;
    
    public float Radius
    {
        get => _radius;
        set => _radius = value;
    }

    [Tooltip("the angle of the view field")]
    [Range(0,360)]
    [SerializeField] private float _angle;
    
    public float Angle
    {
        get => _angle;
        set => _angle = value;
    }
    
    [Tooltip("the reference of the player")]
    [SerializeField] private GameObject _playerRef;
    
    public GameObject PlayerRef
    {
        get => _playerRef;
        set => _playerRef = value;
    }

    [Tooltip("the mask for the registration of the player in the view field")]
    [SerializeField] private LayerMask _targetMask;
    [Tooltip("the mask for the registration of the obstacle in the view field")]
    [SerializeField] private LayerMask obstructionMask;

    [Tooltip("the enemy is able to see the player")]
    [SerializeField] private bool _canSeePlayer;
    
    public bool CanSeePlayer
    {
        get => _canSeePlayer;
        set => _canSeePlayer = value;
    }
    
    [Tooltip("determine the wait time in seconds for every view field check")]
    [SerializeField] float delay = 0.2f;

    private void Start()
    {
        StartCoroutine(FOVRoutine());
    }

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
    
}
