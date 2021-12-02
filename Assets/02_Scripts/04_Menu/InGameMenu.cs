﻿using System;
using System.Collections;
using System.Collections.Generic;
using DA.Menu;
using DarkTonic.MasterAudio;
//using DarkTonic.MasterAudio;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using untitledProject;

public class InGameMenu : MonoBehaviour
{
    [Header("Menu Pages")]
    [SerializeField] private GameObject _wholeMenu;
    [SerializeField] private GameObject _optionPage;
    [SerializeField] private GameObject _menuPage;
    [SerializeField] private GameObject _graphicsPage;
    [SerializeField] private GameObject _audioPage;
    [SerializeField] private GameObject _controlsPage;
    [SerializeField] private GameObject _deathPage;

    private PlayerController _playerController;
    private PlayerAnimationHandler _playerAnimation;
    
    private bool _enemyCatchedPlayer;
    public bool EnemyCatchedPlayer
    {
        get => _enemyCatchedPlayer;
        set => _enemyCatchedPlayer = value;
    }
    
    // activate the restart fade to the loading scene
    // can't open the menu anymore
    private bool _dead = false;

    [Header("Sound")]
    [SerializeField] private Slider _soundSlider;
    
    public Slider SoundSlider
    {
        get => _soundSlider;
        set => _soundSlider = value;
    }
    
    [Header("HUD")]
    [SerializeField] private GameObject _hud;

    [SerializeField] private Texture2D _cursorTexture;

    private bool _openMenu = false;

    private void Start()
    {
        Cursor.SetCursor(_cursorTexture, Vector2.zero, CursorMode.Auto);
        
        _playerController = FindObjectOfType<PlayerController>();
        _playerAnimation = FindObjectOfType<PlayerAnimationHandler>();
        
        // MasterAudio.PlaySound("Wind");
        // MasterAudio.PlaySound("Forest");
        // MasterAudio.PlaySound("TreeRustle");

        _dead = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !_dead)
        {
            if (!_openMenu)
            {
                OpenInGameMenu();
                _openMenu = true;
            }
            else
            {
                ResumeToGame();
                _openMenu = false;
            }
        }
        
        if (EnemyCatchedPlayer)
        {
            _playerAnimation.PlayerDeath();
            _playerController.enabled = false;
            _deathPage.SetActive(true);
            _enemyCatchedPlayer = false;
        }
    }
    
    public void GoToOptionPage()
    {
        _menuPage.SetActive(false);
        _optionPage.SetActive(true);
    }

    public void BackToMenuPage()
    {
        _menuPage.SetActive(true);
        _optionPage.SetActive(false);
    }

    public void BackToMainMenu()
    {
        //MasterAudio.ChangePlaylistByName("PlaylistController","Start");
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
        
        _deathPage.SetActive(false);
        PlayerPrefs.DeleteAll();
    }

    public void CloseGame()
    {
        PlayerPrefs.DeleteAll();
        Application.Quit();
    }

    public void OpenInGameMenu()
    {
        _hud.SetActive(false);
        _wholeMenu.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        //MasterAudio.PlaySound("OpenMenu");
    }

    public void ResumeToGame()
    {
        _openMenu = false;
        _hud.SetActive(true);
        _wholeMenu.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //MasterAudio.PlaySound("CloseMenu");
    }

    public void OpenGraphicsPage()
    {
        _graphicsPage.SetActive(true);
        _optionPage.SetActive(false);
    }

    public void BackToOptionPage()
    {
        _optionPage.SetActive(true);
        _graphicsPage.SetActive(false);
        _audioPage.SetActive(false);
        _controlsPage.SetActive(false);
    }

    public void OpenAudioMenu()
    {
        _optionPage.SetActive(false);
        _audioPage.SetActive(true); 
    }

    public void OpenControlsPage()
    {
        _controlsPage.SetActive(true);
        _optionPage.SetActive(false);
    }
    
    public void DeathScreen()
    {
        _playerController.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _dead = true;
        _deathPage.SetActive(true);
        //MasterAudio.ChangePlaylistByName("PlaylistController","Death");
        //MasterAudio.PlaySound("DeathChoir");
        
        _hud.SetActive(false);
    }

    public void Restart()
    {
        Time.timeScale = 1;
        _deathPage.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
