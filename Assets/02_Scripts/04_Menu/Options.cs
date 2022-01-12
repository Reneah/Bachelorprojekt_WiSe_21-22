using System.Collections.Generic;
using DarkTonic.MasterAudio;
using UnityEngine;
using TMPro;
using Slider = UnityEngine.UI.Slider;
using Toggle = UnityEngine.UI.Toggle;

    public class Options : MonoBehaviour
    {
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
        
        [SerializeField] private Slider _soundSlider;
        
        private ScenePersistent _scenePersistent;

        private SetOptions setOptions;

        private void Start()
        {
            setOptions = FindObjectOfType<SetOptions>();

            Screen.fullScreen = setOptions.ScreenStatus;
            _fullScreen.isOn = setOptions.ScreenStatus;
            
            _soundSlider.value = setOptions.SoundVolume;
            ChangeSoundVolume();

            _musicSlider.value = setOptions.MusicVolume;
            Debug.Log(_musicSlider.value);
            SetMusicVolume();
            
            _framerateDropdown.value = setOptions.Framerate;
            _framerateToggle.isOn = setOptions.FramerateToogle;
            SetFramerate();

            QualitySettings.vSyncCount = setOptions.VSync;
            _vSyncToggle.isOn = setOptions.VSyncToggle;
            
            QualitySettings.SetQualityLevel(setOptions.Quality);
            _graphicsDropdown.value = setOptions.Quality;
            SetQuality();
            
            _framerate.SetActive(setOptions.FramerateToogle);
            
            Resolution();
            
            Screen.SetResolution(setOptions.ScreenResolution.width, setOptions.ScreenResolution.height, Screen.fullScreen);
            _resolutionDropdown.value = setOptions.ResolutionValue;
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
        public void SetMusicVolume()
        {
            MasterAudio.PlaylistMasterVolume = _musicSlider.value;
            
            setOptions.MusicVolume = MasterAudio.PlaylistMasterVolume;
        }

        /// <summary>
        /// set the Graphic Quality of the game
        /// </summary>
        /// <param name="qualityIndex"></param>
        public void SetQuality()
        {
            QualitySettings.SetQualityLevel(_graphicsDropdown.value);
            setOptions.Quality = _graphicsDropdown.value;
            
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
        public void SetFullscreen()
        {
            Screen.fullScreen = _fullScreen.isOn;
            setOptions.ScreenStatus = _fullScreen.isOn;
        }

        /// <summary>
        /// set the current resolution of the screen
        /// </summary>
        /// <param name="resolutionIndex"></param>
        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = _resolutions[_resolutionDropdown.value];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            
            setOptions.ResolutionValue = _resolutionDropdown.value;
            setOptions.ScreenResolution = resolution;
        }

        /// <summary>
        /// activates or deactivated the framerate
        /// </summary>
        /// <param name="showFrameRate"></param>
        public void ShowFramerate()
        {
            _framerate.SetActive(_framerateToggle.isOn);
            
            setOptions.FramerateToogle = _framerateToggle.isOn;
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

            setOptions.VSyncToggle = _vSyncToggle.isOn;

        }
        
        public void ChangeSoundVolume()
        {
            MasterAudio.MasterVolumeLevel = _soundSlider.value;

            setOptions.SoundVolume = MasterAudio.MasterVolumeLevel;
        }

        /// <summary>
        /// set the defined framerates
        /// </summary>
        /// <param name="framerate"></param>
        public void SetFramerate()
        {
            setOptions.Framerate = _framerateDropdown.value;
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
