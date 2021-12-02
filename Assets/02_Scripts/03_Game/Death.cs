using System;
using System.Collections;
using System.Collections.Generic;
using DA.Menu;
using UnityEngine;
using UnityEngine.SceneManagement;
using untitledProject;

public class Death : MonoBehaviour
{
    [Tooltip("the parent of the death scene to activate and deactivate the menu")]
    [SerializeField] private GameObject _deathScene;
    private PlayerController _player;
    private bool _enemyCatchedPlayer;
    [Tooltip("the scene that should be restarted when the player is dead")]
    [SerializeField] private string _reloadSceneName;
    [Tooltip("the name of the main menu scene")]
    [SerializeField] private string _mainMenuName;

    [Tooltip("the parent of the main menu object")]
    [SerializeField] private GameObject _mainMenuParent;

    private PlayerAnimationHandler _playerAnimationHandler;

    private bool _escape = true;

    public bool EnemyCatchedPlayer
    {
        get => _enemyCatchedPlayer;
        set => _enemyCatchedPlayer = value;
    }

    void Start()
    {
        _deathScene.SetActive(false);
        _mainMenuParent.SetActive(false);
        
        _player = GetComponent<PlayerController>();
        _playerAnimationHandler = GetComponent<PlayerAnimationHandler>();
    }

    private void Update()
    {
        PauseMenu();
        
        if (EnemyCatchedPlayer)
        {
            _playerAnimationHandler.PlayerDeath();
            _player.enabled = false;
            _deathScene.SetActive(true);
            _enemyCatchedPlayer = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Death"))
        {
            _playerAnimationHandler.PlayerDeath();
            _player.enabled = false;
            _deathScene.SetActive(true);
        }
    }

    public void Restart()
    {
        // Temporary addition, so the stones UI display works properly on player death
        CollectStones._UIdisplayed = false;
        CollectStones._stonesActive = false;
        
        _player.enabled = true;
        _deathScene.SetActive(false);
        SceneManager.LoadScene(_reloadSceneName);
    }

    public void MainMenu()
    {
        _player.enabled = true;
        _deathScene.SetActive(false);
        SceneManager.LoadScene(_mainMenuName);
        PlayerPrefs.DeleteAll();
    }

    public void CloseGame()
    {
        Time.timeScale = 1;
        PlayerPrefs.DeleteAll();
        Application.Quit();
    }

    public void PauseMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_escape)
            {
                _mainMenuParent.SetActive(true);
                _escape = false;
            }
            else
            {
                _mainMenuParent.SetActive(false);
                _escape = true;
            }
        }

 
    }
}
