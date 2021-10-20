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
        // the ref Velocity of the forward movement
        private float _refVelocity;
        private float _currentForwardVelocity;
        private Vector3 _moveDirection;
        // Movement Inputs
        private float _verticalAxis;
        private float _horizontalAxis;
        private float _currentVerticalVelocity;
        
        private CharacterController _myCharacterController;
        private PlayerAnimationHandler _playerAnimationHandler;
        
        [Header("Jump Settings")]
        [Tooltip("Maximum falling velocity the player can reach")] [Range(1f, 15f)] 
        [SerializeField] private float _terminalVelocity = 10f;
        [SerializeField] private float _gravityModifier;
        [Tooltip("The height in meters the player can jump")] 
        [SerializeField] private float _jumpHeight = 1;
        // Use formula: Mathf.Sqrt(h * (-2) * g)
        private float JumpVelocity => Mathf.Sqrt(_jumpHeight * -2 * Physics.gravity.y);
        
        [Header("Ground Check")]
        [Tooltip("the register radius around the position of the check position")] 
        [SerializeField] private float _groundCheckRadius = 0.001f;
        [Tooltip("the layer mask where the ground will be registered")] 
        [SerializeField] private LayerMask _groundLayerMask;
        [SerializeField] private Transform _groundCheckTransform;
        private RaycastHit hit;
        private bool _isGrounded;
        
        // the current state of the player
        private IPlayerState _currentState;
        private static readonly PlayerIdleState PlayerIdleState = new PlayerIdleState();
        public static readonly PlayerRunState PlayerRunState = new PlayerRunState();
        
        private void Awake()
        {
            // start state machine with LookAroundState
            _currentState = PlayerIdleState;
        }
        
        void Start()
        {
            _playerAnimationHandler = GetComponent<PlayerAnimationHandler>();
            _myCharacterController = GetComponent<CharacterController>();
        }
        
        private void Update()
        {
            MovementDirection();
            Move();
            Rotation();
            Jump();
            
            var playerState = _currentState.Execute(this);
            if (playerState != _currentState)
            {
                _currentState.Exit(this);
                _currentState = playerState;
                _currentState.Enter(this);
            }
        }
        
        /// <summary>
        /// move the character in the desired direction on the X and Z axis
        /// </summary>
        private void Move()
        {
            _playerAnimationHandler.SetSpeed(_currentForwardVelocity);
            // Clamp velocity to reach no more than our defined terminal velocity
            _currentVerticalVelocity = Mathf.Clamp(_currentVerticalVelocity, -_terminalVelocity, JumpVelocity);
            
            // the current velocity will be smoothed, so that it is possible to have some tweaks 
            _currentForwardVelocity = Mathf.SmoothDamp(_currentForwardVelocity, _moveDirection.magnitude * _movementSpeed , ref _refVelocity, _smoothSpeed * Time.deltaTime);
            Vector3 velocity = new Vector3(_moveDirection.x * _currentForwardVelocity, _currentVerticalVelocity, _moveDirection.z * _currentForwardVelocity) + new Vector3(0, Gravity(), 0);
            _myCharacterController.Move(velocity * Time.deltaTime);

            _playerAnimationHandler.SetSpeed(_currentForwardVelocity);
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
            _isGrounded = Physics.CheckSphere(_groundCheckTransform.position, _groundCheckRadius, _groundLayerMask);
            if (!_isGrounded)
            {
                _currentVerticalVelocity += Physics.gravity.y * _gravityModifier * Time.deltaTime;
                return _currentVerticalVelocity;
            }
            return _currentVerticalVelocity;
        }

        public void Jump()
        {
            // Check if we are grounded, if so reset gravity
            _isGrounded = Physics.CheckSphere(_groundCheckTransform.position, _groundCheckRadius, _groundLayerMask);
            if (_isGrounded)
            {
                // Reset current vertical velocity
                _currentVerticalVelocity = 0f;
            }
            if (_isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                //_playerAnimationHandler.DoJump();
                _currentVerticalVelocity = JumpVelocity;
            }
        }
    }
}
