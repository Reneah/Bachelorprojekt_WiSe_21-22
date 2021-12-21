using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BP._02_Scripts._03_Game
{
    public class MissionScore : MonoBehaviour
    {
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

        private void Awake()
        {
            _myCanvas = this.GetComponent<Canvas>();
            _myCanvas.enabled = false;

            _myCollectProvisions = FindObjectOfType<CollectProvisions>();
            _myCollectStones = FindObjectOfType<CollectStones>();
        }

        // Start is called before the first frame update
        void Start()
        {
            if (_playerFinishedGame && !_scoresCalculated)
            {
                _myCanvas.enabled = true;
                CalculateScores();
                ChangeFinalResultText(_finalScoreResult);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (_gameStarted && !_playerFinishedGame)
            {
                _playtimeScoreCounter += Time.deltaTime;
            }
            
            //Update some scores during gameplay
            _provisionsScoreCounter = _myCollectProvisions.ProvisionsCounter;
            _stonesScoreCounter = _myCollectStones.StonesCounter;
        }

        // Resets all required values for the Mission Score to the starting state on new playthrough attempt
        private void ResetMissionScores()
        {
            _playerFinishedGame = false;
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
            SceneManager.LoadScene("MainMenu");
            ResetMissionScores();
        }

        private void CalculateScores()
        {
            _scoresCalculated = true;
            // Calculates the NEGATIVE points to be deduced for the play time
            int playtimePoints = (int)(Math.Round(_playtimeScoreCounter, 0, MidpointRounding.AwayFromZero) * _playtimePointsMultiplier);
            // Sets the value to be displayed as a text
            _playtimeScore.text = playtimePoints.ToString();
            
            // Calculates the POSITIVE points to be added for the acquired provisions
            int provisionsPoints = _provisionsScoreCounter * _provisionsPointsMultiplier;
            // Sets the value to be displayed as a text
            _provisionsScore.text = provisionsPoints.ToString();
           
            // Calculates the POSITIVE points to be added for the acquired stones
            int stonesPoints = _stonesScoreCounter * _stonesPointsMultiplier;
            // Sets the value to be displayed as a text
            _stonesScore.text = stonesPoints.ToString();
            
            // Calculates the POSITIVE points to be added for the distractions with Noisy Items
            int distractionsPoints = DistractionsScoreCounter * _noisyItemPointsMultiplier;
            // Sets the value to be displayed as a text
            _noisyItemScore.text = distractionsPoints.ToString();
            
            // Calculates the NEGATIVE points to be deduced for the player having been spotted
            int spottedPoints = SpottedScoreCounter * _playerSpottedPointsMultiplier;
            // Sets the value to be displayed as a text
            _playerSpottedScore.text = spottedPoints.ToString();
            
            // Calculates the NEGATIVE points to be deduced for the player having failed the game (death or getting caught)
            int deathPoints = DeathScoreCounter * _gameOverPointsMultiplier;
            // Sets the value to be displayed as a text
            _gameOverScore.text = deathPoints.ToString();
            
            // Calculates the NEGATIVE points to be deduced for the player having been spotted
            int restartPoints = RestartScoreCounter * _checkpointRestartPointsMultiplier;
            // Sets the value to be displayed as a text
            _checkpointRestartScore.text = restartPoints.ToString();
            
            // Calculate the final score with all of the sub score values
            _finalScoreResult = 10000 - playtimePoints + provisionsPoints + stonesPoints + distractionsPoints - spottedPoints - deathPoints - restartPoints;
            // Sets the value to be displayed as a text
            _finalGameScore.text = _finalScoreResult.ToString();
        }

        // Activates the appropriate text, depending on the final score result
        private void ChangeFinalResultText(int finalScore)
        {
            
            //Only necessary if references can't be found more cheaply
            /*_excellentResultMessage = GameObject.Find("Text_FinalScore_GreatResult");
            _goodResultMessage = GameObject.Find("Text_FinalScore_GoodResult");
            _averageResultMessage = GameObject.Find("Text_FinalScore_AverageResult");*/
            
            if (finalScore <= 4999)
            {
                _averageResultMessage.SetActive(true);
                _goodResultMessage.SetActive(false);
                _excellentResultMessage.SetActive(false);
            }
            else if (finalScore <= 7499)
            {
                _averageResultMessage.SetActive(false);
                _goodResultMessage.SetActive(true);
                _excellentResultMessage.SetActive(false);
            }
            else if (finalScore >= 7500)
            {
                _averageResultMessage.SetActive(false);
                _goodResultMessage.SetActive(false);
                _excellentResultMessage.SetActive(true);
            }
        }
    }
}
