using System;
using System.Collections;
using System.Collections.Generic;
using DA.Menu;
//using DarkTonic.MasterAudio;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Pages")]
    [SerializeField] private GameObject _optionPage;
    [SerializeField] private GameObject _mainPage;
    [SerializeField] private GameObject _graphicsPage;
    [SerializeField] private GameObject _audioPage;
    [SerializeField] private GameObject _controlsPage;
    
    [SerializeField] private Image _fadeImage;
    [SerializeField] private Texture2D _cursorTexture;

    [SerializeField] private string _introSceneName;

    private bool _openMenu = false;
    
    private bool _fadeIn = false;
    
    private void Start()
    {
        PlayerPrefs.DeleteAll();
        
        Cursor.SetCursor(_cursorTexture,Vector2.zero, CursorMode.Auto);
        
       // MasterAudio.PlaySound("Wind");
       // MasterAudio.PlaySound("Forest");
       // MasterAudio.PlaySound("TreeRustle");
       
       //MasterAudio.ChangePlaylistByName("PlaylistController","Start");
       //MasterAudio.StopAllOfSound("Forest");
       //MasterAudio.StopAllOfSound("Wind");
       
       _fadeImage.DOFade(0, 3);
        
    }

    private void Update()
    {
        if (_fadeImage.color.a <= 0.95f)
        {
            _fadeIn = true;
        }
        

        if (_fadeImage.color.a >= 1 && _fadeIn)
        {
            SceneManager.LoadScene(_introSceneName);
            
            //MasterAudio.ChangePlaylistByName("Start");
            //MasterAudio.StopAllOfSound("Forest");
            //MasterAudio.StopAllOfSound("Wind");
        }
    }
    
    public void GoToOptionPage()
    {
        _mainPage.SetActive(false);
        _optionPage.SetActive(true);
    }

    public void BackToMenuPage()
    {
        _mainPage.SetActive(true);
        _optionPage.SetActive(false);
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
    
    public void StartGame()
    {
        _fadeImage.DOFade(1, 3);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
