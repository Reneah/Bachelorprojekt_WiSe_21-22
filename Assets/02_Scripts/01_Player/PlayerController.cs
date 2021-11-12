using System;
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
        // the ref Velocity of the forward movement
        private float _refVelocity;
        private float _currentForwardVelocity;
        private Vector3 _moveDirection;
        private float targetSpeed;

        public Vector3 MoveDirection
        {
            get => _moveDirection;
            set => _moveDirection = value;
        }

        // Movement Inputs
        private float _verticalAxis;
        private float _horizontalAxis;
        private float _currentVerticalVelocity;

        public float CurrentVerticalVelocity
        {
            get => _currentVerticalVelocity;
            set => _currentVerticalVelocity = value;
        }

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
        }
        
        private void Update()
        {
            var playerState = _currentState.Execute(this);
            if (playerState != _currentState)
            {
                _currentState.Exit(this);
                _currentState = playerState;
                _currentState.Enter(this);
            }
            
            GroundCheck();
            _playerAnimationHandler.SetGrounded(IsGrounded);
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
                targetSpeed = (sprint? _sprintSpeed : _movementSpeed) * _moveDirection.magnitude;
            }
            
            // the current velocity will be smoothed, so that it is possible to have some tweaks 
            _currentForwardVelocity = Mathf.SmoothDamp(_currentForwardVelocity, targetSpeed, ref _refVelocity, _smoothSpeed * Time.deltaTime);
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

            if (_currentVerticalVelocity <= 1)
            {
                _useGroundCheck = true;
            }
        }
        
        public void Jump()
        {
            if (_isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                _playerAnimationHandler.DoJump();
                _currentVerticalVelocity = JumpVelocity;
            }
        }
        
        public void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_groundCheckTransform.position, _groundCheckRadius);
        }
    }
}
