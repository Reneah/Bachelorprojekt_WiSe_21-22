using UnityEngine;

    public class TutorialContinueButton : MonoBehaviour
    {
        private Tutorial _tutorialTrigger;

        public Tutorial TutorialTrigger
        {
            get => _tutorialTrigger;
            set => _tutorialTrigger = value;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("TutorialTrigger"))
            {
                _tutorialTrigger = other.GetComponent<Tutorial>();
            }
        }

        public void Continue()
        {
            _tutorialTrigger.Continue();
        }
    }
