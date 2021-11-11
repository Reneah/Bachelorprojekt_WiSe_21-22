using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using untitledProject;

public class PlayerAnimationHandler : MonoBehaviour
{
    private static readonly int MovementSpeed = Animator.StringToHash("MovementSpeed");

    private Animator _playerAnimator;
    private PlayerController _playerController;
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
    private static readonly int VerticalVelocity = Animator.StringToHash("VerticalVelocity");
    private static readonly int Death = Animator.StringToHash("Death");
    private static readonly int Throw = Animator.StringToHash("Throw");

    // checks if the throw animation is at the end
    private bool _runningThrowAnimation = false;

    public bool RunningThrowAnimation
    {
        get => _runningThrowAnimation;
        set => _runningThrowAnimation = value;
    }

    void Start()
    {
        _playerAnimator = GetComponent<Animator>();
        _playerController = FindObjectOfType<PlayerController>();
    }
    
    /// <summary>
    /// Set the players velocities.
    /// </summary>
    /// <param name="movementSpeed">Players movement speed</param>
    /// <param name="verticalVelocity">Players vertical velocity</param>
    public void SetSpeeds(float movementSpeed, float verticalVelocity)
    {
        _playerAnimator.SetFloat(MovementSpeed, movementSpeed);
        _playerAnimator.SetFloat(VerticalVelocity, verticalVelocity);
    }

    /// <summary>
    /// Triggers the animator jump trigger
    /// </summary>
    public void DoJump()
    {
        _playerAnimator.SetTrigger(Jump);
    }

    public void ResetJumpTrigger()
    {
        _playerAnimator.ResetTrigger(Jump);
    }
    
    /// <summary>
    /// Tells the player whether the player is grounded or not
    /// </summary>
    /// <param name="grounded"></param>
    public void SetGrounded(bool grounded)
    {
        _playerAnimator.SetBool(IsGrounded, grounded);
    }

    public void PlayerDeath()
    {
        _playerAnimator.SetTrigger(Death);
    }

    public void PlayerThrow()
    {
        _playerAnimator.SetTrigger(Throw);
    }

    /// <summary>
    /// Animation Event
    /// </summary>
    public void EndThrowAnimation()
    {
        _runningThrowAnimation = true;
    }
    
    
}
