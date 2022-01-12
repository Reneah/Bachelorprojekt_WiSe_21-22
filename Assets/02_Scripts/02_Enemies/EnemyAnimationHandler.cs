using DarkTonic.MasterAudio;
using Enemy.Controller;
using UnityEngine;

namespace Enemy.AnimationHandler
{
    public class EnemyAnimationHandler : MonoBehaviour
    {
        private Animator _enemyAnimator;
        private EnemyController _enemyController;
        
        // how much the head should rotate
        private float _headRotationWeight = 1;
        // determines when the investigation animation is finished
        private bool _finishedInvestigationAnimation = false;
        // determines when the look animation is finished
        private bool _finishedLookingAnimation = false;
        
        public bool FinishedLookingAnimation
        {
            get => _finishedLookingAnimation;
            set => _finishedLookingAnimation = value;
        }

        public bool FinishedInvestigationAnimation
        {
            get => _finishedInvestigationAnimation;
            set => _finishedInvestigationAnimation = value;
        }

        private static readonly int Investigation = Animator.StringToHash("Investigation");
        private static readonly int MovementSpeed = Animator.StringToHash("MovementSpeed");
        private static readonly int Hit = Animator.StringToHash("FinalHit");
        private static readonly int Looking = Animator.StringToHash("Looking");
        private static readonly int Talk = Animator.StringToHash("Talk");
        private static readonly int Loot = Animator.StringToHash("Loot");

        void Start()
        {
            _enemyAnimator = GetComponent<Animator>();
            _enemyController = GetComponent<EnemyController>();
        }
        
        /// <summary>
        /// sets the speed of the run animation and decides to play the run or idle animation
        /// </summary>
        public void SetSpeed(float movementSpeed)
        {
            _enemyAnimator.SetFloat(MovementSpeed, movementSpeed);
        }

        /// <summary>
        /// When the enemy reached the noisy item, the enemy will play the investigation animation
        /// </summary>
        public void InvestigatePoint()
        {
            _enemyAnimator.SetTrigger(Investigation);
        }

        /// <summary>
        /// reset the investigation animation
        /// </summary>
        public void ResetInvestigatePoint()
        {
            _enemyAnimator.ResetTrigger(Investigation);
        }

        /// <summary>
        /// When the sound stage 3 at the noisy item is reached, the look around animation will be played as well
        /// </summary>
        public void LookingAround()
        {
            _enemyAnimator.SetTrigger(Looking);
        }
        
        /// <summary>
        /// reset the look around animation
        /// </summary>
        public void ResetLookingAround()
        {
            _enemyAnimator.ResetTrigger(Looking);
        }
        
        /// <summary>
        /// When the enemy reached the player, the game is over and will execute the final hit
        /// </summary>
        public void FinalHit()
        {
            _enemyAnimator.SetTrigger(Hit);
        }

        /// <summary>
        /// Animation Event, determines when the investigation animation is finished to execute the next method
        /// </summary>
        public void FinishedInvestigationAnimationClip()
        {
            _finishedInvestigationAnimation = true;
            ResetInvestigatePoint();
        }

        /// <summary>
        /// Animation Event, determines when the look animation is finished to execute the next method
        /// </summary>
        public void FinishedLookingAnimationClip()
        {
            _finishedLookingAnimation = true;
            ResetLookingAround();
        }

        /// <summary>
        /// determines when to play the talk animation
        /// </summary>
        /// <param name="talk"></param>
        public void TalkToEnemy(bool talk)
        {
            _enemyAnimator.SetBool(Talk, talk);
        }

        /// <summary>
        /// determines when to play the loot animation
        /// </summary>
        /// <param name="loot"></param>
        public void LootSpot(bool loot)
        {
            _enemyAnimator.SetBool(Loot, loot);
        }

        /// <summary>
        /// controls the head rotation of the enemy
        /// </summary>
        /// <param name="layerIndex"></param>
        private void OnAnimatorIK(int layerIndex)
        {
            // controls the head rotation when the enemy is in chase state
            if (_enemyController.InChaseState)
            {
                _enemyAnimator.SetLookAtWeight(0.5f);
                _enemyAnimator.SetLookAtPosition(_enemyController.LookPositionAtSpotted.position);
            }

            // controls the head rotation when the enemies are in the guard behaviour and follow their look route
            if (_enemyController.GuardBehaviour)
            {
                _enemyAnimator.SetLookAtWeight(_headRotationWeight);
                _enemyAnimator.SetLookAtPosition(_enemyController.CurrentLookPosition.transform.position);
            }
        }
        
        /// <summary>
        /// plays the enemy footstep sound
        /// </summary>
        /// <param name="animationEvent"></param>
        public void EnemyFootsteps(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                MasterAudio.PlaySound3DAtTransform("EnemyFootsteps", transform);
            }
        }
    }
}
