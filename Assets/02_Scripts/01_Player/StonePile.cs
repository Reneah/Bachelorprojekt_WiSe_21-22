using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonePile : MonoBehaviour
{
    [Tooltip("the amount of stones that can be collect")]
    [SerializeField] private float _collectAmount;

    public float CollectAmount
    {
        get => _collectAmount;
        set => _collectAmount = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
