using UnityEngine;
using System.Collections;

public class ImageAnimation : MonoBehaviour
{

    [SerializeField] private GameObject[] _images;
    [SerializeField] private float _secondsForNextFrame;

    void OnEnable() 
    {
        StartCoroutine(LoopImages());
    }

    IEnumerator LoopImages()
    {
        _images[0].SetActive(true);
        _images[1].SetActive(false);
        yield return new WaitForSeconds(_secondsForNextFrame);
        _images[0].SetActive(false);
        _images[1].SetActive(true);
        yield return new WaitForSeconds(_secondsForNextFrame);
        StartCoroutine(LoopImages());
    }
}
