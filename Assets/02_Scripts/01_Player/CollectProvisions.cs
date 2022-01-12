using DarkTonic.MasterAudio;
using TMPro;
using UnityEngine;

public class CollectProvisions : MonoBehaviour
{
    [Tooltip("the item sprite that will show up when the player is in range and hovers over the item")]
    [SerializeField] private GameObject _provisionsSprite;
    [Tooltip("the item sprite that will show up when the player hovers over the item and it is not available")]
    [SerializeField] private GameObject _negativeProvisionsSprite;
    [Tooltip("the amount of provisions that the player can collect with this pile")]
    [SerializeField] private TextMeshProUGUI _provisionsAmountText;
    [Tooltip("modify the text position at the mouse position")]
    [SerializeField] private Vector2 _textOffset;
    [Tooltip("the amount of provisions that can be collected")]
    [SerializeField] private int _maxProvisionsAmount;
    [Tooltip("The GO of the provisions UI element and text")]
    [SerializeField] private GameObject _ProvisionsUIelements;
    
    private int _provisionsCounter = 0;
    private bool _provisionsCollectible = false;
    private Provisions _provisions;
    public static bool _provisionsActive;
    public static bool _UIdisplayed;
    private PlayerController _playerController;

    public int ProvisionsCounter
    {
        get => _provisionsCounter;
        set => _provisionsCounter = value;
    }

    void Start()
    {
        _UIdisplayed = System.Convert.ToBoolean(PlayerPrefs.GetInt("ProvisionsUI", 0));
        _provisionsActive = System.Convert.ToBoolean(PlayerPrefs.GetInt("ProvisionsActive", 0));

        _playerController = FindObjectOfType<PlayerController>();
        
        _provisionsAmountText.text = _provisionsCounter.ToString();
        
        _provisionsCounter = PlayerPrefs.GetInt("ProvisionsAmount", 0);

        if (_provisionsCounter > 0)
        {
            _ProvisionsUIelements.SetActive(true);
        }
        
        _provisionsAmountText.text = _provisionsCounter.ToString();
    }
    
    void Update()
    {
        if (_provisionsActive && !_UIdisplayed)
        {
            _ProvisionsUIelements.SetActive(true);
            _UIdisplayed = true;
            PlayerPrefs.SetInt("ProvisionsUI", 0);
        }
        
        _provisionsSprite.transform.position = new Vector3(_textOffset.x, _textOffset.y, 0) + Input.mousePosition;
        _negativeProvisionsSprite.transform.position = new Vector3(_textOffset.x, _textOffset.y, 0) + Input.mousePosition;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit _hit;

        if (Physics.Raycast(ray, out _hit, Mathf.Infinity, LayerMask.GetMask("Provisions")))
        {
            _provisions = _hit.collider.GetComponent<Provisions>();
            if (!_provisionsCollectible)
            {
                _negativeProvisionsSprite.gameObject.SetActive(true);
            }
            
            else if (_provisionsCollectible)
            {
                _provisionsSprite.SetActive(true);
                _negativeProvisionsSprite.gameObject.SetActive(false);
                
                if (Input.GetMouseButtonDown(0))
                {
                    _playerController.PickUpItem = true;
                    _provisionsActive = true;
                    PlayerPrefs.SetInt("ProvisionsActive", 1);
                    _provisionsCounter += _provisions.CollectAmount();
                    
                    if (_maxProvisionsAmount <= _provisionsCounter)
                    {
                        // maximize the provisions amount, when for example only 2 provisions can be collected even through the pile amount is 3
                        _provisionsCounter = _maxProvisionsAmount;
                    }
                    
                    _provisionsAmountText.text = _provisionsCounter.ToString();
                    MasterAudio.PlaySound3DAtTransform("Backpack", _provisions.transform);
                    _provisions.GetComponent<Collider>().enabled = false;
                    _provisions.GetComponent<Provisions>().ProvisionsParent.SetActive(false);
                    _provisionsCollectible = false;
                }
            }
        }
        else
        {   _provisionsSprite.SetActive(false);
            _negativeProvisionsSprite.gameObject.SetActive(false);
        }
    }

    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Provisions"))
        {
            _provisionsCollectible = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Provisions"))
        {
            _provisionsCollectible = false;
        }
    }
}
