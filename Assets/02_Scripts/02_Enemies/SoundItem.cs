using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SoundItem : MonoBehaviour
{ 
    [Header("Item")]
    [Tooltip("shows the current button to use")]
    [SerializeField] private TextMeshProUGUI _useButtonText;
    [SerializeField] private bool _reusable;
    
    [Header("Sound Stage")]
    [Tooltip("First Stage: the enemy will walk to the point of interest" +
             " Second Stage: the enemy will run to the point of interest" +
             " Third Stage: the enemy runs to the point of interest, knows that the player is nearby and start searching")]
    [Range(1,3)]
    [SerializeField] private int _stage;

   // [Tooltip("modify the text position at the mouse position")]
     private Vector2 _textOffset;

    [Header("Sound Collider")]
    [Tooltip("the collider, which shows the sound range of the item")]
    [SerializeField] private GameObject _soundRangeCollider;
    
    public bool Reusable
    {
        get => _reusable;
        set => _reusable = value;
    }

    private bool _reuseItem;

    // deactivate the sound collider after a fixed time
    private float _deactivationTime = 0.3f;

    public GameObject SoundRangeCollider
    {
        get => _soundRangeCollider;
        set => _soundRangeCollider = value;
    }

    private PlayerThrowTrigger _playerThrowTrigger;

    // when the player is able to reuse the sound, the alert stage of the enemy rise
    private bool _oneTimeUsed = false;

    public bool OneTimeUsed
    {
        get => _oneTimeUsed;
        set => _oneTimeUsed = value;
    }

    public int Stage
    {
        get => _stage;
        set => _stage = value;
    }
    
    private bool _itemUsed = false;

    public bool ItemUsed
    {
        get => _itemUsed;
        set => _itemUsed = value;
    }

    private bool _itemUsable = false;

    void Start()
    {
        _useButtonText.gameObject.SetActive(false);
        _playerThrowTrigger = FindObjectOfType<PlayerThrowTrigger>();
        
    }

    private void Update()
    {
        _textOffset.x = 270;
        _textOffset.y = -60;
        _useButtonText.transform.position = new Vector3(_textOffset.x, _textOffset.y, 0) + Input.mousePosition;

        ItemActivation();
        ItemExecution();
    }

    private void ItemActivation()
    {
        if (_itemUsable)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (_oneTimeUsed)
                {
                    _stage++;

                    if (_stage >= 3)
                    {
                        _stage = 3;
                    }
                }
            
                _useButtonText.gameObject.SetActive(false);
                _soundRangeCollider.SetActive(true);

                _itemUsed = true;
                return;
            }
            
            _playerThrowTrigger.Close = true;
            _useButtonText.gameObject.SetActive(true);
        }
    }
    
    /// <summary>
    /// decides if the item can be reused or not after a certain time
    /// </summary>
    private void ItemExecution()
    {
        if (_itemUsed)
        {
            _deactivationTime -= Time.deltaTime;

            if (_deactivationTime <= 0)
            {
                if (_reusable)
                {
                    _deactivationTime = 0.3f;
                    _soundRangeCollider.SetActive(false);
                    _itemUsed = false;
                    _oneTimeUsed = true;
                }
                else
                {
                    _soundRangeCollider.SetActive(false);
                    Destroy(this);
                }

            }
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !_itemUsed)
        {
            _itemUsable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.CompareTag("Player") && !_itemUsed)
            {
                _itemUsable = false;
                _playerThrowTrigger.Close = false;
                _useButtonText.gameObject.SetActive(false);
            }
        }
    }
}
