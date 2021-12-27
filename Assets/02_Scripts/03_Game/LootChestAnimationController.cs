using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using untitledProject;

    public class LootChestAnimationController : MonoBehaviour
    {
        private Animator _animator;
        private static readonly int ChestOpen = Animator.StringToHash("OpenChest");
        
        void Start()
        {
            _animator = GetComponent<Animator>();
        }
        
        public void OpenChest(bool open)
        {
            _animator.SetBool(ChestOpen, open);
        }
    }

