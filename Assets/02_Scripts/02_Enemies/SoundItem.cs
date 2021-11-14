using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SoundItem : MonoBehaviour
{ 
    [Header("Item UI")]
    [Tooltip("shows the usable item")]
   [SerializeField] private TextMeshProUGUI _itemText;
    [Tooltip("shows the current button to use")]
   [SerializeField] private TextMeshProUGUI _useButtonText;


    [Header("Choose the ONE Stage which will be represented")]
    [Tooltip("First Stage: the enemy will walk to the point of interest" +
             " Second Stage: the enemy will run to the point of interest" +
             " Third Stage: the enemy runs to the point of interest, knows that the player is nearby and start searching")]
    [Range(1,3)]
    [SerializeField] private int stage;

   // [Tooltip("modify the text position at the mouse position")]
     private Vector2 _textOffset;

    [Header("Sound Collider")]
    [Tooltip("the collider, which shows the sound range of the item")]
    [SerializeField] private GameObject _soundRangeCollider;
    
    [SerializeField] private bool _reusable;

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

    public int Stage
    {
        get => stage;
        set => stage = value;
    }
    
    private bool _itemUsed = false;

    public bool ItemUsed
    {
        get => _itemUsed;
        set => _itemUsed = value;
    }

    void Start()
    {
        _useButtonText.gameObject.SetActive(false);
        _itemText.gameObject.SetActive(false);
        _playerThrowTrigger = FindObjectOfType<PlayerThrowTrigger>();
        
    }

    private void Update()
    {
        _textOffset.x = 270;
        _textOffset.y = -60;
        _useButtonText.transform.position = new Vector3(_textOffset.x, _textOffset.y, 0) + Input.mousePosition;
        
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
        if (other.CompareTag("Player")  && !_itemUsed)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                _itemText.gameObject.SetActive(false);
                _useButtonText.gameObject.SetActive(false);
                _soundRangeCollider.SetActive(true);

                _itemUsed = true;
                return;
            }
            
            _playerThrowTrigger.Close = true;
            _itemText.gameObject.SetActive(true);
            _useButtonText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.CompareTag("Player") && !_itemUsed)
            {
                _playerThrowTrigger.Close = false;
                _useButtonText.gameObject.SetActive(false);
                _itemText.gameObject.SetActive(false);
            }
        }
    }
}
