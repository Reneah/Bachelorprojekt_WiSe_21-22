using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class PlayerGroundDetection : MonoBehaviour
    {
        private bool _lowGround;
        private bool _highGround;
        
        public bool LowGround
        {
            get => _lowGround;
            set => _lowGround = value;
        }
        
        public bool HighGround
        {
            get => _highGround;
            set => _highGround = value;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("HighGround"))
            {
                _highGround = true;
                _lowGround = false;
            }

            if (other.CompareTag("LowGround"))
            {
                _highGround = false;
                _lowGround = true;
            }
        }
    }

