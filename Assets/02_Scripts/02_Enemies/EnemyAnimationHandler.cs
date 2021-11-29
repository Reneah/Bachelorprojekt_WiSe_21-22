using Enemy.Controller;
using UnityEngine;
using untitledProject;

namespace Enemy.AnimationHandler
{
    public class EnemyAnimationHandler : MonoBehaviour
    {
        private Animator _enemyAnimator;
        private EnemyController _enemyController;
        private PlayerController _playerController;

        private float _headRotationWeight = 1;
        private bool _finishedInvestigationAnimation = false;
        private bool _finishedLookingAnimation = false;

        public float HeadRotationWeight
        {
            get => _headRotationWeight;
            set => _headRotationWeight = value;
        }

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
            _playerController = FindObjectOfType<PlayerController>();
        }
        
        /// <summary>
        /// sets the speed of the run animation and decides to play the run or idle animation
        /// </summary>
        public void SetSpeed(float movementSpeed)
        {
            _enemyAnimator.SetFloat(MovementSpeed, movementSpeed);
        }

        public void InvestigatePoint()
        {
            _enemyAnimator.SetTrigger(Investigation);
        }

        public void ResetInvestigatePoint()
        {
            _enemyAnimator.ResetTrigger(Investigation);
        }

        public void LookingAround()
        {
            _enemyAnimator.SetTrigger(Looking);
        }
        
        public void ResetLookingAround()
        {
            _enemyAnimator.ResetTrigger(Looking);
        }
        
        public void FinalHit()
        {
            _enemyAnimator.SetTrigger(Hit);
        }

        // animation event
        public void FinishedInvestigationAnimationClip()
        {
            _finishedInvestigationAnimation = true;
            ResetInvestigatePoint();
        }

        // animation event
        public void FinishedLookingAnimationClip()
        {
            _finishedLookingAnimation = true;
            ResetLookingAround();
        }

        public void TalkToEnemy(bool talk)
        {
            _enemyAnimator.SetBool(Talk, talk);
        }

        public void LootSpot(bool loot)
        {
            _enemyAnimator.SetBool(Loot, loot);
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (_enemyController.CanSeePlayer)
            {
                _enemyAnimator.SetLookAtWeight(_headRotationWeight);
                _enemyAnimator.SetLookAtPosition(_enemyController.LookPositionAtSpotted.position);
            }

            if (_enemyController.GuardBehaviour)
            {
                _enemyAnimator.SetLookAtWeight(_headRotationWeight);
                _enemyAnimator.SetLookAtPosition(_enemyController.CurrentLookPosition.transform.position);
            }

        }
    }

}
