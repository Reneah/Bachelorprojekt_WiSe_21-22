using System;
using System.Collections;
using System.Collections.Generic;
using Enemy.Controller;
using UnityEngine;
using untitledProject;

public class Tutorial : MonoBehaviour
    {
        [SerializeField] private string _playerPrefsKey;
        [SerializeField] private GameObject _tutorialWindow;
        [SerializeField] private GameObject[] _content;
        [SerializeField] private GameObject[] _sprites;
        [SerializeField] private GameObject _hud;
        [SerializeField] private bool _noTutorialAtChase;

        private PlayerController _playerController;
        private PlayerStepsSound _playerStepsSound;
        private EnemyController[] _enemyController;

        private int _contentCounter = 0;

        private bool _tutorialWindowOpen = false;

        private bool _reactivateTutorialSpot = true;

        private void OnEnable()
        {
            _playerController = FindObjectOfType<PlayerController>();
            _playerStepsSound = FindObjectOfType<PlayerStepsSound>();
            _enemyController = FindObjectsOfType<EnemyController>();
            _reactivateTutorialSpot = System.Convert.ToBoolean(PlayerPrefs.GetInt(_playerPrefsKey, 1));
            
            if (!_reactivateTutorialSpot)
            {
                gameObject.SetActive(false);
                _reactivateTutorialSpot = true;
            }
        }

        public bool TutorialWindowOpen
        {
            get => _tutorialWindowOpen;
            set => _tutorialWindowOpen = value;
        }

        private void OnTriggerEnter(Collider other)
        {
            for (int i = 0; i < _enemyController.Length; i++)
            {
                if (_noTutorialAtChase && _enemyController[i].InChaseState)
                {
                    return;
                }
            }
            
            if (other.CompareTag("Player"))
            {
                _playerController.enabled = false;
                _playerStepsSound.enabled = false;
                    
                _hud.SetActive(false);
                _tutorialWindowOpen = true;
                
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
                _playerController.enabled = true;
                _playerStepsSound.enabled = true;
                
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
                _hud.SetActive(true);
                _tutorialWindowOpen = false;

                PlayerPrefs.SetInt(_playerPrefsKey, 0);
                return;
            }
            
            if(_sprites[_contentCounter] != null)
            _sprites[_contentCounter].SetActive(true);
            if(_content[_contentCounter] != null)
            _content[_contentCounter].SetActive(true);
        }
    }

