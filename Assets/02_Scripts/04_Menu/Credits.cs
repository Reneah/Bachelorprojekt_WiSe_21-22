using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    [Header("Credits Settings")]
    [Tooltip("The GameObject for our credits texts")] [SerializeField]
    private GameObject _myCredits;
    [Tooltip("The Canvas background image")] [SerializeField]
    private RawImage _myCreditsFadeImage;
    [Tooltip("The time it takes to fade out the background image")] [SerializeField]
    private float _fadeImageFadeDuration;
    [Tooltip("The scene to be loaded after the credits have played")] [SerializeField]
    private string _sceneToLoad;

    private float _timer;
    private float _imageAlpha;

    private GameObject _scenePersistentGameObjects;
    

    // Get the necessary references
    private void Awake()
    {
        _scenePersistentGameObjects = GameObject.Find("ScenePersistent");
        
        _imageAlpha = _myCreditsFadeImage.GetComponent<RawImage>().color.a;
        
        // Activates credits GO on scene start, if it was deactivated
        if (!_myCredits.activeInHierarchy)
        {
            _myCredits.SetActive(true);
        }
        
        // Resets background image alpha, if it not at full value
        if (_imageAlpha < 1)
        {
            _imageAlpha = 1f;
        }
    }

    private void Start()
    {
        _myCreditsFadeImage.DOFade(0, _fadeImageFadeDuration * 4);
    }

    private void FadeInBackgroundImage()
    {
        _myCreditsFadeImage.DOFade(1, _fadeImageFadeDuration).OnComplete(LoadScene);
    }

    public void ReturnToMainMenu()
    {
        FadeInBackgroundImage();
    }

    private void LoadScene()
    {
        //Destroy(_scenePersistentGameObjects);
        SceneManager.LoadScene(_sceneToLoad);
    }
}
