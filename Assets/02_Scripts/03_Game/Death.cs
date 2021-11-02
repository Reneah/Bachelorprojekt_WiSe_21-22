using System;
using System.Collections;
using System.Collections.Generic;
using DA.Menu;
using UnityEngine;
using UnityEngine.SceneManagement;
using untitledProject;

public class Death : MonoBehaviour
{
    [SerializeField] private GameObject _deathScene;
    private PlayerController _player;
    private bool _enemyCatchedPlayer;

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
        SceneManager.LoadScene("01_Scenes/LevelDesign/GameWorld_BesiegedKeep_2");
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
       // _player.enabled = true;
        _deathScene.SetActive(false);
        SceneManager.LoadScene("MainMenu");
    }

    public void CloseGame()
    {
        Time.timeScale = 1;
        Application.Quit();
    }
}
