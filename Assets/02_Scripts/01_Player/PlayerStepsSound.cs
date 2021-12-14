using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using untitledProject;

public class PlayerStepsSound : MonoBehaviour
{
    [Tooltip("the sound radius when sneaking")]
    [SerializeField] private float _sneakSoundRadius;
    [Tooltip("the sound radius when running")]
    [SerializeField] private float _runSoundRadius;
    [Tooltip("the sound radius when running")]
    [SerializeField] private float _fleeingSoundRadius;
    [Tooltip("smooth the radius switch between running and sneaking")]
    [SerializeField] private float _smoothSwitch = 3;
    
    private PlayerController _playerController;
    private SphereCollider _collider;
    
    private float _refRadius;
    private float _targetSpeed;
    
    void Start()
    {
        _playerController = GetComponentInParent<PlayerController>();
        _collider = GetComponent<SphereCollider>();
    }
    
    void Update()
    {
        bool sprint = Input.GetKey(KeyCode.LeftShift);
        _targetSpeed = (sprint ? _runSoundRadius : _sneakSoundRadius);
        
        if (_playerController.PlayerAnimationHandler.PlayerAnimator.GetBool("Flee"))
        {
            _targetSpeed = _fleeingSoundRadius;
                    
        }
        
        _collider.radius = Mathf.SmoothDamp(_collider.radius, _targetSpeed, ref _refRadius, Time.deltaTime * _smoothSwitch);

        if (_playerController.CurrentForwardVelocity <= 0.5f)
        {
            _collider.enabled = false;
        }

        if (_playerController.CurrentForwardVelocity >= 0.5f)
        {
            _collider.enabled = true;
        }
    }
}
