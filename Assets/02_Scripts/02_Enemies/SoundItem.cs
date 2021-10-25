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
    [Tooltip("the enemy will walk to the point of interest")] 
    [SerializeField] private bool _firstStage;
    [Tooltip("the enemy will run to the point of interest")] 
    [SerializeField] private bool _secondStage;
    [Tooltip("the enemy runs to the point of interest, knows that the player is nearby and start searching")] 
    [SerializeField] private bool _thirdStage;
    
    public bool FirstStage
    {
        get => _firstStage;
        set => _firstStage = value;
    }

    public bool SecondStage
    {
        get => _secondStage;
        set => _secondStage = value;
    }

    public bool ThirdStage
    {
        get => _thirdStage;
        set => _thirdStage = value;
    }

    private bool _itemUsed = false;
    
    void Start()
    {
        _useButtonText.gameObject.SetActive(false);
        _itemText.gameObject.SetActive(false);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")  && !_itemUsed)
        {
            _itemText.gameObject.SetActive(true);
            _useButtonText.gameObject.SetActive(true);
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
                
                _itemUsed = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.CompareTag("Player") && !_itemUsed)
            {
                _useButtonText.gameObject.SetActive(false);
                _itemText.gameObject.SetActive(false);
            }
        }
    }
}
