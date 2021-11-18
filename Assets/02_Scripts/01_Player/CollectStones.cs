using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CollectStones : MonoBehaviour
{
    [Tooltip("the item text that will show up when the player is in range and hovers over the item")]
    [SerializeField] private TextMeshProUGUI _stoneText;
    [Tooltip("the item text that will show up when the player hovers over the item and it is not available")]
    [SerializeField] private TextMeshProUGUI _negativeStoneText;
    [Tooltip("the amount of stones that the player can collect with this pile")]
    [SerializeField] private TextMeshProUGUI _stonesAmountText;
    [Tooltip("modify the text position at the mouse position")]
    [SerializeField] private Vector2 _textOffset;
    [Tooltip("the amount of stones that can be collected")]
    [SerializeField] private float _maxStoneAmount;

    private GameObject _usebleMarker;
    
    private float _stonesCounter = 0;
    private bool _stonesCollectible = false;
    private GameObject _stones;

    public float StonesCounter
    {
        get => _stonesCounter;
        set => _stonesCounter = value;
    }

    void Start()
    {
        _stonesAmountText.text = "Stones: " + _stonesCounter;
        
        // just create a new GameObject to not be null. Otherwise, the usable marker will not dissappear.
        // the randomness doesn't matter, because when the player enters the trigger, it will be updated and can only be used in the trigger
        _usebleMarker = new GameObject();
    }
    
    void Update()
    {
        _stoneText.transform.position = new Vector3(_textOffset.x, _textOffset.y, 0) + Input.mousePosition;
        _negativeStoneText.transform.position = new Vector3(_textOffset.x, _textOffset.y, 0) + Input.mousePosition;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit _hit;

        if (Physics.Raycast(ray, out _hit, Mathf.Infinity, LayerMask.GetMask("Stones")))
        {
            if (!_stonesCollectible)
            {
                _negativeStoneText.gameObject.SetActive(true);
            }
            
            else if (_stonesCollectible)
            {
                _usebleMarker.SetActive(true);
                _stoneText.gameObject.SetActive(true);
                _negativeStoneText.gameObject.SetActive(false);
                
                if (Input.GetMouseButtonDown(0))
                {
                    _stonesCounter += _stones.GetComponent<StonePile>().CollectAmount;
                    
                    // if max amount reached, return
                    if (_maxStoneAmount <= _stonesCounter)
                    {
                        // maximize the stone amount, when for example only 2 stones can be collected even through the pile amount is 3
                        _stonesCounter = _maxStoneAmount;
                    }
                    
                    _stonesAmountText.text = "Stones: " + _stonesCounter;
                    _stones.SetActive(false);
                    _stonesCollectible = false;
                }
            }
        }
        else
        {
            _stoneText.gameObject.SetActive(false);
            _negativeStoneText.gameObject.SetActive(false);
            _usebleMarker.SetActive(false);
        }
    }

    public void StoneUsed()
    {
        // if 0 is reached, return
        if (0 >= _stonesCounter)
        {
            return;
        }
        _stonesCounter--;
        _stonesAmountText.text = "Stones: " + _stonesCounter;
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Stones"))
        {
            _stones = other.gameObject;
            _stonesCollectible = true;
            
            _usebleMarker = other.transform.GetChild(0).gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Stones"))
        {
            _stonesCollectible = false;
        }
    }
}
