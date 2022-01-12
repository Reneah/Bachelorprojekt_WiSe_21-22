using UnityEngine;

public class StonePile : MonoBehaviour
{
    [Tooltip("the amount of stones that can be collect")]
    [SerializeField] private int _collectAmount;

    [SerializeField] private string _playerPrefsKey;

    [SerializeField] private GameObject _stonePileParent;

    public GameObject StonePileParent
    {
        get => _stonePileParent;
        set => _stonePileParent = value;
    }

    private bool _collected = false;
    private bool _safeState;

    public bool SafeState
    {
        get => _safeState;
        set => _safeState = value;
    }

    private void Start()
    {
        _collected = System.Convert.ToBoolean(PlayerPrefs.GetInt(_playerPrefsKey, 0));
        
        if (_collected)
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (_safeState)
        {
            PlayerPrefs.SetInt(_playerPrefsKey, _collected.GetHashCode());
            _safeState = false;
        }
    }

    public int CollectAmount()
    {
        _collected = true;
        return _collectAmount;
    }
}
