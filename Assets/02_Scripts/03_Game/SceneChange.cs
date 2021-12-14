using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Enemy.SoundItem;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using untitledProject;

public class SceneChange : MonoBehaviour
{
    [Tooltip("the image that will fill the screen")]
    [SerializeField] private Image _fadeImage;
    [Tooltip("the text which will appears at completed fade in")]
    [SerializeField] private TextMeshProUGUI _text;
    [Tooltip("the time how long the text will fade in and out")]
    [SerializeField] private float _textFadeTime;
    [Tooltip("the time how long the fade image needs to fade in and out")]
    [SerializeField] private float _fadeTime;
    [Tooltip("the time how long the text should appears when the fade in is completed")]
    [SerializeField] private float _fadeStayTime;
    [Tooltip("the next scene name that should be loaded")]
    [SerializeField] private string _nextSceneName;
    private float _fadeStayCooldown = 0;
    [Tooltip("skip the text")]
    [SerializeField] private GameObject _skipButton;

    private PlayerController _playerController;
    private GameObject _questManager;
    private GameObject _stoneUI;

    private CollectStones _collectStones;
    
    private NoisyItem[] _noisyItems;
    
    void Start()
    {
        _fadeImage.fillAmount = 0;
        _fadeStayCooldown = _fadeStayTime;

        _playerController = FindObjectOfType<PlayerController>();
        _questManager = GameObject.Find("QuestManager");
        _stoneUI = GameObject.Find("StoneUI");

        _collectStones = FindObjectOfType<CollectStones>();
        
        _skipButton.SetActive(false);

        _noisyItems = FindObjectsOfType<NoisyItem>();
    }
    
    void Update()
    {
        bool _completeFadeIn = _fadeImage.color.a >= 0.99f;
        if (_completeFadeIn)
        {
            _skipButton.SetActive(true);
            
            if (_fadeStayCooldown <= 0)
            {
                _text.DOFade(0, _textFadeTime);

                if (_text.color.a <= 0.001f)
                {
                    PlayerPrefs.DeleteKey("PlayerPositionX");
                    PlayerPrefs.DeleteKey("PlayerPositionY");
                    PlayerPrefs.DeleteKey("PlayerPositionZ");
                    PlayerPrefs.SetInt("StonesAmount", _collectStones.StonesCounter);
                    for (int i = 0; i < _noisyItems.Length; i++)
                    {
                        _noisyItems[i].SafeState = true;
                    }
                    
                    PlayerPrefs.Save();
                    SceneManager.LoadScene(_nextSceneName);
                }
            }
            else
            {
                _fadeStayCooldown -= Time.deltaTime;
                _text.DOFade(1, _textFadeTime);
            }
        }
    }

    public void ChangeScene()
    {
        _playerController.PlayerAnimationHandler.SetSpeeds(0,0);
        _playerController.enabled = false;
        // Deactivate QuestManager parent object, this is a temporary solution so it doesn't overlap with the narrative text
        _questManager.SetActive(false);
        _stoneUI.SetActive(false);
        _fadeImage.DOFade(1, _fadeTime);
    }

    public void Skip()
    {
        _fadeStayCooldown = 0;
    }
}
