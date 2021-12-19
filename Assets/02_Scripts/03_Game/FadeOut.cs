using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using untitledProject;

public class FadeOut : MonoBehaviour
{
    [Tooltip("the image that will fill the screen")]
    [SerializeField] private Image _fadeImage;
    [Tooltip("the time how long the fade image needs to fade in and out")]
    [SerializeField] private float _fadeTime;

    [SerializeField] private GameObject _tutorial;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private bool firstSceneFadeOut;

    private float _movementSpeed;
    private float _smoothRotation;
    private bool fadingOut = true;

    void Start()
    {
        if (firstSceneFadeOut)
        {
            _tutorial.SetActive(false);
            _movementSpeed = _playerController.MovementSpeed;
            _smoothRotation = _playerController.SmoothRotation;
            _playerController.MovementSpeed = 0;
            _playerController.SmoothRotation = 0;
        }
        Color _color = _fadeImage.color;
        _color.a = 1;
        _fadeImage.DOFade(0, _fadeTime);
    }

    private void Update()
    {
        if (fadingOut && firstSceneFadeOut)
        {
            if (_fadeImage.color.a <= 0.1)
            {
                EnableTutorial();
                fadingOut = false;
            }
        }
    }

    public void EnableTutorial()
    {
        _tutorial.SetActive(true);
        _playerController.enabled = true;
        _playerController.MovementSpeed = _movementSpeed;
        _playerController.SmoothRotation = _smoothRotation;
    }
}
