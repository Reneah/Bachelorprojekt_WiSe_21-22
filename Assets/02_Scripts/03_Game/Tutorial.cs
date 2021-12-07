using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class Tutorial : MonoBehaviour
    {
        [SerializeField] private GameObject _tutorialWindow;
        [SerializeField] private GameObject[] _content;
        [SerializeField] private GameObject[] _sprites;

        private int _contentCounter = 0;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Time.timeScale = 0;
                _tutorialWindow.SetActive(true);
                
                if(_content[0] != null)
                _content[0].SetActive(true);
                
                if(_sprites[0] != null)
                _sprites[0].SetActive(true);
            }
        }

        public void Continue()
        {
            if(_sprites[_contentCounter] != null)
                _sprites[_contentCounter].SetActive(false);
            if(_content[_contentCounter] != null)
                _content[_contentCounter].SetActive(false);
            
            _contentCounter++;

            if (_contentCounter >= _content.Length)
            {
                Time.timeScale = 1;
                _tutorialWindow.SetActive(false);
                
                for (int i = 0; i < _content.Length; i++)
                {   
                    _content[i].SetActive(false);
                }
                for (int i = 0; i < _sprites.Length; i++)
                {   
                    _sprites[i].SetActive(false);
                }
                gameObject.SetActive(false);
                return;
            }
            
            if(_sprites[_contentCounter] != null)
            _sprites[_contentCounter].SetActive(true);
            if(_content[_contentCounter] != null)
            _content[_contentCounter].SetActive(true);
        }
    }

