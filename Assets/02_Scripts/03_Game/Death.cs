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
    void Start()
    {
        _inGameMenu = FindObjectOfType<InGameMenu>();
        _playerController = FindObjectOfType<PlayerController>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Death"))
        {
            _inGameMenu.DeathScreen();
            
            //when the player hits the death collider, he will die and the character use the Ragdoll
            _playerController.PlayerAnimationHandler.PlayerAnimator.enabled = false;

        }
    }
}
