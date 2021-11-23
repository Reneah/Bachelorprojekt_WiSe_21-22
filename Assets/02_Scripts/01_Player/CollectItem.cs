using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using untitledProject;

public class CollectItem : MonoBehaviour
{ 
    [Header("Item UI")]
    [Tooltip("shows, that it is in the possession of the player")] 
    [SerializeField] private GameObject _ItemImage;
    [Tooltip("modify the text position at the mouse position")]
    [SerializeField] private Vector2 _textOffset;
    [Tooltip("the item text that will show up when the player is in range, hovers over the item and when it is available")]
    [SerializeField] private TextMeshProUGUI _collectibleText;
    [Tooltip("the item text that will show up when the player hovers over the item and it is not available")]
    [SerializeField] private TextMeshProUGUI _negativeText;
    [Tooltip("signalize that the stones can be collected")]
    [SerializeField] private GameObject _usebleMarker;

    [Header("Choose the ONE Item which will be represented")] 
    [Tooltip("the key to open doors")] 
    [SerializeField] private bool _key;
    [Tooltip("has to be picked up by the player as a quest item")] 
    [SerializeField] private bool _backpack;
    [Tooltip("has to be picked up by the player as a quest item")] 
    [SerializeField] private bool _parchment;
    [Tooltip("has to be interacted with by the player as a quest task")] 
    [SerializeField] private bool _secretPassage;
    
    private bool _itemCollectible;
    
    // a raycast in another script hits the collectable to activate the UI & functionalities
    private bool _hitCollectable;

    public bool HitCollectable
    {
        get => _hitCollectable;
        set => _hitCollectable = value;
    }

    //[SerializeField]
    private float _textVanishTime;
    
    public static bool _keyCollected = false;
    public static bool _backpackCollected = false;
    public static bool _parchmentCollected = false;
    public static bool _secretPassageOpened = false;

    private float _vanishTime;
    private bool _itemCollected = false;

    private SceneChange _sceneChange;
    private PlayerController _playerController;
    private GameObject _enemies;
    
    void Start()
    {
        _vanishTime = _textVanishTime;
        
        _collectibleText.gameObject.SetActive(false);
        _ItemImage.SetActive(false);
        _sceneChange = FindObjectOfType<SceneChange>();
        _playerController = FindObjectOfType<PlayerController>();
        _enemies = GameObject.Find("Enemies");
    }
    
    void Update()
    {
        _collectibleText.transform.position = new Vector3(_textOffset.x, _textOffset.y, 0) + Input.mousePosition;
        _negativeText.transform.position = new Vector3(_textOffset.x, _textOffset.y, 0) + Input.mousePosition;
        
        if (_hitCollectable)
        {
            if (!_itemCollectible)
            {
                _negativeText.gameObject.SetActive(true);
            }
            
            else if (_itemCollectible)
            {
                _usebleMarker.SetActive(true);
                _collectibleText.gameObject.SetActive(true);
                _negativeText.gameObject.SetActive(false);
                
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    _itemCollectible = false;
                    
                    if (_key)
                    {
                        _keyCollected = true;
                    }
                    else if(_backpack)
                    {
                        _backpackCollected = true;
                        _sceneChange.ChangeScene();
                        _playerController.enabled = false;
                    }
                    else if(_parchment)
                    {
                        _parchmentCollected = true;
                        _sceneChange.ChangeScene();
                        _playerController.enabled = false;
                    }
                    else if(_secretPassage)
                    {
                        if (!_keyCollected)
                        { //This seems to cause issues, as you can't pick up with the secret door even when you have collected the key
                            return;
                        }
                        _secretPassageOpened = true;
                        _sceneChange.ChangeScene();
                        _playerController.enabled = false;
                        _enemies.SetActive(false);
                    }
                
                    _collectibleText.gameObject.SetActive(false);
                    _ItemImage.SetActive(true);
                    _itemCollected = true;
                    gameObject.SetActive(false);
                }
            }
        }
        else
        {
            _collectibleText.gameObject.SetActive(false);
            _negativeText.gameObject.SetActive(false);
            _usebleMarker.SetActive(false);
        }
        
        if (_itemCollected)
        {
            _vanishTime -= Time.deltaTime;

            if (_vanishTime <= 0)
            { 
                // have the possibility to deactivate the object later when something is running with cooldown after collecting the item
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")  && !_itemCollected)
        {
            _collectibleText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")  && !_itemCollected)
        {
            _itemCollectible = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.CompareTag("Player") && !_itemCollected)
            {
                _collectibleText.gameObject.SetActive(false);
                _itemCollectible = false;
            }
        }
    }
}
