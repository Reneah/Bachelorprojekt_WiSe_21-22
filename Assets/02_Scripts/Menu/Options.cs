using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;
using Toggle = UnityEngine.UI.Toggle;

namespace DA.Menu
{
    public class Options : MonoBehaviour
    {
        [Header("AudioSettings")]
        [Tooltip("The Music Audio Mixer")]
        [SerializeField] private AudioMixer _audioMixer;

        public AudioMixer AudioMixer
        {
            get => _audioMixer;
            set => _audioMixer = value;
        }

        // all resolutions of the PC
        private List<Resolution> _resolutions = new List<Resolution>();

        [Tooltip("The resolution Dropdown")]
        [SerializeField] private TMP_Dropdown _resolutionDropdown;

        [Tooltip("The graphics Dropdown")]
        [SerializeField] private TMP_Dropdown _graphicsDropdown;

        [Tooltip("The framerate Dropdown")]
        [SerializeField] private TMP_Dropdown _framerateDropdown;

        [Tooltip("Framerate object")]
        [SerializeField]
        private GameObject _framerate;

        [Tooltip("the framerate Toggle")]
        [SerializeField]
        private Toggle _framerateToggle;

        [Tooltip("the V-Sync Toggle")]
        [SerializeField]
        private Toggle _vSyncToggle;

        [SerializeField] private Toggle _fullScreen;
        // determines which resolution is set
        int currentResolutionIndex = 0;

        // the list to get all resolutions 
        List<string> _resolutionOptions = new List<string>();
        
        [SerializeField] private Slider _musicSlider;

        public Slider MusicSlider
        {
            get => _musicSlider;
            set => _musicSlider = value;
        }

        private ScenePersistent _scenePersistent;

        private void Start()
        {
            _scenePersistent = FindObjectOfType<ScenePersistent>();
            _scenePersistent.Loaded = true;
            // deactivates the V-Sync
            QualitySettings.vSyncCount = 0;
            // because the V-Sync is deactivated the toggle has to be on false
            _vSyncToggle.isOn = false;

            // set the framerate at the start of the game
            Application.targetFrameRate = 60;
            // set the right framerate Display at the Dropdown menu
            _framerateDropdown.value = 1;
            // deactivates the framerate object
            _framerate.SetActive(false);
            // because the framerate label is deactivated the toggle has to be on false
            _framerateToggle.isOn = false;

            // set the quality level in the beginning
            QualitySettings.SetQualityLevel(2);
            // get the current graphic settings and show that in the dropdown 
            _graphicsDropdown.value = QualitySettings.GetQualityLevel();

            Resolution();
        }

        private void Resolution()
        {
            // get all resolution with 60 Hz to stop showing every framerate in the Menu
            // NOTE: the framerate can be changed at the framerate menu, that is the reason why I am doing this, otherwise i have multiple times the same resolution in the menu
            for (int i = 0; i < Screen.resolutions.Length; i++)
            {
                if (Screen.resolutions[i].refreshRate == 60)
                {
                    _resolutions.Add(Screen.resolutions[i]);
                }
            }

            // clear all resolutions of the dropdown to ensure that no custom text of the inspector is in there
            _resolutionDropdown.ClearOptions();

            //loop through all resolutions to get the width and height and add that to the list to choose between them later
            for (int i = 0; i < _resolutions.Count; i++)
            {
                // get all resolution of width and height
                string option = _resolutions[i].width + " x " + _resolutions[i].height;

                // add the resolutions to the list
                _resolutionOptions.Add(option);

                // if the current screen resolution is found, set the value 
                if (_resolutions[i].width == Screen.currentResolution.width && _resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }

            // add the found resolutions to the Dropdown menu
            _resolutionDropdown.AddOptions(_resolutionOptions);
            // show the current Resolution at the Dropdown menu
            _resolutionDropdown.value = currentResolutionIndex;
            // refresh the shown values to get the new resolution 
            _resolutionDropdown.RefreshShownValue();
        }

        /// <summary>
        /// Set the volume of the music
        /// </summary>
        /// <param name="volume"></param>
        public void SetMusicVolume(float volume)
        {
            ScenePersistent.MusicVolume = _musicSlider.value;
            _audioMixer.SetFloat("volume", _musicSlider.value);
        }

        /// <summary>
        /// set the Graphic Quality of the game
        /// </summary>
        /// <param name="qualityIndex"></param>
        public void SetQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(_graphicsDropdown.value);

            if (_vSyncToggle.isOn)
            {
                QualitySettings.vSyncCount = 1;
            }
            else
            {
                QualitySettings.vSyncCount = 0;
            }

        }

        /// <summary>
        /// decide to set it to fullscreen or not
        /// </summary>
        /// <param name="isFullScreen"></param>
        public void SetFullscreen(bool isFullScreen)
        {
            Screen.fullScreen = _fullScreen.isOn;
        }

        /// <summary>
        /// set the current resolution of the screen
        /// </summary>
        /// <param name="resolutionIndex"></param>
        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = _resolutions[_resolutionDropdown.value];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        /// <summary>
        /// activates or deactivated the framerate
        /// </summary>
        /// <param name="showFrameRate"></param>
        public void ShowFramerate()
        {
            _framerate.SetActive(_framerateToggle.isOn);
        }

        public void SetVsync()
        {
            if (_vSyncToggle.isOn)
            {
                QualitySettings.vSyncCount = 1;
            }
            else
            {
                QualitySettings.vSyncCount = 0;
            }

        }

        /// <summary>
        /// set the defined framerates
        /// </summary>
        /// <param name="framerate"></param>
        public void SetFramerate(int framerate)
        {
            switch (_framerateDropdown.value)
            {
                case 0:
                    Application.targetFrameRate = 30;
                    break;
                case 1:
                    Application.targetFrameRate = 60;
                    break;
                case 2:
                    Application.targetFrameRate = 75;
                    break;
                case 3:
                    Application.targetFrameRate = 120;
                    break;
                case 4:
                    Application.targetFrameRate = 144;
                    break;
                case 5:
                    Application.targetFrameRate = 200;
                    break;
                case 6:
                    Application.targetFrameRate = 240;
                    break;
                default:
                    Debug.Log("not set Framerate");
                    break;
            }
        }
    }

}
