using BP;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroScene : MonoBehaviour
{
    [Tooltip("the text which will appears at completed fade in")]
    [SerializeField] private TextMeshProUGUI _text;
    [Tooltip("the time how long the text will fade in and out")]
    [SerializeField] private float _textFadeTime;
    //[Tooltip("the time how long the text should appears when the fade in is completed")]
    //[SerializeField] private float _fadeStayTime;
    [Tooltip("the next scene name that should be loaded")]
    [SerializeField] private string _nextSceneName;
    private float _fadeStayCooldown = 0;

    private bool _activateFade = true;

    [SerializeField] private IntroText[] IntroTexts;
    private int currentIntroText = 0;

    [SerializeField] private Image _fadingOverlay;
    
    void Start()
    {
        _text.text = IntroTexts[currentIntroText].text;
        _text.DOFade(1, _textFadeTime);
        _fadeStayCooldown = IntroTexts[currentIntroText].fadeStayTime;
    }

    void Update()
    {
        if (_fadeStayCooldown < 0)
        {
            if (_activateFade)
            {
                //_textColor = 1;
                _text.DOFade(0, _textFadeTime).OnComplete(CheckNextStep);
                _activateFade = false;
            }
            
        }
        else
        {
            _fadeStayCooldown -= Time.deltaTime;
        }
    }

    public void FadeInNextIntroText()
    {
        currentIntroText++;
        _activateFade = true;
        _text.text = IntroTexts[currentIntroText].text;
        _text.DOFade(1, _textFadeTime);
        _fadeStayCooldown = IntroTexts[currentIntroText].fadeStayTime;
    }

    public void CheckNextStep()
    {
        //_textColor = 0;
        if (currentIntroText >= IntroTexts.Length-1)
        {
            LoadNextScene();
        }
        else
        {
            FadeInNextIntroText();
        }
    }

    public void SkipCurrentIntroText()
    {
        _fadeStayCooldown = 0;
    }
    
    public void SkipEverything()
    {
        _fadingOverlay.DOFade(1, _textFadeTime).OnComplete(LoadNextScene);
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(_nextSceneName);
    }
}