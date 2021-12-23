using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace BP
{
    public class StepVisualization : MonoBehaviour
    {
        [SerializeField] 
        private Color myColor;
        [SerializeField]
        private float minSize;
        [SerializeField]
        private float maxSize;

        [SerializeField]
        private float scalingDuration;
        [SerializeField]
        private float fadeDuration;
        
        private ProjectorForLWRP.ProjectorForLWRP myProjector;
        
        private float currentSize;

        private Material myMaterial;

        // Start is called before the first frame update
        void Start()
        {
            myProjector = GetComponent<ProjectorForLWRP.ProjectorForLWRP>();
            currentSize = myProjector.projector.orthographicSize;
            myMaterial = myProjector.projector.material;
            myMaterial.color = myColor;
        }

        // Update is called once per frame
        void Update()
        {
            myProjector.projector.orthographicSize = currentSize;
            if (Input.GetKeyDown(KeyCode.L))
            {
                VisualizeStep();
            }
        }

        public void VisualizeStep()
        {
            myMaterial.DOKill();
            myMaterial.color = myColor;
            currentSize = minSize;
            DOTween.To(() => currentSize, x => currentSize = x, maxSize, scalingDuration).OnComplete(FadeVisualization);
        }

        public void FadeVisualization()
        {
            myMaterial.DOColor(Color.black, fadeDuration).OnComplete(ResetStepVisualization);
        }

        public void ResetStepVisualization()
        {
            currentSize = minSize;
        }
    }
}
