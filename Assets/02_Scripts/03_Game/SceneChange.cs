using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    
    void Start()
    {
        _fadeImage.fillAmount = 0;
        _fadeStayCooldown = _fadeStayTime;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ChangeScene();
        }
        
        bool _completeFadeIn = _fadeImage.color.a >= 0.99f;
        if (_completeFadeIn)
        {
            if (_fadeStayCooldown <= 0)
            {
                _text.DOFade(0, _textFadeTime);

                if (_text.color.a <= 0)
                {
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
        _fadeImage.DOFade(1, _fadeTime);
    }
}
