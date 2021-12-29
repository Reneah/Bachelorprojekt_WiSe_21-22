using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using untitledProject;

namespace BP
{
    public class StepVisualization : MonoBehaviour
    {
        private StepVisualizationManager myStepVisualizationManager;
        
        private ProjectorForLWRP.ProjectorForLWRP myProjector;
        
        private float currentSize;

        private Material myMaterial;
        
        private float minSize;

        private float maxSize;

        private PlayerController player;
        private bool visualize;

        private float scalingSpeed;
        private float fadeDuration;

        // Start is called before the first frame update
        void Start()
        {
            player = FindObjectOfType<PlayerController>();
            myStepVisualizationManager = FindObjectOfType<StepVisualizationManager>();
            myProjector = GetComponent<ProjectorForLWRP.ProjectorForLWRP>();
            scalingSpeed = 
            currentSize = myProjector.projector.orthographicSize;
            myMaterial = myProjector.projector.material;
            myMaterial.color = myStepVisualizationManager.Color;
            minSize = myStepVisualizationManager.MinSize;
        }

        // Update is called once per frame
        void Update()
        {
            bool sprint = Input.GetKey(KeyCode.LeftShift);
            maxSize = sprint ? myStepVisualizationManager.SneakRunSize : myStepVisualizationManager.SneakSize;
            scalingSpeed = sprint ? myStepVisualizationManager.ScalingSpeedSneakRun : myStepVisualizationManager.ScalingSpeedSneak;
            fadeDuration = sprint ? myStepVisualizationManager.FadeDurationSneakRun : myStepVisualizationManager.FadeDurationSneak;
        
            if (player.PlayerAnimationHandler.PlayerAnimator.GetBool("Flee"))
            {
                maxSize = myStepVisualizationManager.FleeSize;
                scalingSpeed = myStepVisualizationManager.ScalingSpeedFlee;
                fadeDuration = myStepVisualizationManager.FadeDurationFlee;
            }
            myProjector.projector.orthographicSize = currentSize;

            if (visualize)
            {
                if (currentSize <= maxSize)
                {
                    scalingSpeed = maxSize + minSize + scalingSpeed;
                    currentSize += scalingSpeed * Time.deltaTime;
                }
                else
                {
                    FadeVisualization();
                }
            }
        }

        public void VisualizeStep()
        {
            myMaterial.DOKill();
            myMaterial.color = myStepVisualizationManager.Color;
            visualize = true;
            //currentSize = minSize;

            //DOTween.To(() => currentSize, x => currentSize = x, maxSize, myStepVisualizationManager.ScalingDuration).OnComplete(FadeVisualization);
        }

        public void FadeVisualization()
        {
            myMaterial.DOColor(Color.black, fadeDuration).OnComplete(ResetStepVisualization);
        }

        public void ResetStepVisualization()
        {
            visualize = false;
            myMaterial.color = Color.black;
            currentSize = minSize;
        }
    }
}
