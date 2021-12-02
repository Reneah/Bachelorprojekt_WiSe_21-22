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
        MusicVolume = 1;
        SoundVolume = 1;
        
        _loadingscene = SceneManager.GetActiveScene();
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        _loadingscene = SceneManager.GetActiveScene();
        
        if (_loadingscene == SceneManager.GetSceneByName("GameWorld_BesiegedKeep_1") && _loaded || _loadingscene == SceneManager.GetSceneByName("MainMenu") && _loaded
            || _loadingscene == SceneManager.GetSceneByName("GameWorld_BesiegedKeep_2") && _loaded || _loadingscene == SceneManager.GetSceneByName("GameWorld_BesiegedKeep_3") && _loaded)
        {
            FindObjectOfType<Options>().MusicSlider.value = MusicVolume;
            FindObjectOfType<Options>().AudioMixer.SetFloat("Volume", MusicVolume);
            FindObjectOfType<Options>().SoundSlider.value = SoundVolume;
            _loaded = false;
        }
    }
}
