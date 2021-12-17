using System;
using System.Collections;
using System.Collections.Generic;
using Enemy.Controller;
using Enemy.SoundItem;
using UnityEngine;
using untitledProject;

public class Checkpoint : MonoBehaviour
{
    private CollectStones _collectStones;
    private CollectProvisions _collectProvisions;

    private NoisyItem[] _noisyItems;
    
    private void Start()
    {
        _collectStones = FindObjectOfType<CollectStones>();
        _collectProvisions = FindObjectOfType<CollectProvisions>();

        _noisyItems = FindObjectsOfType<NoisyItem>();
    }

    private static bool _blockUpdate;

    public static bool BlockUpdate
    {
        get => _blockUpdate;
        set => _blockUpdate = value;
    }

    private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerPrefs.SetFloat("PlayerPositionX",transform.position.x);
                PlayerPrefs.SetFloat("PlayerPositionY",transform.position.y);
                PlayerPrefs.SetFloat("PlayerPositionZ",transform.position.z);
                PlayerPrefs.SetInt("StonesAmount", _collectStones.StonesCounter);
                PlayerPrefs.SetInt("ProvisionsAmount", _collectProvisions.ProvisionsCounter);
                
                for (int i = 0; i < _noisyItems.Length; i++)
                {
                    _noisyItems[i].SafeState = true;
                }
                
                PlayerPrefs.Save();
                BlockUpdate = true;

                gameObject.SetActive(false);
            }
            
        }
    }

