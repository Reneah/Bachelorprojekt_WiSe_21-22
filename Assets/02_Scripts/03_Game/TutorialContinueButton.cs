using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class TutorialContinueButton : MonoBehaviour
    {
        private Tutorial _tutorialTrigger;
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
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
