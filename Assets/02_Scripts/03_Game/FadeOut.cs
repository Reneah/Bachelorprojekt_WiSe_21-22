using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    [Tooltip("the image that will fill the screen")]
    [SerializeField] private Image _fadeImage;
    [Tooltip("the time how long the fade image needs to fade in and out")]
    [SerializeField] private float _fadeTime;
    
    void Start()
    {
        Color _color = _fadeImage.color;
        _color.a = 1;
        _fadeImage.DOFade(0, _fadeTime);
    }
}
