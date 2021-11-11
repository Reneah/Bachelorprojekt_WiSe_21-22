using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerThrowTrigger : MonoBehaviour
{
    [Header("Throwing Rocks")]
    [Tooltip("shows text if the player can throw the rock towards the noisy item")]
    [SerializeField] private TextMeshProUGUI _throwableText;
    [Tooltip("shows text if the player can not throw the rock towards the noisy item")]
    [SerializeField] private TextMeshProUGUI _notThrowableText;
    [Tooltip("the origin of the raycast to signalize the obstacles in the trajectory")] 
    [SerializeField] private Transform _inWayRaycastPosition;
    [Tooltip("signalize the noisy item that can be range activated")]
    [SerializeField] private GameObject _throwableMarker;

    private Transform noiseItemPositionPosition;

    public Transform NoiseItemPosition
    {
        get => noiseItemPositionPosition;
        set => noiseItemPositionPosition = value;
    }

    private bool _throwstate;

    public bool Throwstate
    {
        get => _throwstate;
        set => _throwstate = value;
    }
    
    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("NoisyItem"))
        {
            Debug.DrawRay(_inWayRaycastPosition.position, other.transform.position - transform.position * Vector3.Distance(other.transform.position, transform.position));
            RaycastHit hit;
            if (Physics.Raycast(_inWayRaycastPosition.position, other.transform.position - transform.position, out hit, Vector3.Distance(other.transform.position, transform.position)))
            {
                // if something is blocking the trajectory
                // NOTE: throwable text will be not deactivated during throwing because of the update
                if (hit.collider.CompareTag("Wall"))
                {
                    _throwableText.gameObject.SetActive(false);
                    _throwableMarker.SetActive(false);
                    _notThrowableText.gameObject.SetActive(true);
                    _notThrowableText.transform.position = Input.mousePosition;
                }
                else
                {
                    _throwableText.gameObject.SetActive(true);
                    _throwableMarker.SetActive(true);
                    _notThrowableText.gameObject.SetActive(false);
                    _throwableText.transform.position = Input.mousePosition;

                    // NOTE: it seems to be that stay trigger update is not fast enough to be able to click every time
                    if (Input.GetMouseButtonDown(0))
                    {
                        _throwableMarker.SetActive(false);
                        _throwstate = true;
                        _throwableText.gameObject.SetActive(false);
                        _notThrowableText.gameObject.SetActive(true);
                        noiseItemPositionPosition = other.transform;
                    }
                }
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NoisyItem"))
        {
            _throwableText.gameObject.SetActive(false);
            _notThrowableText.gameObject.SetActive(false);
            _throwableMarker.SetActive(false);
        }
    }
}
