using TMPro;
using UnityEngine;
using untitledProject;

public class CollectItem : MonoBehaviour
{ 
    [Header("Item UI")]
    [Tooltip("shows, that it is in the possession of the player")] 
    [SerializeField] private GameObject _ItemImage;
    [Tooltip("modify the text position at the mouse position")]
    [SerializeField] private Vector2 _textOffset;
    [Tooltip("the item sprite that will show up when the player is in range, hovers over the item and when it is available")]
    [SerializeField] private GameObject _collectibleSprite;
    [Tooltip("the item sprite that will show up when the player hovers over the item and it is not available")]
    [SerializeField] private GameObject _negativeSprite;
    [Tooltip("signalize that the stones can be collected")]
    [SerializeField] private GameObject _usebleMarker;
    [Tooltip("only needed for throne to activate next quest item")]
    [SerializeField] private GameObject _spawnKey;
    [Tooltip("only needed for key to deactivate throne interaction particle effect")]
    [SerializeField] private GameObject _removeThroneParticleEffect;


    [Header("Choose the ONE Item which will be represented")] 
    [Tooltip("the key to open doors")] 
    [SerializeField] private bool _key;
    [Tooltip("has to be picked up by the player as a quest item")] 
    [SerializeField] private bool _backpack;
    [Tooltip("has to be picked up by the player as a quest item")] 
    [SerializeField] private bool _parchment;
    [Tooltip("has to be interacted with by the player as a quest task")] 
    [SerializeField] private bool _throneCompartment;
    [Tooltip("has to be interacted with by the player as a quest task")] 
    [SerializeField] private bool _staircaseToCellar;
    
    public static bool _backpackCollected = false;
    public static bool _parchmentCollected = false;
    public static bool _throneCompartmentOpened = false;
    public static bool _keyCollected = false;
    public static bool _enteredStaircase = false;
    
    private bool _itemCollectible;
    
    // a raycast in another script hits the collectable to activate the UI & functionalities
    private bool _hitCollectable;

    public bool HitCollectable
    {
        get => _hitCollectable;
        set => _hitCollectable = value;
    }

    public bool ItemCollected
    {
        get => _itemCollected;
        set => _itemCollected = value;
    }

    public GameObject CollectibleSprite
    {
        get => _collectibleSprite;
        set => _collectibleSprite = value;
    }

    public bool StaircaseToCellar
    {
        get => _staircaseToCellar;
        set => _staircaseToCellar = value;
    }

    public GameObject ItemImage
    {
        get => _ItemImage;
        set => _ItemImage = value;
    }

    public GameObject Enemies
    {
        get => _enemies;
        set => _enemies = value;
    }

    public PlayerController PlayerController
    {
        get => _playerController;
        set => _playerController = value;
    }

    public SceneChange SceneChange
    {
        get => _sceneChange;
        set => _sceneChange = value;
    }

    public bool Key
    {
        get => _key;
        set => _key = value;
    }

    public bool ItemCollectible
    {
        get => _itemCollectible;
        set => _itemCollectible = value;
    }

    public bool Backpack
    {
        get => _backpack;
        set => _backpack = value;
    }

    public bool Parchment
    {
        get => _parchment;
        set => _parchment = value;
    }

    public bool ThroneCompartment
    {
        get => _throneCompartment;
        set => _throneCompartment = value;
    }

    public GameObject SpawnKey
    {
        get => _spawnKey;
        set => _spawnKey = value;
    }

    public GameObject RemoveThroneParticleEffect
    {
        get => _removeThroneParticleEffect;
        set => _removeThroneParticleEffect = value;
    }

    //[SerializeField]
    private float _textVanishTime;
    
    private bool _itemCollected = false;

    private SceneChange _sceneChange;
    private PlayerController _playerController;
    private GameObject _enemies;
    
    void Start()
    {
        _collectibleSprite.gameObject.SetActive(false);
        _ItemImage.SetActive(false);
        _sceneChange = FindObjectOfType<SceneChange>();
        _playerController = FindObjectOfType<PlayerController>();
        _enemies = GameObject.Find("Enemies");
    }
    
    void Update()
    {
        _collectibleSprite.transform.position = new Vector3(_textOffset.x, _textOffset.y, 0) + Input.mousePosition;
        _negativeSprite.transform.position = new Vector3(_textOffset.x, _textOffset.y, 0) + Input.mousePosition;
        
        if (_hitCollectable)
        {
            if (!_itemCollectible)
            {
                _negativeSprite.gameObject.SetActive(true);
            }
            
            else if (_itemCollectible)
            {
                _usebleMarker.SetActive(true);
                _collectibleSprite.gameObject.SetActive(true);
                _negativeSprite.gameObject.SetActive(false);
            }
        }
        else
        {
            _collectibleSprite.gameObject.SetActive(false);
            _negativeSprite.gameObject.SetActive(false);
            _usebleMarker.SetActive(false);
        }
    }
}
