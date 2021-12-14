using System;
using System.Collections;
using System.Collections.Generic;
using Enemy.SoundItem;
using TMPro;
using UnityEngine;
using untitledProject;

public class PlayerThrowTrigger : MonoBehaviour
{
    [Header("Throwing Rocks")]
    [Tooltip("shows sprite if the player can throw the rock towards the noisy item")]
    [SerializeField] private GameObject _throwableSprite;
    [Tooltip("shows sprite if the player can not throw the rock towards the noisy item")]
    [SerializeField] private GameObject _notThrowableSprite;
    [Tooltip("the origin of the raycast to signalize the obstacles in the trajectory")] 
    [SerializeField] private Transform _inWayRaycastPosition;
    [Tooltip("modify the text position at the mouse position")]
    [SerializeField] private Vector2 _textOffset;
    [Tooltip("the rotation speed towards the throw target")]
    [SerializeField] private float _rotationSpeed;
    [Tooltip("wait to throw during rotation towards the target")]
    [SerializeField] public float _waitToThrowDuringRotation;
    [Tooltip("the set game object will spawn to signalize the AI where the player has thrown")]
    [SerializeField] private Transform _lastThrowPositionObject;
    [Tooltip("the noisy item layer signalize the noisy item in the world ")]
    [SerializeField] private LayerMask _noisyItemLayer;
    [Tooltip("the throw distance on the noisy item")]
    [SerializeField] private float _throwDistance;
    

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

    // signalize that the noisy item can be activated in throw range
    private GameObject _throwRange;
    
    private NoisyItem _noisyItem;
    
    public NoisyItem NoisyItem
    {
        get => _noisyItem;
        set => _noisyItem = value;
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

    private Ray ray;
    private RaycastHit _hit;
    private bool _hitNoisyItem;

    private void Start()
    {
        _collectStones = FindObjectOfType<CollectStones>();
        
        // just find a random sound item and new GameObject to not be null. Otherwise, there will be errors
        // the randomness and new GameObject creation doesn't matter, because when the player enters the trigger, it will be updated and can only be used in the trigger
        _noisyItem = FindObjectOfType<NoisyItem>();
    }

    private void Update()
    {
        Throw();
    }
    
    private void Throw()
    {
        // update the UI position all the time
        _throwableSprite.transform.position =  new Vector3(_textOffset.x, _textOffset.y, 0) + Input.mousePosition;
        _notThrowableSprite.transform.position = new Vector3(_textOffset.x, _textOffset.y, 0) + Input.mousePosition;
        
        // if the player is near the noisy item, he is able to activate it per hand and doesn't need to throw
        if (_close)
        {
            _throwableSprite.gameObject.SetActive(false);
            _notThrowableSprite.gameObject.SetActive(false);
            _throwRange.SetActive(false);
        }
        
        if (!_close)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            _hitNoisyItem = Physics.Raycast(ray, out _hit, Mathf.Infinity, _noisyItemLayer);
            
                // if the mouse is hovering over the noisy item, the corresponding text will show up and the throw is available
                //!Physics.Raycast(ray, Mathf.Infinity, LayerMask.GetMask("Blocked"))
                if(_hitNoisyItem) 
                {
                    _noisyItem = _hit.collider.GetComponent<NoisyItem>();
                    _throwRange = _noisyItem.ThrowRangeRadius;
                    
                    if (!_throwstate && Vector3.Distance(transform.position, _hit.collider.gameObject.transform.position) < _throwDistance)
                    {
                        _throwAvailable = true;
                    }
                    
                    _throwRange.SetActive(true);
                    _notThrowableSprite.gameObject.SetActive(true);
                    
                    if (_throwAvailable && _collectStones.StonesCounter > 0)
                    {
                        // if something is blocking the trajectory
                        if(Physics.Raycast(_inWayRaycastPosition.position, _noisyItem.transform.position - _inWayRaycastPosition.position, out hit, Vector3.Distance(_noisyItem.transform.position, _inWayRaycastPosition.position), LayerMask.GetMask("Wall")))
                        {
                            _throwableSprite.gameObject.SetActive(false);
                            _notThrowableSprite.gameObject.SetActive(true);
                        }
                        else
                        {
                            _throwableSprite.gameObject.SetActive(true);
                            _notThrowableSprite.gameObject.SetActive(false);
                            
                            if (Input.GetMouseButtonDown(0))
                            {
                                _throwRange.SetActive(false);
                                _throwstate = true;
                                _throwableSprite.gameObject.SetActive(false);
                                _throwAvailable = false;
                                _throwPosition = Instantiate(_lastThrowPositionObject, transform.position, Quaternion.identity);
                                _playerThrew = true;
                            }
                        } 
                    }
                }
                else
                {
                    _throwableSprite.gameObject.SetActive(false);
                    _notThrowableSprite.gameObject.SetActive(false);
                    if (_throwRange != null)
                    {
                        _throwRange.SetActive(false);
                    }
                    _throwAvailable = false;
                }
        }
    }
}
