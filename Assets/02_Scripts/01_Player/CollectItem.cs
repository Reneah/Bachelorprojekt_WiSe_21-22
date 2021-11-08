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
    [Tooltip("shows the collectable item")]
    [SerializeField] private TextMeshProUGUI _itemCollectableText;
    [Tooltip("shows the current button to use")]
    [SerializeField] private TextMeshProUGUI _useButtonText;
    [Tooltip("shows, that it is in the possession of the player")] 
    [SerializeField] private GameObject _ItemImage;

    [Header("Choose the ONE Item which will be represented")] 
    [Tooltip("the key to open doors")] 
    [SerializeField] private bool _key;
    [Tooltip("has to be picked up by the player as a quest item")] 
    [SerializeField] private bool _backpack;
    [Tooltip("has to be picked up by the player as a quest item")] 
    [SerializeField] private bool _parchment;
    [Tooltip("has to be interacted with by the player as a quest task")] 
    [SerializeField] private bool _secretPassage;

    // Bela: Don't see why this should be needed.
    /*public bool Key
    {
        get => _key;
        set => _key = value;
    }*/

    //[SerializeField]
    private float _textVanishTime;
    
    private static bool _keyCollected = false;
    private static bool _backpackCollected = false;
    private static bool _parchmentCollected = false;
    private static bool _secretPassageOpened = false;

    private float _vanishTime;
    private bool _itemCollected = false;

    private SceneChange _sceneChange;
    private PlayerController _playerController;
    
    void Start()
    {
        _vanishTime = _textVanishTime;
        
        _useButtonText.gameObject.SetActive(false);
        _itemCollectableText.gameObject.SetActive(false);
        _ItemImage.SetActive(false);
        _sceneChange = FindObjectOfType<SceneChange>();
        _playerController = FindObjectOfType<PlayerController>();
    }
    
    void Update()
    {
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
            _itemCollectableText.gameObject.SetActive(true);
            _useButtonText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")  && !_itemCollected)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
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
                }
                
                _itemCollectableText.gameObject.SetActive(false);
                _useButtonText.gameObject.SetActive(false);
                _ItemImage.SetActive(true);
                _itemCollected = true;
                gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.CompareTag("Player") && !_itemCollected)
            {
                _useButtonText.gameObject.SetActive(false);
                _itemCollectableText.gameObject.SetActive(false);
            }
        }
    }
}
