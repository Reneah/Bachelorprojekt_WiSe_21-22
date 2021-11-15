using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CollectStones : MonoBehaviour
{
    [Tooltip("the item text that will show up when the player is in range and hovers over the item")]
    [SerializeField] private TextMeshProUGUI _stoneText;
    [Tooltip("modify the text position at the mouse position")]
    [SerializeField] private Vector2 _textOffset;
    [Tooltip("the amount of stones that can be collected")]
    [SerializeField] private float _stonesAmount;
    [Tooltip("the amount of stones that the player can collect with this pile")]
    [SerializeField] private float _stonesAmountText;
    
    private float _stonesCounter;
    private bool _stonesCollectible = false;
    private GameObject _stones;
    
    void Start()
    {
        
    }
    
    void Update()
    {
        _stoneText.transform.position = new Vector3(_textOffset.x, _textOffset.y, 0) + Input.mousePosition;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit _hit;

        if (Physics.Raycast(ray, out _hit, Mathf.Infinity, LayerMask.GetMask("Stones")))
        {
            _stoneText.gameObject.SetActive(true);
            
            if (_stonesCollectible)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _stones.SetActive(false);
                }
            }
        }
        else
        {
            _stoneText.gameObject.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.GetMask("Stones"))
        {
            _stones = other.gameObject;
            _stonesCollectible = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.GetMask("Stones"))
        {
            _stonesCollectible = false;
        }
    }
}
