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

    public bool EnemyCatchedPlayer
    {
        get => _enemyCatchedPlayer;
        set => _enemyCatchedPlayer = value;
    }

    void Start()
    {
        _deathScene.SetActive(false);
        _player = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (EnemyCatchedPlayer)
        {
            Time.timeScale = 0;
           // _player.enabled = false;
            _deathScene.SetActive(true);
            _enemyCatchedPlayer = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Death"))
        {
            Time.timeScale = 0;
            //_player.enabled = false;
            _deathScene.SetActive(true);
        }
    }

    public void Restart()
    {
        Time.timeScale = 1;
        //_player.enabled = true;
        _deathScene.SetActive(false);
        SceneManager.LoadScene(_reloadSceneName);
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
       // _player.enabled = true;
        _deathScene.SetActive(false);
        SceneManager.LoadScene(_mainMenuName);
    }

    public void CloseGame()
    {
        Time.timeScale = 1;
        Application.Quit();
    }
}
