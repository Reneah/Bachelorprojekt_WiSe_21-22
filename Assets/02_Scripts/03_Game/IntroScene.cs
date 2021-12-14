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
    private float _textColor;


    void Start()
    {
        _fadeStayCooldown = _fadeStayTime;
        _textColor = _text.color.a;
    }

    void Update()
    {
        if (_fadeStayCooldown <= 0)
        {
            //_textColor = 1;
            _text.DOFade(0, _textFadeTime);
            
            if (_textColor <= 0.001f)
            {
                //_textColor = 0;
                SceneManager.LoadScene(_nextSceneName);
            }
        }
        else
        {
            _fadeStayCooldown -= Time.deltaTime;
            _text.DOFade(1, _textFadeTime);
        }
    }

    public void SkipIntro()
    {
        SceneManager.LoadScene(_nextSceneName);
    }
}