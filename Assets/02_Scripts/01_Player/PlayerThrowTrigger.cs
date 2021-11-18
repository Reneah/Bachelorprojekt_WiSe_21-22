using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using untitledProject;

public class PlayerThrowTrigger : MonoBehaviour
{
    [Header("Throwing Rocks")]
    [Tooltip("shows text if the player can throw the rock towards the noisy item")]
    [SerializeField] private TextMeshProUGUI _throwableText;
    [Tooltip("shows text if the player can not throw the rock towards the noisy item")]
    [SerializeField] private TextMeshProUGUI _notThrowableText;
    [Tooltip("the origin of the raycast to signalize the obstacles in the trajectory")] 
    [SerializeField] private Transform _inWayRaycastPosition;
    [Tooltip("the name of the marker to show the current usable noisy item ")]
    [SerializeField] private string _usableMarkerName;
    [Tooltip("modify the text position at the mouse position")]
    [SerializeField] private Vector2 _textOffset;
    [Tooltip("the rotation speed towards the throw target")]
    [SerializeField] private float _rotationSpeed;
    [Tooltip("wait to throw during rotation towards the target")]
    [SerializeField] public float _waitToThrowDuringRotation;

    private CollectStones _collectStones;
    
    public float RotationSpeed
    {
        get => _rotationSpeed;
        set => _rotationSpeed = value;
    }

    public float WaitToThrowDuringRotation
    {
        get => _waitToThrowDuringRotation;
        set => _waitToThrowDuringRotation = value;
    }

    // signalize that the noisy item can be activated
    private GameObject _usableMarker;
    
    private SoundItem _soundItem;
    
    public SoundItem SoundItem
    {
        get => _soundItem;
        set => _soundItem = value;
    }
    
    // the player is able to throw
    private bool _throwAvailable = false;
    
    // shows if the throw state is activated or not
    private bool _throwstate;

    public bool Throwstate
    {
        get => _throwstate;
        set => _throwstate = value;
    }

    // if the player is near the noisy item, he is able to activated it per hand
    private bool _close = false;

    public bool Close
    {
        get => _close;
        set => _close = value;
    }

    Ray ray;

    private void Start()
    {
        _collectStones = FindObjectOfType<CollectStones>();
        
        // just find a random sound item and new GameObject to not be null. Otherwise, there will be errors
        // the randomness and new GameObject creation doesn't matter, because when the player enters the trigger, it will be updated and can only be used in the trigger
        _soundItem = FindObjectOfType<SoundItem>();
        _usableMarker = new GameObject();
    }

    private void Update()
    {
        if (_soundItem != null && !_soundItem.ItemUsed)
        {
            Throw();
        }
        else
        {
            _throwableText.gameObject.SetActive(false);
            _notThrowableText.gameObject.SetActive(false);
            _usableMarker.SetActive(false);
        }
    }

    private void Throw()
    {
        // update the UI position all the time
        _throwableText.transform.position =  new Vector3(_textOffset.x, _textOffset.y, 0) + Input.mousePosition;
        _notThrowableText.transform.position = new Vector3(_textOffset.x, _textOffset.y, 0) + Input.mousePosition;
        
        // if the player is near the noisy item, he is able to activate it per hand and doesn't need to throw
        if (_close)
        {
            _usableMarker.SetActive(true);
            _throwableText.gameObject.SetActive(false);
            _notThrowableText.gameObject.SetActive(false);
        }
        
        if (!_close)
        {
            Debug.DrawRay(_inWayRaycastPosition.position, _soundItem.transform.position - transform.position * Vector3.Distance(_soundItem.transform.position, transform.position));
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            Physics.Raycast(_inWayRaycastPosition.position, _soundItem.transform.position - transform.position, out hit, Vector3.Distance(_soundItem.transform.position, transform.position));
            
                // if the mouse is hovering over the noisy item, the corresponding text will show up and the throw is available
                if(Physics.Raycast(ray, Mathf.Infinity, LayerMask.GetMask("NoisyItem")))
                {
                    _notThrowableText.gameObject.SetActive(true);
                    
                    if (_throwAvailable && _collectStones.StonesCounter > 0)
                    {
                        _usableMarker.SetActive(true);
                        
                        // if something is blocking the trajectory
                        if (hit.collider.CompareTag("Wall"))
                        {
                            _throwableText.gameObject.SetActive(false);
                            _notThrowableText.gameObject.SetActive(true);
                        }
                        else
                        {
                            _throwableText.gameObject.SetActive(true);
                            _notThrowableText.gameObject.SetActive(false);

                            // this should be actually in the state so that it can't be used again when the item can be used again
                            if (Input.GetMouseButtonDown(0))
                            {
                                _usableMarker.SetActive(false);
                                _throwstate = true;
                                _throwableText.gameObject.SetActive(false);
                                _throwAvailable = false;
                            }
                        } 
                    }
                }
                else
                {
                    _throwableText.gameObject.SetActive(false);
                    _notThrowableText.gameObject.SetActive(false);
                    _usableMarker.SetActive(false);
                }
        }
    }
    
    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("NoisyItem") && !_throwstate && !_close)
        {
            _soundItem = other.GetComponent<SoundItem>();

            foreach (Transform child in other.transform)
            {
                if (child.name == _usableMarkerName)
                {
                    _usableMarker = child.gameObject;
                }
            }
            
            _throwAvailable = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NoisyItem"))
        {
            _throwableText.gameObject.SetActive(false);
            _notThrowableText.gameObject.SetActive(false);
            _usableMarker.SetActive(false);
            _throwAvailable = false;
        }
    }
}
