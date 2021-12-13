using System;
using System.Collections;
using System.Collections.Generic;
using Enemy.Controller;
using UnityEngine;

namespace Enemy.SoundItem
{
    public class NoisyItemPullRadius : MonoBehaviour
    {
        private NoisyItem _noisyItem;
        
        // Start is called before the first frame update
        void Start()
        {
            _noisyItem = GetComponentInParent<NoisyItem>();
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        
        
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                
                
                Debug.Log("yes");
            }
        }
    }
}
