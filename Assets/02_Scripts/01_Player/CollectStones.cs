using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class CollectStones : MonoBehaviour
{
    [Tooltip("the item sprite that will show up when the player is in range and hovers over the item")]
    [SerializeField] private GameObject _stoneSprite;
    [Tooltip("the item sprite that will show up when the player hovers over the item and it is not available")]
    [SerializeField] private GameObject _negativeStoneSprite;
    [Tooltip("the amount of stones that the player can collect with this pile")]
    [SerializeField] private TextMeshProUGUI _stonesAmountText;
    [Tooltip("modify the text position at the mouse position")]
    [SerializeField] private Vector2 _textOffset;
    [Tooltip("the amount of stones that can be collected")]
    [SerializeField] private int _maxStoneAmount;
    [Tooltip("The GO of the stones UI element and text")]
    [SerializeField] private GameObject _stonesUIelements;
    
    private GameObject _usebleMarker;
    
    private int _stonesCounter = 0;
    private bool _stonesCollectible = false;
    private GameObject _stones;
    public static bool _stonesActive;
    public static bool _UIdisplayed;

    public int StonesCounter
    {
        get => _stonesCounter;
        set => _stonesCounter = value;
    }

    void Start()
    {
        _UIdisplayed = System.Convert.ToBoolean(PlayerPrefs.GetInt("StoneUI", 0));
        _stonesActive = System.Convert.ToBoolean(PlayerPrefs.GetInt("StoneActive", 0));
        
        _stonesAmountText.text = _stonesCounter.ToString();
        
        // just create a new GameObject to not be null. Otherwise, the usable marker will not disappear.
        // the randomness doesn't matter, because when the player enters the trigger, it will be updated and can only be used in the trigger
        _usebleMarker = new GameObject();
        
        _stonesCounter = PlayerPrefs.GetInt("StonesAmount", 0);

        if (_stonesCounter > 0)
        {
            _stonesUIelements.SetActive(true);
        }
        
        _stonesAmountText.text = _stonesCounter.ToString();
    }
    
    void Update()
    {
        if (_stonesActive && !_UIdisplayed)
        {
            _stonesUIelements.SetActive(true);
            _UIdisplayed = true;
            PlayerPrefs.SetInt("StoneUI", 0);
        }
        
        _stoneSprite.transform.position = new Vector3(_textOffset.x, _textOffset.y, 0) + Input.mousePosition;
        _negativeStoneSprite.transform.position = new Vector3(_textOffset.x, _textOffset.y, 0) + Input.mousePosition;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit _hit;

        if (Physics.Raycast(ray, out _hit, Mathf.Infinity, LayerMask.GetMask("Stones")))
        {
            if (!_stonesCollectible)
            {
                _negativeStoneSprite.gameObject.SetActive(true);
            }
            
            else if (_stonesCollectible)
            {
                _usebleMarker.SetActive(true);
                _stoneSprite.SetActive(true);
                _negativeStoneSprite.gameObject.SetActive(false);
                
                if (Input.GetMouseButtonDown(0))
                {
                    _stonesActive = true;
                    PlayerPrefs.SetInt("StoneActive", 1);
                    _stonesCounter += _stones.GetComponent<StonePile>().CollectAmount;
                    
                    
                    if (_maxStoneAmount <= _stonesCounter)
                    {
                        // maximize the stone amount, when for example only 2 stones can be collected even through the pile amount is 3
                        _stonesCounter = _maxStoneAmount;
                    }
                    
                    _stonesAmountText.text = _stonesCounter.ToString();
                    _stones.SetActive(false);
                    _stonesCollectible = false;
                }
            }
        }
        else
        {
            _stoneSprite.SetActive(false);
            _negativeStoneSprite.gameObject.SetActive(false);
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
        _stonesAmountText.text = _stonesCounter.ToString();
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
