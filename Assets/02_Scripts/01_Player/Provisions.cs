using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Provisions : MonoBehaviour
{
    [Tooltip("the amount of provisions that can be collect")]
    [SerializeField] private int _collectAmount;

    public int CollectAmount
    {
        get => _collectAmount;
        set => _collectAmount = value;
    }
}
