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
        [SerializeField]
        private float movementSpeed = 3;
        [Tooltip("smooth the character rotation")]
        [SerializeField]
        private float smoothRotation = 10;
        [Tooltip("smooth the movement speed of the character")]
        [SerializeField]
        private float smoothSpeed = 10;
        // the ref Velocity of the forward movement
        private float refVelocity;
        private float currentForwardVelocity;
        private Vector3 moveDirection;
        // Movement Inputs
        private float verticalAxis;
        private float horizontalAxis;
        
        [Tooltip("Maximum falling velocity the player can reach")] [Range(1f, 15f)] [SerializeField]
        private float terminalVelocity = 10f;

        private CharacterController myCharacterController;

        private RaycastHit hit;
        private bool isGrounded;
        [Tooltip("The height in meters the player can jump")] [SerializeField]
        private float jumpHeight = 1;
        
        [Tooltip("the register radius around the position of the check position")] 
        [SerializeField] private float groundCheckRadius = 0.001f;
        [Tooltip("the layer mask where the ground will be registered")] 
        [SerializeField] private LayerMask groundLayerMask;

        [SerializeField] private float gravityModifier;
        private PlayerAnimationHandler playerAnimationHandler;
        private bool playerIsAbleToMove = true;
        [SerializeField] private float slowDownTime = 15;
        [SerializeField] private Animator myAnimator;
        
        public bool PlayerIsAbleToMove
        {
            get => playerIsAbleToMove;
            set => playerIsAbleToMove = value;
        }

        public float RefVelocity => refVelocity;

        
        // the current state of the player
        private IPlayerState currentState;

        private static readonly PlayerIdleState PlayerIdleState = new PlayerIdleState();
        public static readonly PlayerRunState PlayerRunState = new PlayerRunState();
        private float currentVerticalVelocity;

        [SerializeField] private Transform groundCheckTransform;
         private bool jumpButton;

        // Use formula: Mathf.Sqrt(h * (-2) * g)
        private float JumpVelocity => Mathf.Sqrt(jumpHeight * -2 * Physics.gravity.y);


        private void Awake()
        {
            // start state machine with LookAroundState
            currentState = PlayerIdleState;
        }
        
        // Start is called before the first frame update
        void Start()
        {
            playerAnimationHandler = GetComponent<PlayerAnimationHandler>();
            myCharacterController = GetComponent<CharacterController>();
        }
    
        // Update is called once per frame
        void FixedUpdate()
        {
            
        }

        // Update is called once per frame
        private void Update()
        {
            jumpButton = Input.GetButtonDown("Jump");
            Debug.Log(Input.GetKeyDown(KeyCode.Space) + "space");
            Debug.Log(jumpButton);
            Debug.Log(currentState);
            // get the input of the direction
            verticalAxis = Input.GetAxisRaw("Vertical");
            horizontalAxis = Input.GetAxisRaw("Horizontal");
            
            moveDirection = (Camera.main.transform.right * horizontalAxis + Camera.main.transform.forward * verticalAxis).normalized;
            
            if (playerIsAbleToMove)
            {
                Move();
                Rotation();
                Jump();
            }
            
            var playerState = currentState.Execute(this);
            if (playerState != currentState)
            {
                currentState.Exit(this);
                currentState = playerState;
                currentState.Enter(this);
            }
        }
        
        /// <summary>
        /// move the character in the desired direction on the X and Z axis
        /// </summary>
        private void Move()
        {
            playerAnimationHandler.SetSpeed(currentForwardVelocity);
            // Clamp velocity to reach no more than our defined terminal velocity
            currentVerticalVelocity = Mathf.Clamp(currentVerticalVelocity, -terminalVelocity, JumpVelocity);
            
            // the current velocity will be smoothed, so that it is possible to have some tweaks 
            currentForwardVelocity = Mathf.SmoothDamp(currentForwardVelocity, moveDirection.magnitude * movementSpeed , ref refVelocity, smoothSpeed * Time.deltaTime);
            
            Vector3 velocity = new Vector3(moveDirection.x * currentForwardVelocity, currentVerticalVelocity, moveDirection.z * currentForwardVelocity) + new Vector3(0, Gravity(), 0);
            
            myCharacterController.Move(velocity * Time.deltaTime);

            playerAnimationHandler.SetSpeed(currentForwardVelocity);
        }
        
        /// <summary>
        /// rotates the character in the desired direction on the X and Z axis
        /// </summary>
        private void Rotation()
        {
            if (moveDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(moveDirection.x, 0, moveDirection.z)), smoothRotation * Time.deltaTime);
            }
        }
        
        /// <summary>
        /// returns the gravity of the character
        /// </summary>
        private float Gravity()
        {
            isGrounded = Physics.CheckSphere(groundCheckTransform.position, groundCheckRadius, groundLayerMask);
            if (!isGrounded)
            {
                currentVerticalVelocity += Physics.gravity.y * gravityModifier * Time.deltaTime;
                return currentVerticalVelocity;
            }
            return currentVerticalVelocity;
        }

        public void Jump()
        {
            // Check if we are grounded, if so reset gravity
            isGrounded = Physics.CheckSphere(groundCheckTransform.position, groundCheckRadius, groundLayerMask);
            if (isGrounded)
            {
                // Reset current vertical velocity
                currentVerticalVelocity = 0f;
            }
            if (isGrounded && jumpButton)
            {
                Debug.Log("JUMP");
                //_playerAnimationHandler.DoJump();
                currentVerticalVelocity = JumpVelocity;
            }
        }
        
        public void StopPlayerMovement()
        {
            playerIsAbleToMove = false;
        }
        
        public void ContinuePlayerMovement()
        {
            playerIsAbleToMove = true;
      
        }
    }
}
