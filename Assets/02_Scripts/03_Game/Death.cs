using System;
using System.Collections;
using System.Collections.Generic;
using DA.Menu;
using UnityEngine;
using UnityEngine.SceneManagement;
using untitledProject;

public class Death : MonoBehaviour
{
    private InGameMenu _inGameMenu;
    private PlayerController _playerController;

    [SerializeField] private float _openMenuCooldown = 2;

    private float _openCooldown = 0;

    private bool _dead = false;

    public bool Dead
    {
        get => _dead;
        set => _dead = value;
    }

    void Start()
    {
        _openCooldown = _openMenuCooldown;
        
        _inGameMenu = FindObjectOfType<InGameMenu>();
        _playerController = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        if (_dead)
        {
            _openCooldown -= Time.deltaTime;

            if (_openCooldown <= 0)
            {
                _inGameMenu.DeathScreen();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Death"))
        {
            _dead = true;
            
            //when the player hits the death collider, he will die and the character use the Ragdoll
            _playerController.PlayerAnimationHandler.PlayerAnimator.enabled = false;
            _playerController.enabled = false;

        }
    }
}
