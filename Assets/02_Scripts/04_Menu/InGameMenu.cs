using Enemy.Controller;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    private TutorialContinueButton _tutorialContinueButton;
    private Death _death;
    private MissionScore _myMissionScore;

    private EnemyController[] _enemyControllers;
    
    private bool _enemyCatchedPlayer;
    public bool EnemyCatchedPlayer
    {
        get => _enemyCatchedPlayer;
        set => _enemyCatchedPlayer = value;
    }

    private bool _playerIsChased = false;
    
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

    private bool _gotTutorialObject;

    private SceneChange _sceneChange;

    private void Start()
    {
        Cursor.SetCursor(_cursorTexture, Vector2.zero, CursorMode.Auto);
        
        _playerController = FindObjectOfType<PlayerController>();
        _playerAnimation = FindObjectOfType<PlayerAnimationHandler>();
        _tutorialContinueButton = FindObjectOfType<TutorialContinueButton>();
        _sceneChange = FindObjectOfType<SceneChange>();
        _death = FindObjectOfType<Death>();
        _myMissionScore = FindObjectOfType<MissionScore>();
        _enemyControllers = FindObjectsOfType<EnemyController>();
        
        // MasterAudio.PlaySound("Wind");
        // MasterAudio.PlaySound("Forest");
        // MasterAudio.PlaySound("TreeRustle");

        _dead = false;
    }

    private void Update()
    {
        if (_tutorialContinueButton.TutorialTrigger != null && !_gotTutorialObject)
        {
            _gotTutorialObject = true;
        }
        
        if (Input.GetKeyDown(KeyCode.Escape) && !_dead && !_gotTutorialObject && !_sceneChange.CurrentlyChangeScene || Input.GetKeyDown(KeyCode.Escape) && !_dead  && _gotTutorialObject && !_tutorialContinueButton.TutorialTrigger.TutorialWindowOpen && !_sceneChange.CurrentlyChangeScene)
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
            _death.Dead = true;
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
        PlayerPrefs.DeleteAll();
        //MasterAudio.ChangePlaylistByName("PlaylistController","Start");
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
        
        _deathPage.SetActive(false);

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
        //MasterAudio.PlaySound("OpenMenu");
    }

    public void ResumeToGame()
    {
        _openMenu = false;
        _hud.SetActive(true);
        _wholeMenu.SetActive(false);
        Time.timeScale = 1;
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
        // Depending on whether the player character is dead or not count up different score counters on the Restart button press
        // Also if player character has been spotted and is fleeing, players can't restart to cheat the score, as it will count as a death

        for (int i = 0; i < _enemyControllers.Length; i++)
        {
            if (_enemyControllers[i].InChaseState)
            {
                _playerIsChased = true;
                break;
            }
        }
        
        if (_dead || _playerIsChased)
        {
            // Count up the death score counter for the MissionScore.cs
            _myMissionScore.DeathScoreCounter += 1;
        }
        else
        {
            // Count up the restart score counter for the MissionScore.cs
            _myMissionScore.RestartScoreCounter += 1;
        }
        
        Time.timeScale = 1;
        _deathPage.SetActive(false);
        _playerIsChased = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
