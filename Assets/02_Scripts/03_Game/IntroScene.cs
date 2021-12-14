using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScene : MonoBehaviour
{
    [Tooltip("the text which will appears at completed fade in")]
    [SerializeField] private TextMeshProUGUI _text;
    [Tooltip("the time how long the text will fade in and out")]
    [SerializeField] private float _textFadeTime;
    [Tooltip("the time how long the text should appears when the fade in is completed")]
    [SerializeField] private float _fadeStayTime;
    [Tooltip("the next scene name that should be loaded")]
    [SerializeField] private string _nextSceneName;
    private float _fadeStayCooldown = 0;

    private bool _activateFade = true;
    
    void Start()
    {
        _text.DOFade(1, _textFadeTime);
        _fadeStayCooldown = _fadeStayTime;
    }

    void Update()
    {
        if (_fadeStayCooldown < 0)
        {
            if (_activateFade)
            {
                //_textColor = 1;
                _text.DOFade(0, _textFadeTime);
                _activateFade = false;
            }
            
            if (_text.color.a <= 0.001f)
            {
                //_textColor = 0;
                SceneManager.LoadScene(_nextSceneName);
            }
        }
        else
        {
            _fadeStayCooldown -= Time.deltaTime;
        }
    }

    public void SkipIntro()
    {
        _fadeStayCooldown = 0;
    }
}