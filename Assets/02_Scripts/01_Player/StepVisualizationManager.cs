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
