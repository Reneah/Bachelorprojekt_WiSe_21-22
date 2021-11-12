using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


//NOTE: usable marker has to get of every single noisy item
// NOTE: need from the noisy item "itemUsed" to deactivated it
public class PlayerThrowTrigger : MonoBehaviour
{
    [Header("Throwing Rocks")]
    [Tooltip("shows text if the player can throw the rock towards the noisy item")]
    [SerializeField] private TextMeshProUGUI _throwableText;
    [Tooltip("shows text if the player can not throw the rock towards the noisy item")]
    [SerializeField] private TextMeshProUGUI _notThrowableText;
    [Tooltip("the origin of the raycast to signalize the obstacles in the trajectory")] 
    [SerializeField] private Transform _inWayRaycastPosition;
    [Tooltip("Text, that the player is close to the noisy item to activate it per hand")]
    [SerializeField] private TextMeshProUGUI _closeText;
    [Tooltip("the name of the marker to show the current usable noisy item ")]
    [SerializeField] private string _usableMarkerName;

    [SerializeField] private Vector2 _textOffset;

    // signalize the noisy item that it can be activated
    private GameObject _usableMarker;
    
    private SoundItem _soundItem;
    
    public SoundItem SoundItem
    {
        get => _soundItem;
        set => _soundItem = value;
    }

    public TextMeshProUGUI CloseText
    {
        get => _closeText;
        set => _closeText = value;
    }
    
    // the player is able to throw
    private bool _inRange = false;
    
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

    private void Start()
    {
       
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
            _closeText.gameObject.SetActive(false);
            _usableMarker.SetActive(false);
        }
    }

    private void Throw()
    {
        // update the UI position all the time
        _throwableText.transform.position =  new Vector3(_textOffset.x, _textOffset.y, 0) + Input.mousePosition;
        _notThrowableText.transform.position = new Vector3(_textOffset.x, _textOffset.y, 0) + Input.mousePosition;
        _closeText.transform.position = new Vector3(_textOffset.x + 40, _textOffset.y, 0) + Input.mousePosition;
        
        // if the player is near the noisy item, he is able to activate it per hand and doesn't need to throw
        if (_close)
        {
            _usableMarker.SetActive(true);
            _closeText.gameObject.SetActive(true);
            _throwableText.gameObject.SetActive(false);
            _notThrowableText.gameObject.SetActive(false);
        }
        
        if (_inRange && !_close)
        {
            _closeText.gameObject.SetActive(false);
            
            Debug.DrawRay(_inWayRaycastPosition.position, _soundItem.transform.position - transform.position * Vector3.Distance(_soundItem.transform.position, transform.position));
            RaycastHit hit;
            if (Physics.Raycast(_inWayRaycastPosition.position, _soundItem.transform.position - transform.position, out hit, Vector3.Distance(_soundItem.transform.position, transform.position)))
            {
                // if something is blocking the trajectory
                if (hit.collider.CompareTag("Wall"))
                {
                    _usableMarker.SetActive(true);
                    _throwableText.gameObject.SetActive(false);
                    _notThrowableText.gameObject.SetActive(true);
                }
                else
                {
                    _throwableText.gameObject.SetActive(true);
                    _usableMarker.SetActive(true);
                    _notThrowableText.gameObject.SetActive(false);

                    // this should be actually in the state so that it can't be used again when the item can be used again
                    if (Input.GetMouseButtonDown(0))
                    {
                        _usableMarker.SetActive(false);
                        _throwstate = true;
                        _throwableText.gameObject.SetActive(false);
                        _notThrowableText.gameObject.SetActive(false);
                        _inRange = false;
                    }
                }
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
            
            _inRange = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NoisyItem"))
        {
            _throwableText.gameObject.SetActive(false);
            _notThrowableText.gameObject.SetActive(false);
            _usableMarker.SetActive(false);
            _inRange = false;
        }
    }
}
