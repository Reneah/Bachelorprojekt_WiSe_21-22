using System;
using System.Collections;
using System.Collections.Generic;
using BP._02_Scripts._03_Game;
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

    private MissionScore _myMissionScore;

    private bool _openMenu = false;
    
    //private bool _fadeIn = false;
    
    private void Start()
    {
        _myMissionScore = FindObjectOfType<MissionScore>();
        
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
        /*if (_fadeImage.color.a <= 0.95f)
        {
            _fadeIn = true;
        }
        
        if (_fadeImage.color.a >= 1 && _fadeIn)
        {
            //MasterAudio.ChangePlaylistByName("Start");
            //MasterAudio.StopAllOfSound("Forest");
            //MasterAudio.StopAllOfSound("Wind");
        }*/
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
        // Set static booleans to false to reset QuestManager on New Game Start
        CollectItem._backpackCollected = false;
        CollectItem._parchmentCollected = false;
        CollectItem._keyCollected = false;
        CollectItem._enteredStaircase = false;
        
        // Signal to the MissionScore.cs that the game has been started, so the gameplay timer starts
        _myMissionScore.GameStarted = true;
        
        _fadeImage.DOFade(1, 3).OnComplete(LoadGame);
    }

    private void LoadGame()
    {
        SceneManager.LoadScene(_introSceneName);
    }

    public void CreditsButton()
    {
        _fadeImage.DOFade(1, 3).OnComplete(LoadCredits);
    }
    private void LoadCredits()
    {
        SceneManager.LoadScene("Credits");
    }


    public void Exit()
    {
        Application.Quit();
    }
}
