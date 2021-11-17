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

public class InGameMenu : MonoBehaviour
{
    [Header("Menu Pages")]
    [SerializeField] private GameObject _optionPage;
    [SerializeField] private GameObject _menuPage;
    [SerializeField] private GameObject _wholeMenu;
    [SerializeField] private GameObject _graphicsPage;
    [SerializeField] private GameObject _audioPage;
    [SerializeField] private GameObject _controlsPage;
    [SerializeField] private GameObject _deathPage;
    [SerializeField] private Image _deathBackground;
    [SerializeField] private TextMeshProUGUI[] _deathTexts;
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

    [SerializeField] private Image _fadeImage;
    // this script is in the main menu and in the plaing scene, thus i don't need the fade at both scenes
    [SerializeField] private bool _fade = false;

    [Header("HUD")]
    [SerializeField] private GameObject _hud;

    [SerializeField] private Texture2D _cursorTexture;

    private bool _openMenu = false;

    private void Start()
    {
        Cursor.SetCursor(_cursorTexture,Vector2.zero, CursorMode.Auto);
        
       // MasterAudio.PlaySound("Wind");
       // MasterAudio.PlaySound("Forest");
       // MasterAudio.PlaySound("TreeRustle");
        
        _dead = false;
        
        if (_fade)
        {
            _fadeImage.DOFade(0, 3);
            
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && _fadeImage.color.a <= 0 && !_dead)
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
        
        if (_dead)
        {
            if (_fadeImage.color.a >= 1)
            {
                SceneManager.LoadScene("Loading");
            }
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
    }

    public void OpenInGameMenu()
    {
        _wholeMenu.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        //MasterAudio.PlaySound("OpenMenu");
    }

    public void ResumeToGame()
    {
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

    public void ChangeSoundVolume()
    {
        ScenePersistent.SoundVolume = _soundSlider.value;
        //MasterAudio.MasterVolumeLevel = _soundSlider.value;
    }

    public void DeathScreen()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _dead = true;
        _deathPage.SetActive(true);
        //MasterAudio.ChangePlaylistByName("PlaylistController","Death");
        //MasterAudio.PlaySound("DeathChoir");
        
        _deathBackground.DOFade(0.5f, 1.5f);
        _hud.SetActive(false);

        for (int i = 0; i < _deathTexts.Length; i++)
        {
            _deathTexts[i].DOFade(1, 3);
        }
    }

    public void Restart()
    {
        Time.timeScale = 1;
        _fadeImage.DOFade(1, 3);
        _deathPage.SetActive(false);
    }
}
