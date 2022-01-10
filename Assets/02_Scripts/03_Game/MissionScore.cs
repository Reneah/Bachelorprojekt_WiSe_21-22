using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BP._02_Scripts._03_Game
{
    public sealed class MissionScore : MonoBehaviour
    {
        
        // Singleton pattern - simple thread-safety
        private static MissionScore _instance = null;
        private static readonly object _padlock = new object();

        MissionScore()
        {
        }

        public static MissionScore Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new MissionScore();
                    }
                    return _instance;
                }
            }
        }
        
        private Canvas _myCanvas;

        private bool _gameStarted;
        private bool _playerFinishedGame;
        private bool _scoresCalculated;

        private float _playtimeScoreCounter;
        private int _provisionsScoreCounter;
        private int _stonesScoreCounter;
        private int _distractionsScoreCounter;
        private int _spottedScoreCounter;
        private int _deathScoreCounter;
        private int _restartScoreCounter;

        private CollectProvisions _myCollectProvisions;
        private CollectStones _myCollectStones;
        
        // Probably not needed unless we want to display the highscore somewhere in a Main Menu sub page
        /*private float _playtimeScoreResult;
        private int _provisionsScoreResult;
        private int _stonesScoreResult;
        private int _distractionsScoreResult;
        private int _spottedScoreResult;
        private int _deathScoreResult;
        private int _restartScoreResult;*/
        private int _finalScoreResult;

        [Header("Text fields to be assigned to certain calculations.")]
        [Tooltip("The text field for the playtime to be displayed.")]
        [SerializeField] private TextMeshProUGUI _playtimeDisplay;
        [Tooltip("The text field for the end score of the playtime.")]
        [SerializeField] private TextMeshProUGUI _playtimeScore;
        [Tooltip("The value that the playtime is multiplied with.")]
        [SerializeField] private int _playtimePointsMultiplier;
        [Tooltip("The text field for the provisions count to be displayed.")]
        [SerializeField] private TextMeshProUGUI _provisionsDisplay;
        [Tooltip("The text field for the end score of the provisions count.")]
        [SerializeField] private TextMeshProUGUI _provisionsScore;
        [Tooltip("The value that the provisions count is multiplied with.")]
        [SerializeField] private int _provisionsPointsMultiplier;
        [Tooltip("The text field for the stones count to be displayed.")]
        [SerializeField] private TextMeshProUGUI _stonesDisplay;
        [Tooltip("The text field for the end score of the stones count.")]
        [SerializeField] private TextMeshProUGUI _stonesScore;
        [Tooltip("The value that the stones count is multiplied with.")]
        [SerializeField] private int _stonesPointsMultiplier;
        [Tooltip("The text field for the Noisy Items to be displayed.")]
        [SerializeField] private TextMeshProUGUI _noisyItemDisplay;
        [Tooltip("The text field for the end score of the Noisy Items count.")]
        [SerializeField] private TextMeshProUGUI _noisyItemScore;
        [Tooltip("The value that the Noisy Items count is multiplied with.")]
        [SerializeField] private int _noisyItemPointsMultiplier;
        [Tooltip("The text field for the spotted count to be displayed.")]
        [SerializeField] private TextMeshProUGUI _playerSpottedDisplay;
        [Tooltip("The text field for the end score of the spotted count.")]
        [SerializeField] private TextMeshProUGUI _playerSpottedScore;
        [Tooltip("The value that the spotted count is multiplied with.")]
        [SerializeField] private int _playerSpottedPointsMultiplier;
        [Tooltip("The text field for the game over count to be displayed.")]
        [SerializeField] private TextMeshProUGUI _gameOverDisplay;
        [Tooltip("The text field for the end score of the game over count.")]
        [SerializeField] private TextMeshProUGUI _gameOverScore;
        [Tooltip("The value that the game over count is multiplied with.")]
        [SerializeField] private int _gameOverPointsMultiplier;
        [Tooltip("The text field for the checkpoint restart count to be displayed.")]
        [SerializeField] private TextMeshProUGUI _checkpointRestartDisplay;
        [Tooltip("The text field for the end score of the checkpoint restart count.")]
        [SerializeField] private TextMeshProUGUI _checkpointRestartScore;
        [Tooltip("The value that the checkpoint restart count is multiplied with.")]
        [SerializeField] private int _checkpointRestartPointsMultiplier;
        [Tooltip("The text field of the overall game score.")]
        [SerializeField] private TextMeshProUGUI _finalGameScore;
        
        [Tooltip("The text to be displayed with an excellent score result (>=10k).")]
        [SerializeField] private GameObject _excellentResultMessage;
        [Tooltip("The text to be displayed with an good score result (<=7.5k+).")]
        [SerializeField] private GameObject _goodResultMessage;
        [Tooltip("The text to be displayed with an good score result (<=5k).")]
        [SerializeField] private GameObject _averageResultMessage;
        
        [SerializeField] private Image _fadeImage;
        [SerializeField] private float _fadeSpeed;
        private bool _activateFade;
        
        public int DistractionsScoreCounter
        {
            get => _distractionsScoreCounter;
            set => _distractionsScoreCounter = value;
        }

        public int SpottedScoreCounter
        {
            get => _spottedScoreCounter;
            set => _spottedScoreCounter = value;
        }

        public int DeathScoreCounter
        {
            get => _deathScoreCounter;
            set => _deathScoreCounter = value;
        }

        public int RestartScoreCounter
        {
            get => _restartScoreCounter;
            set => _restartScoreCounter = value;
        }

        public bool GameStarted
        {
            get => _gameStarted;
            set => _gameStarted = value;
        }

        public bool PlayerFinishedGame
        {
            get => _playerFinishedGame;
            set => _playerFinishedGame = value;
        }

        private void Awake()
        {
            _myCanvas = this.GetComponent<Canvas>();
            _myCanvas.enabled = false;
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.name == "MainMenu" && loadSceneMode == LoadSceneMode.Single)
            {
                _myCanvas.enabled = false;
            }
            
            if (scene.name == "GameWorld_BesiegedKeep_3" && loadSceneMode == LoadSceneMode.Single && _myCollectStones == null)
            {
               _myCollectStones = FindObjectOfType<CollectStones>();
               _myCollectProvisions = FindObjectOfType<CollectProvisions>();
            }
            else if (scene.name == "MissionScore" && loadSceneMode == LoadSceneMode.Single)
            {
                _activateFade = true;
                
                if (!_scoresCalculated)
                {
                    _myCanvas.enabled = true;
                    DisplayCounterValues();
                    CalculateScores();
                    ChangeFinalResultText(_finalScoreResult);
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            // Runs a timer as soon as the game is started, until the player finishes it,
            // by interacting with the staircase to the cellar
            if (GameStarted && !PlayerFinishedGame)
            {
                _playtimeScoreCounter += Time.deltaTime;
            }

            if (_activateFade)
            {
                _fadeImage.DOFade(0, _fadeSpeed);
                _activateFade = false;
            }
        }
        
        // Resets all required values for the Mission Score to the starting state on new playthrough attempt
        private void ResetMissionScores()
        {
            _gameStarted = false;
            PlayerFinishedGame = false;
            _scoresCalculated = false;
            
            _playtimeScoreCounter = 0;
            _provisionsScoreCounter = 0;
            _stonesScoreCounter = 0;
            DistractionsScoreCounter = 0;
            SpottedScoreCounter = 0;
            DeathScoreCounter = 0;
            RestartScoreCounter = 0;
        }
        
        // Used when clicking the Continue button on the Mission Score screen
        public void BackToMainMenu()
        {
            _fadeImage.DOFade(1, _fadeSpeed).OnComplete(LoadMainMenu);
        }

        private void LoadMainMenu()
        {
            ResetMissionScores();
            SceneManager.LoadScene("MainMenu");
        }

        // Overwrites placeholder values with actual values from the player
        private void DisplayCounterValues()
        {
            _playtimeDisplay.text = (Math.Round(_playtimeScoreCounter, 0, MidpointRounding.AwayFromZero).ToString());
            _provisionsDisplay.text = _provisionsScoreCounter.ToString();
            _stonesDisplay.text = _stonesScoreCounter.ToString();
            _noisyItemDisplay.text = DistractionsScoreCounter.ToString();
            _playerSpottedDisplay.text = SpottedScoreCounter.ToString();
            _gameOverDisplay.text = DeathScoreCounter.ToString();
            _checkpointRestartDisplay.text = RestartScoreCounter.ToString();
        }

        // Calculates all the individual scores and exchanges placeholder scores with them
        private void CalculateScores()
        {
            _scoresCalculated = true;
            // Calculates the NEGATIVE points to be deduced for the play time
            int playtimePoints = (int)(Math.Round(_playtimeScoreCounter, 0, MidpointRounding.AwayFromZero) * _playtimePointsMultiplier);
            // Sets the value to be displayed as a text
            _playtimeScore.text = "- " + playtimePoints;
            
            // Calculates the POSITIVE points to be added for the acquired provisions
            int provisionsPoints = _provisionsScoreCounter * _provisionsPointsMultiplier;
            // Sets the value to be displayed as a text
            _provisionsScore.text = "+ " + provisionsPoints;
           
            // Calculates the POSITIVE points to be added for the acquired stones
            int stonesPoints = _stonesScoreCounter * _stonesPointsMultiplier;
            // Sets the value to be displayed as a text
            _stonesScore.text = "+ " + stonesPoints;
            
            // Calculates the POSITIVE points to be added for the distractions with Noisy Items
            int distractionsPoints = DistractionsScoreCounter * _noisyItemPointsMultiplier;
            // Sets the value to be displayed as a text
            _noisyItemScore.text = "+ " + distractionsPoints;
            
            // Calculates the NEGATIVE points to be deduced for the player having been spotted
            int spottedPoints = SpottedScoreCounter * _playerSpottedPointsMultiplier;
            // Sets the value to be displayed as a text
            _playerSpottedScore.text = "- " + spottedPoints;
            
            // Calculates the NEGATIVE points to be deduced for the player having failed the game (death or getting caught)
            int deathPoints = DeathScoreCounter * _gameOverPointsMultiplier;
            // Sets the value to be displayed as a text
            _gameOverScore.text = "- " + deathPoints;
            
            // Calculates the NEGATIVE points to be deduced for the player having been spotted
            int restartPoints = RestartScoreCounter * _checkpointRestartPointsMultiplier;
            // Sets the value to be displayed as a text
            _checkpointRestartScore.text = "- " + restartPoints;
            
            // Calculate the final score with all of the sub score values
            _finalScoreResult = 10000 - playtimePoints + provisionsPoints + stonesPoints + distractionsPoints - spottedPoints - deathPoints - restartPoints;
            // Sets the value to be displayed as a text
            _finalGameScore.text = _finalScoreResult.ToString();
        }

        // Activates the appropriate text, depending on the final score result
        private void ChangeFinalResultText(int finalScore)
        {
            if (finalScore <= 4999)
            {
                _averageResultMessage.SetActive(true);
                _goodResultMessage.SetActive(false);
                _excellentResultMessage.SetActive(false);
            }
            else if (finalScore <= 9999)
            {
                _averageResultMessage.SetActive(false);
                _goodResultMessage.SetActive(true);
                _excellentResultMessage.SetActive(false);
            }
            else if (finalScore >= 10000)
            {
                _averageResultMessage.SetActive(false);
                _goodResultMessage.SetActive(false);
                _excellentResultMessage.SetActive(true);
            }
        }

        public void GrabStonesAndProvisionsValues()
        {
            _stonesScoreCounter = _myCollectStones.StonesCounter;
            _provisionsScoreCounter = _myCollectProvisions.ProvisionsCounter;
        }
    }
}
