using BP;
using DarkTonic.MasterAudio;
using UnityEngine;

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
    private static readonly int Flee = Animator.StringToHash("Flee");

    // checks if the throw animation is at the end
    private bool _runningThrowAnimation = false;

    private bool _runningPickUpAnimation = false;

    private StepVisualizationManager myStepVisualizationManager;
    private static readonly int PickUp = Animator.StringToHash("PickUp");


    public Animator PlayerAnimator
    {
        get => _playerAnimator;
        set => _playerAnimator = value;
    }

    public bool RunningThrowAnimation
    {
        get => _runningThrowAnimation;
        set => _runningThrowAnimation = value;
    }

    public bool RunningPickUpAnimation
    {
        get => _runningPickUpAnimation;
        set => _runningPickUpAnimation = value;
    }

    void Start()
    {
        _playerAnimator = GetComponent<Animator>();
        _playerController = FindObjectOfType<PlayerController>();
        myStepVisualizationManager = FindObjectOfType<StepVisualizationManager>();
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

    public void PlayerFlee(bool flee)
    {
        _playerAnimator.SetBool(Flee, flee);
    }

    public void PlayerPickUp()
    {
        _playerAnimator.SetTrigger(PickUp);
    }

    public void EndPickUpAnimation()
    {
        _runningPickUpAnimation = false;
    }
    public void ResetPickUpAnimation()
    {
        _playerAnimator.ResetTrigger(PickUp);
    }
    
    /// <summary>
    /// Animation Event
    /// </summary>
    public void EndThrowAnimation()
    {
        _runningThrowAnimation = false;
    }

    public void PlayerQuietFootsteps(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            MasterAudio.PlaySound3DAtTransform("PlayerQuietFootsteps", transform);
        }
    }
    
    public void PlayerMediumFootsteps(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            MasterAudio.PlaySound3DAtTransform("PlayerMediumFootsteps", transform);
        }
    }
    
    public void PlayerHeavyFootsteps(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            MasterAudio.PlaySound3DAtTransform("PlayerLoudFootsteps", transform);
        }
    }

    public void PlayStepVisualizationOne()
    {
        myStepVisualizationManager.PlayStepVisualizationOne();
    }
    
    public void PlayStepVisualizationTwo()
    {
        myStepVisualizationManager.PlayStepVisualizationTwo();
    }
}
