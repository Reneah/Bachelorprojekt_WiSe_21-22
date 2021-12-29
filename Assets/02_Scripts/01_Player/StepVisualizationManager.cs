using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BP
{
    public class StepVisualizationManager : MonoBehaviour
    {
        [SerializeField]
        private StepVisualization stepVisualizationOne;
        [SerializeField]
        private StepVisualization stepVisualizationTwo;
        
        [SerializeField] 
        private Color color;
        
        [SerializeField] [Range(0.5f, 7)] private float minSize;
        [SerializeField] [Range(0.5f, 7)] private float sneakSize;
        [SerializeField] [Range(0.5f, 7)] private float sneakRunSize;
        [SerializeField] [Range(0.5f, 7)] private float fleeSize;
        
        [SerializeField]
        private float scalingSpeedSneak;
        [SerializeField]
        private float scalingSpeedSneakRun;
        [SerializeField]
        private float scalingSpeedFlee;
        [SerializeField]
        private float fadeDurationSneak;
        [SerializeField]
        private float fadeDurationSneakRun;
        [SerializeField]
        private float fadeDurationFlee;

        public float MinSize => minSize;
        public float SneakSize => sneakSize;
        public float SneakRunSize => sneakRunSize;
        public float FleeSize => fleeSize;
        

        public float ScalingSpeedSneak => scalingSpeedSneak;
        public float ScalingSpeedSneakRun => scalingSpeedSneakRun;
        public float ScalingSpeedFlee => scalingSpeedFlee;

        public float FadeDurationSneak => fadeDurationSneak;
        public float FadeDurationSneakRun => fadeDurationSneakRun;
        public float FadeDurationFlee => fadeDurationFlee;

        public Color Color => color;

        public void PlayStepVisualizationOne()
        {
            stepVisualizationOne.VisualizeStep();
        }
        
        public void PlayStepVisualizationTwo()
        {
            stepVisualizationTwo.VisualizeStep();
        }
    }
}
