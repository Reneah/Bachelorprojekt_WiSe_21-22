using UnityEngine;
using untitledProject;

public class WallCheck : MonoBehaviour
    {
        [Tooltip("the force how hard the player should slide down the wall when it is too steep")]
        [SerializeField] private float _slopeStrength = 3;
        
        // need the script to know when the player is grounded to prevent conflicts with the jump
        // and to use the character controller to move the character the wall down
        private PlayerController _playerController;


        // to know in which angle the character is moving relative to the ground to activate the wall check
        private float _angle;

        private bool _deactivateCancel = false;

        void Start()
        {
            _playerController = GetComponent<PlayerController>();
        }
    
        void Update()
        {
            SlideWallDown(70, transform.TransformDirection(new Vector3(0,0,0)), _slopeStrength);
        }

        private void SlideWallDown(float angleLimit, Vector3 raycastOffset, float slopeStrength)
        {
            Debug.DrawRay(transform.position + raycastOffset, Vector3.down, Color.magenta);
            
            RaycastHit hit;
            if (Physics.Raycast(transform.position + raycastOffset, Vector3.down, out hit, 8))
            {
                Debug.Log(_angle);
                _angle = Vector3.Angle(hit.normal, Vector3.up);

                // on which area the wall check works is limited so that the player can move properly at the specific situations
                if (_angle > angleLimit && _playerController.IsGrounded && hit.collider != null && hit.collider.CompareTag("Wall"))
                {
                    // the vector shows the surface direction along the x axis of the player
                    Vector3 firstOrthogonalVector = Vector3.Cross(hit.normal, Vector3.down);
                    // the vector shows the surface direction along the z axis of the player, thus can we move the player down the wall to the ground
                    Vector3 slopeDirection = Vector3.Cross(firstOrthogonalVector, hit.normal);
                    _playerController.CharacterController.Move(slopeDirection * (Time.deltaTime * slopeStrength));
                    _deactivateCancel = false;
                }
            }
        }
    }

