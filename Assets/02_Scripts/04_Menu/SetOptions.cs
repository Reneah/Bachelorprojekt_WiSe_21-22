using DarkTonic.MasterAudio;
using UnityEngine;

    public class SetOptions : MonoBehaviour
    {
        private bool _screenStatus;
        private float _soundVolume;
        private float _musicVolume;
        private int _framerate;
        private bool _framerateToggle;
        private int _vSync;
        private bool _vSyncToggle;
        private int _quality;
        private Resolution _screenResolution;
        private int _resolutionValue;

        public int ResolutionValue
        {
            get => _resolutionValue;
            set => _resolutionValue = value;
        }

        public bool ScreenStatus
        {
            get => _screenStatus;
            set => _screenStatus = value;
        }

        public float SoundVolume
        {
            get => _soundVolume;
            set => _soundVolume = value;
        }

        public float MusicVolume
        {
            get => _musicVolume;
            set => _musicVolume = value;
        }

        public int VSync
        {
            get => _vSync;
            set => _vSync = value;
        }

        public bool VSyncToggle
        {
            get => _vSyncToggle;
            set => _vSyncToggle = value;
        }

        public int Quality
        {
            get => _quality;
            set => _quality = value;
        }

        public Resolution ScreenResolution
        {
            get => _screenResolution;
            set => _screenResolution = value;
        }

        public int Framerate
        {
            get => _framerate;
            set => _framerate = value;
        }

        public bool FramerateToogle
        {
            get => _framerateToggle;
            set => _framerateToggle = value;
        }

        private void Awake()
        {
            SetValues();
        }
        
        public void SetValues()
        {
            _screenStatus = Screen.fullScreen;
            _soundVolume = MasterAudio.MasterVolumeLevel;
            _musicVolume = MasterAudio.PlaylistMasterVolume;
            _vSync = QualitySettings.vSyncCount;
            _quality = QualitySettings.GetQualityLevel();
            _screenResolution = Screen.currentResolution;

            _framerateToggle = false;
            _vSyncToggle = false;
            _resolutionValue = 1;
            _framerate = 1;
        }
    }

