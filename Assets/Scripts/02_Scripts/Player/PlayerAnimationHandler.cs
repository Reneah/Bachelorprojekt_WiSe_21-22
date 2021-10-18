using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using untitledProject;

public class PlayerAnimationHandler : MonoBehaviour
{
    private static readonly int MovementSpeed = Animator.StringToHash("MovementSpeed");

    private Animator playerAnimator;
    private PlayerController playerController;

    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        playerController = FindObjectOfType<PlayerController>();
    }
    
    /// <summary>
    /// sets the speed of the run animation and decides to play the run or idle animation
    /// </summary>
    public void SetSpeed(float movementSpeed)
    {
        playerAnimator.SetFloat(MovementSpeed, movementSpeed);
    }

    public void StopPlayerMovement()
    {
        playerController.StopPlayerMovement();
    }
        
    public void ContinuePlayerMovement()
    {
        playerController.ContinuePlayerMovement();
    }
}
