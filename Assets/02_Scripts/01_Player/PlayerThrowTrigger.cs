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

    [SerializeField] private Transform _lastThrowPositionObject;

    private CollectStones _collectStones;
    
    private bool _playerThrew = false;

    public bool PlayerThrew
    {
        get => _playerThrew;
        set => _playerThrew = value;
    }
    
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
    
    private NoisyItem noisyItem;
    
    public NoisyItem NoisyItem
    {
        get => noisyItem;
        set => noisyItem = value;
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

    // is for the enemy to know where to search after investigation the noisy item
    private Transform _throwPosition;

    public Transform ThrowPosition
    {
        get => _throwPosition;
        set => _throwPosition = value;
    }

    Ray ray;

    private void Start()
    {
        _collectStones = FindObjectOfType<CollectStones>();
        
        // just find a random sound item and new GameObject to not be null. Otherwise, there will be errors
        // the randomness and new GameObject creation doesn't matter, because when the player enters the trigger, it will be updated and can only be used in the trigger
        noisyItem = FindObjectOfType<NoisyItem>();
        _usableMarker = new GameObject();
    }

    private void Update()
    {
        if (noisyItem != null && !noisyItem.ItemUsed)
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
            _throwableText.gameObject.SetActive(false);
            _notThrowableText.gameObject.SetActive(false);
            _usableMarker.SetActive(false);
        }
        
        if (!_close)
        {
            Debug.DrawRay(_inWayRaycastPosition.position, noisyItem.transform.position - transform.position * Vector3.Distance(noisyItem.transform.position, transform.position));
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            Physics.Raycast(_inWayRaycastPosition.position, noisyItem.transform.position - transform.position, out hit, Vector3.Distance(noisyItem.transform.position, transform.position));
            
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
                                _throwPosition = Instantiate(_lastThrowPositionObject, transform.position, Quaternion.identity);
                                _playerThrew = true;
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
            noisyItem = other.GetComponent<NoisyItem>();

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
