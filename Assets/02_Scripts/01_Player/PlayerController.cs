using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using untitledProject;
using Physics = UnityEngine.Physics;
using Random = UnityEngine.Random;

namespace untitledProject
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [Tooltip("set the speed of the character")]
        [SerializeField] private float _movementSpeed = 3;
        [Tooltip("smooth the movement speed of the character")]
        [SerializeField] private float _smoothSpeed = 10;
        [Tooltip("smooth the character rotation")]
        [SerializeField] private float _smoothRotation = 10;
        [Tooltip("set the sprint speed of the character ")]
        [SerializeField] private float _sprintSpeed = 5;
        [Tooltip("the speed that the player has while fleeing")]
        [SerializeField] private float _fleeSpeed = 6;
        [Tooltip("when the enemy has lost the sight to the player, the time is set to still play the flee animation")]
        [SerializeField] private float _calmDownTime = 3;
        // the ref Velocity of the forward movement
        private float _refVelocity;
        private float _currentForwardVelocity;
        private Vector3 _moveDirection;
        private float _targetSpeed;

        private float _calmDownCooldown;
        private bool _playerIsSpotted = true;

        public float CurrentForwardVelocity
        {
            get => _currentForwardVelocity;
            set => _currentForwardVelocity = value;
        }

        public Vector3 MoveDirection
        {
            get => _moveDirection;
            set => _moveDirection = value;
        }

        // Movement Inputs
        private float _verticalAxis;
        private float _horizontalAxis;
        private float _currentVerticalVelocity;
        
        private CharacterController _characterController;

        public CharacterController CharacterController
        {
            get => _characterController;
            set => _characterController = value;
        }

        private PlayerAnimationHandler _playerAnimationHandler;
        
        public PlayerAnimationHandler PlayerAnimationHandler
        {
            get => _playerAnimationHandler;
            set => _playerAnimationHandler = value;
        }

        private CollectStones _collectStones;

        public CollectStones CollectStones
        {
            get => _collectStones;
            set => _collectStones = value;
        }

        [Header("Jump Settings")]
        [Tooltip("Maximum falling velocity the player can reach")] [Range(1f, 15f)] 
        [SerializeField] private float _terminalVelocity = 10f;
        [SerializeField] private float _gravityModifier;
        [Tooltip("The height in meters the player can jump")] 
        [SerializeField] private float _jumpHeight = 1;
        // Use formula: Mathf.Sqrt(h * (-2) * g)
        private float JumpVelocity => Mathf.Sqrt(_jumpHeight * -2 * Physics.gravity.y);
        private bool _isJumping = false;
        private bool _resetVerticalVelocity = false;
        public bool IsJumping
        {
            get => _isJumping;
            set => _isJumping = value;
        }
        
        [Header("Ground Check")]
        [Tooltip("the register radius around the position of the check position")] 
        [SerializeField] private float _groundCheckRadius = 0.001f;
        [Tooltip("the layer mask where the ground will be registered")] 
        [SerializeField] private LayerMask _groundLayerMask;
        [SerializeField] private Transform _groundCheckTransform;
        private RaycastHit hit;
        private bool _isGrounded;
        private bool _useGroundCheck;
        private bool _lowGround;
        private bool _highGround;
        
        public bool LowGround
        {
            get => _lowGround;
            set => _lowGround = value;
        }
        
        public bool HighGround
        {
            get => _highGround;
            set => _highGround = value;
        }

        [Header("Slope Settings")]
        [Tooltip("the force to the ground at the character")]
        [SerializeField]
        private float _slopeForce;
        [Tooltip("the length at which the slope force should take action")]
        [SerializeField]
        private float _slopeForceRayLength;

        public bool IsGrounded
        {
            get => _isGrounded;
            set => _isGrounded = value;
        }

        // get the script to have the bool value to use it in the states
        private PlayerThrowTrigger _playerThrowTrigger;

        public PlayerThrowTrigger PlayerThrowTrigger
        {
            get => _playerThrowTrigger;
            set => _playerThrowTrigger = value;
        }

        // the current state of the player
        private IPlayerState _currentState;
        public static readonly PlayerIdleState PlayerIdleState = new PlayerIdleState();
        public static readonly PlayerRunState PlayerRunState = new PlayerRunState();
        public static readonly PlayerJumpState PlayerJumpState =  new PlayerJumpState();
        public static readonly PlayerThrowState PlayerThrowState =  new PlayerThrowState();
        
        private void Awake()
        {
            // start state machine with LookAroundState
            _currentState = PlayerIdleState;

        }
        
        void Start()
        {
            _playerAnimationHandler = GetComponent<PlayerAnimationHandler>();
            _characterController = GetComponent<CharacterController>();
            _playerThrowTrigger = FindObjectOfType<PlayerThrowTrigger>();
            _collectStones = FindObjectOfType<CollectStones>();

            _characterController.material.staticFriction = 0;
            _characterController.material.dynamicFriction = 0;

            _calmDownCooldown = _calmDownTime;

            StartCoroutine(PlayerPosition());

            _characterController.enabled = false;
            transform.position = new Vector3(PlayerPrefs.GetFloat("PlayerPositionX", transform.position.x), PlayerPrefs.GetFloat("PlayerPositionY", transform.position.y), PlayerPrefs.GetFloat("PlayerPositionZ", transform.position.z));
            _characterController.enabled = true;
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                PlayerPrefs.DeleteAll();
            }
            
            var playerState = _currentState.Execute(this);
            if (playerState != _currentState)
            {
                _currentState.Exit(this);
                _currentState = playerState;
                _currentState.Enter(this);
            }
            
            GroundCheck();
            _playerAnimationHandler.SetGrounded(IsGrounded);

            CalmDownTime();
        }

        private IEnumerator PlayerPosition()
        {
            yield return new WaitForSeconds(0.1f);
            
        }
        
        /// <summary>
        /// when the enemy has lost the player, hew needs time to calm down to play the sneak animation again
        /// </summary>
        private void CalmDownTime()
        {
            if (!_playerIsSpotted)
            {
                if (_calmDownCooldown > 0)
                {
                    _calmDownCooldown -= Time.deltaTime;
                }

                if (_calmDownCooldown <= 0)
                {
                    _calmDownCooldown = _calmDownTime;
                    _playerAnimationHandler.PlayerFlee(false);
                    _playerIsSpotted = true;
                }
            }
        }

        public void MovementExecution()
        {
            MovementDirection();
            Rotation();
            Move();
        }
        
        /// <summary>
        /// move the character in the desired direction on the X and Z axis
        /// </summary>
        public void Move()
        {
            // Clamp velocity to reach no more than our defined terminal velocity
            _currentVerticalVelocity = Mathf.Clamp(_currentVerticalVelocity, -_terminalVelocity, JumpVelocity);

            // use this bool to prevent the player control about the the speed during the jump
            if (!_isJumping)
            {
                _resetVerticalVelocity = true;
                bool sprint = Input.GetKey(KeyCode.LeftShift);
                _targetSpeed = (sprint? _sprintSpeed : _movementSpeed) * _moveDirection.magnitude;

                if (_playerAnimationHandler.PlayerAnimator.GetBool("Flee"))
                {
                    _targetSpeed = _fleeSpeed * _moveDirection.magnitude;
                }
                
            }
            
            // the current velocity will be smoothed, so that it is possible to have some tweaks 
            _currentForwardVelocity = Mathf.SmoothDamp(_currentForwardVelocity, _targetSpeed, ref _refVelocity, _smoothSpeed * Time.deltaTime);
            Vector3 velocity = new Vector3(_moveDirection.x * _currentForwardVelocity, _currentVerticalVelocity, _moveDirection.z * _currentForwardVelocity) + new Vector3(0, Gravity(), 0);
            _characterController.Move(velocity * Time.deltaTime);
            
            _playerAnimationHandler.SetSpeeds(_currentForwardVelocity, _currentVerticalVelocity);
        }

        private void MovementDirection()
        {
            // get the input of the direction
            _verticalAxis = Input.GetAxisRaw("Vertical");
            _horizontalAxis = Input.GetAxisRaw("Horizontal");
            
            _moveDirection = (Camera.main.transform.right * _horizontalAxis + Camera.main.transform.forward * _verticalAxis).normalized;
        }
        
        /// <summary>
        /// rotates the character in the desired direction on the X and Z axis
        /// </summary>
        private void Rotation()
        {
            if (_moveDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(_moveDirection.x, 0, _moveDirection.z)), _smoothRotation * Time.deltaTime);
            }
        }
        
        /// <summary>
        /// returns the gravity of the character
        /// </summary>
        private float Gravity()
        {
            if (!_isGrounded)
            {
                _currentVerticalVelocity += Physics.gravity.y * _gravityModifier * Time.deltaTime;
                return _currentVerticalVelocity;
            }
            return _currentVerticalVelocity;
        }

        public void GroundCheck()
        {
            if (_useGroundCheck)
            {
                // Check if we are grounded
                _isGrounded = Physics.CheckSphere(_groundCheckTransform.position, _groundCheckRadius, _groundLayerMask);
            }
            
            if (_isGrounded && _resetVerticalVelocity)
            {
                // Reset current vertical velocity
                _currentVerticalVelocity = 0;
                _playerAnimationHandler.ResetJumpTrigger();
                _resetVerticalVelocity = false;
                _useGroundCheck = false;
            }
            
            // if the character is not moving horizontal to the ground, the slope will be activated to hold the character down
            if ((_verticalAxis != 0 || _horizontalAxis != 0) && OnSlope() && _isGrounded)
            {
                _characterController.Move(Vector3.down * _characterController.height / 2 * (_slopeForce * Time.deltaTime)); 
            }

            if (_currentVerticalVelocity <= 1)
            {
                _useGroundCheck = true;
            }
        }
        
        /// <summary>
        /// decides when to activate the slope to hold the player down at not horizontal surfaces
        /// </summary>
        /// <returns></returns>
        private bool OnSlope()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, _characterController.height / 2 * _slopeForceRayLength))
            {
                // if the character is not horizontal relative to Vector3.up then the character will be dragged to the ground to don't take off
                if (hit.normal != Vector3.up)
                    return true;
            }
            return false;
        }
        
        public void Jump()
        {
            if (_isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                _playerAnimationHandler.DoJump();
                _currentVerticalVelocity = JumpVelocity;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("ViewCone"))
            {
                _calmDownCooldown = _calmDownTime;
                _playerIsSpotted = true;
            }

            if (other.CompareTag("HighGround"))
            {
                _highGround = true;
                _lowGround = false;
            }

            if (other.CompareTag("LowGround"))
            {
                _highGround = false;
                _lowGround = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if(other.CompareTag("ViewCone"))
            {
                _playerIsSpotted = false;
            }
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_groundCheckTransform.position, _groundCheckRadius);
        }
    }
}
