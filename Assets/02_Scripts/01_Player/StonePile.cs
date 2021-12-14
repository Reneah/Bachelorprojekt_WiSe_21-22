using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonePile : MonoBehaviour
{
    [Tooltip("the amount of stones that can be collect")]
    [SerializeField] private int _collectAmount;

    public int CollectAmount
    {
        get => _collectAmount;
        set => _collectAmount = value;
    }
}
