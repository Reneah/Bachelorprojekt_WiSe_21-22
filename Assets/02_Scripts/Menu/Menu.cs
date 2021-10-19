using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DA.Menu
{
    public class Menu : MonoBehaviour
    {
        // all buttons with text
        Animator[] _animator;

        [Header("Buttons")]
        [Tooltip("Play Modes Buttons")]
        [SerializeField] GameObject[] _playModesButtons;
        [Tooltip("Main Menu Buttons")]
        [SerializeField] GameObject[] _menuButtons;
        [SerializeField] GameObject[] _extrasButtons;
        [SerializeField] GameObject[] _singleplayerButtons;

        [Header("Windows")]
        [Tooltip("Multiplayer Window")]
        [SerializeField] GameObject[] _multiplayerWindow;
        [Tooltip("the content for the multiplayer window")]
        [SerializeField] private GameObject _multiplayerContent;
        [Tooltip("the hourglass & loading text for the multiplayer text")] 
        [SerializeField] private GameObject _loadingMultiplayerContent;
        [Tooltip("Option Window")]
        [SerializeField] GameObject[] _optionWindow;

        [Header("Title")]
        [Tooltip("Title Game Objects")]
        [SerializeField] private GameObject[] _title;

        [Header("Settings in the Option Window")] 
        [Tooltip("the audio parent of the settings")]
        [SerializeField] private GameObject[] _audioSettings;
        [Tooltip("the graphic parent of the settings")] 
        [SerializeField] private GameObject[] _graphicSettings;
        [Tooltip("the Controls parent of the settings")]
        [SerializeField] private GameObject[] _controlsSetting;
        [Tooltip("the game parent of the settings")]
        [SerializeField] private GameObject[] _gameSettings;

        [Header("Cursor Settings")]
        [Tooltip("the Texture of the Cursor")]
        public Texture2D[] CursorTexture;


        void Start()
        {
            Cursor.SetCursor(CursorTexture[0], Vector2.down, CursorMode.Auto);
            _animator = FindObjectsOfType<Animator>();

            for (int i = 0; i < _playModesButtons.Length; i++)
            {
                _playModesButtons[i].SetActive(false);
            }
        }

        void Update()
        {

            GoBack();
        }

        /// <summary>
        /// Fade Out the Game Objects
        /// </summary>
        /// <param name="fadeObject"></param>
        /// <returns></returns>
        public IEnumerator FadeOut(GameObject fadeObject)
        {
            
            fadeObject.GetComponent<Animator>().SetTrigger("FadeOut");
            yield return new WaitForSeconds(0.15f);
            fadeObject.SetActive(false);
        }

        /// <summary>
        /// Fade In the Game Objects
        /// </summary>
        /// <param name="fadeObject"></param>
        /// <returns></returns>
        public IEnumerator FadeIn(GameObject fadeObject)
        {
            yield return new WaitForSeconds(0.15f);
            fadeObject.SetActive(true);
            fadeObject.GetComponent<Animator>().SetTrigger("FadeIn");
        }
        
        /// <summary>
        /// quit the game
        /// </summary>
        public void QuitGame()
        {
            Application.Quit();
        }

        /// <summary>
        /// Press Escape to go back to the Title Screen
        /// </summary>
        private void GoBack()
        {
            if(Input.GetKey(KeyCode.Escape))
            {
                for (int i = 0; i < _playModesButtons.Length; i++)
                {
                    StartCoroutine(FadeOut(_playModesButtons[i]));
                }

                for (int i = 0; i < _multiplayerWindow.Length; i++)
                {
                    StartCoroutine(FadeOut(_multiplayerWindow[i]));
                }

                for (int i = 0; i < _optionWindow.Length; i++)
                {
                    StartCoroutine(FadeOut(_optionWindow[i]));
                }

                for (int i = 0; i < _extrasButtons.Length; i++)
                {
                    StartCoroutine(FadeOut(_extrasButtons[i]));
                }

                for (int i = 0; i < _singleplayerButtons.Length; i++)
                {
                    StartCoroutine(FadeOut(_singleplayerButtons[i]));
                }

                for (int i = 0; i < _audioSettings.Length; i++)
                {
                    StartCoroutine(FadeOut(_audioSettings[i]));
                }

                for (int i = 0; i < _graphicSettings.Length; i++)
                {
                    StartCoroutine(FadeOut(_graphicSettings[i]));
                }

                for (int i = 0; i < _gameSettings.Length; i++)
                {
                    StartCoroutine(FadeOut(_gameSettings[i]));
                }

                for (int i = 0; i < _controlsSetting.Length; i++)
                {
                    StartCoroutine(FadeOut(_controlsSetting[i]));
                }

                StartCoroutine(FadeOut(_multiplayerContent));
                StartCoroutine(FadeOut(_loadingMultiplayerContent));
                
                for (int i = 0; i < _menuButtons.Length; i++)
                {
                    StartCoroutine(FadeIn(_menuButtons[i]));
                }

                for (int i = 0; i < _title.Length; i++)
                {
                    StartCoroutine(FadeIn(_title[i]));
                }
            }
        }

        /// <summary>
        /// fade in the Play Modes Buttons and fade out the Start Menu 
        /// </summary>
        public void StartButton()
        {
            for (int i = 0; i < _menuButtons.Length; i++)
            {
                StartCoroutine(FadeOut(_menuButtons[i]));
            }

            for (int i = 0; i < _playModesButtons.Length; i++)
            {
                StartCoroutine(FadeIn(_playModesButtons[i]));              
            }

        }

        /// <summary>
        /// fade in the Option Menu and fade out the title and menu Buttons
        /// </summary>
        public void OptionButton()
        {
            for (int i = 0; i < _title.Length; i++)
            {
                StartCoroutine(FadeOut(_title[i]));
            }

            for (int i = 0; i < _menuButtons.Length; i++)
            {
                StartCoroutine(FadeOut(_menuButtons[i]));
            }

            for (int i = 0; i < _optionWindow.Length; i++)
            {
                StartCoroutine(FadeIn(_optionWindow[i]));
            }

            for (int i = 0; i < _audioSettings.Length; i++)
            {
                StartCoroutine(FadeIn(_audioSettings[i]));
            }
        }
        
        /// <summary>
        /// fade in the extras buttons and fade out the menu buttons
        /// </summary>
        public void ExtrasButton()
        {
            for (int i = 0; i < _menuButtons.Length; i++)
            {
                StartCoroutine(FadeOut(_menuButtons[i]));
            }

            for (int i = 0; i < _extrasButtons.Length; i++)
            {
                StartCoroutine(FadeIn(_extrasButtons[i]));
            }

        }

        /// <summary>
        /// fade in the Singleplayer buttons and fade out the Play Modes buttons
        /// </summary>
        public void SingleplayerButtons()
        {
            for (int i = 0; i < _playModesButtons.Length; i++)
            {
                StartCoroutine(FadeOut(_playModesButtons[i]));
            }

            for (int i = 0; i < _singleplayerButtons.Length; i++)
            {
                StartCoroutine(FadeIn(_singleplayerButtons[i]));
            }
        }

        public void GraphicSettings()
        {
            for (int i = 0; i < _controlsSetting.Length; i++)
            {
                StartCoroutine(FadeOut(_controlsSetting[i]));
            }

            for (int i = 0; i < _gameSettings.Length; i++)
            {
                StartCoroutine(FadeOut(_gameSettings[i]));
            }

            for (int i = 0; i < _audioSettings.Length; i++)
            {
                StartCoroutine(FadeOut(_audioSettings[i]));
            }

            for (int i = 0; i < _graphicSettings.Length; i++)
            {
                StartCoroutine(FadeIn(_graphicSettings[i]));
            }
        }

        public void AudioSettings()
        {
            for (int i = 0; i < _controlsSetting.Length; i++)
            {
                StartCoroutine(FadeOut(_controlsSetting[i]));
            }

            for (int i = 0; i < _gameSettings.Length; i++)
            {
                StartCoroutine(FadeOut(_gameSettings[i]));
            }

            for (int i = 0; i < _graphicSettings.Length; i++)
            {
                StartCoroutine(FadeOut(_graphicSettings[i]));
            }

            for (int i = 0; i < _audioSettings.Length; i++)
            {
                StartCoroutine(FadeIn(_audioSettings[i]));
            }
        }

        public void GameSettings()
        {
            for (int i = 0; i < _controlsSetting.Length; i++)
            {
                StartCoroutine(FadeOut(_controlsSetting[i]));
            }

            for (int i = 0; i < _audioSettings.Length; i++)
            {
                StartCoroutine(FadeOut(_audioSettings[i]));
            }

            for (int i = 0; i < _graphicSettings.Length; i++)
            {
                StartCoroutine(FadeOut(_graphicSettings[i]));
            }

            for (int i = 0; i < _gameSettings.Length; i++)
            {
                StartCoroutine(FadeIn(_gameSettings[i]));
            }
        }

        public void ControlsSettings()
        {
            for (int i = 0; i < _gameSettings.Length; i++)
            {
                StartCoroutine(FadeOut(_gameSettings[i]));
            }

            for (int i = 0; i < _audioSettings.Length; i++)
            {
                StartCoroutine(FadeOut(_audioSettings[i]));
            }

            for (int i = 0; i < _graphicSettings.Length; i++)
            {
                StartCoroutine(FadeOut(_graphicSettings[i]));
            }

            for (int i = 0; i < _controlsSetting.Length; i++)
            {
                StartCoroutine(FadeIn(_controlsSetting[i]));
            }
        }

    }
}


