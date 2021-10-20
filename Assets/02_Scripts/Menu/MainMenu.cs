using System;
using System.Collections;
using System.Collections.Generic;
//using DarkTonic.MasterAudio;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private Image _fadeImage;

    private void Start()
    {
        //MasterAudio.ChangePlaylistByName("PlaylistController","Start");
        //MasterAudio.StopAllOfSound("Forest");
        //MasterAudio.StopAllOfSound("Wind");
    }

    private void Update()
    {
        if (_fadeImage.color.a >= 1)
        {
            SceneManager.LoadScene("Start");
            
            //MasterAudio.ChangePlaylistByName("Start");
            //MasterAudio.StopAllOfSound("Forest");
            //MasterAudio.StopAllOfSound("Wind");
        }
    }

    public void StartGame()
    {
        _fadeImage.DOFade(1, 3);
    }

    public void Exit()
    {
        Application.Quit();
    }

}
