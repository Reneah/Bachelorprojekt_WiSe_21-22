using System.Collections;
using System.Collections.Generic;
using DA.Menu;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePersistent : MonoBehaviour
{
    public static float SoundVolume;
    public static float MusicVolume;

    private Scene _loadingscene;

    private bool _loaded;

    public bool Loaded
    {
        get => _loaded;
        set => _loaded = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        MusicVolume = 0;
        SoundVolume = 1;
        
        _loadingscene = SceneManager.GetActiveScene();
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        _loadingscene = SceneManager.GetActiveScene();
        
        if (_loadingscene == SceneManager.GetSceneByName("Play") && _loaded || _loadingscene == SceneManager.GetSceneByName("MainMenu") && _loaded)
        {
            FindObjectOfType<Options>().MusicSlider.value = MusicVolume;
            FindObjectOfType<Options>().AudioMixer.SetFloat("volume", MusicVolume);
            FindObjectOfType<InGameMenu>().SoundSlider.value = SoundVolume;
            _loaded = false;
        }
    }
}
