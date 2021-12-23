using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Provisions : MonoBehaviour
{
    [Tooltip("the amount of provisions that can be collect")]
    [SerializeField] private int _collectAmount;
    
    [SerializeField] private string _playerPrefsKey;
    [SerializeField] private GameObject _provisionsParent;
    
    private bool _collected;
    private bool _safeState;

    public bool SafeState
    {
        get => _safeState;
        set => _safeState = value;
    }
    
    public GameObject ProvisionsParent
    {
        get => _provisionsParent;
        set => _provisionsParent = value;
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
