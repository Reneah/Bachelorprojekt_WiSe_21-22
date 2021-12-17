using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


    public class CollectibleCloseActivation : MonoBehaviour
    {
        private CollectItem _collectItem;
        private QuestManager _questManager;
        
        void Start()
        {
            _collectItem = GetComponentInParent<CollectItem>();
            _questManager = FindObjectOfType<QuestManager>();
        }
        
        void Update()
        {
            if (Input.GetKey(KeyCode.Mouse0) && _collectItem.HitCollectable && _collectItem.ItemCollectible)
            {
                _collectItem.ItemCollectible = false;
                    
                if (_collectItem.Key)
                {
                    CollectItem._keyCollected = true;
                }
                else if(_collectItem.Backpack)
                {
                    CollectItem._backpackCollected = true;
                    _collectItem.SceneChange.ChangeScene();
                    _collectItem.PlayerController.enabled = false;
                }
                else if(_collectItem.Parchment)
                {
                    CollectItem._parchmentCollected = true;
                    _collectItem.SceneChange.ChangeScene();
                    _collectItem.PlayerController.enabled = false;
                }
                else if(_collectItem.ThroneCompartment)
                {
                    // Bela: Apparently this value is never used
                    CollectItem._throneCompartmentOpened = true;
                    // This works fine
                    _collectItem.RemoveThroneParticleEffect.SetActive(false);
                    _collectItem.SpawnKey.SetActive(true);
                    _collectItem.gameObject.layer = LayerMask.GetMask("Default");
                    _collectItem.enabled = false;
                    gameObject.SetActive(false);
                }
                else if(_collectItem.StaircaseToCellar)
                {
                    if (!CollectItem._keyCollected || !_questManager.ProvisionsQuestDone)
                    { //This seems to cause issues, as you can't pick up with the secret door even when you have collected the key
                        Debug.Log("You haven't met all conditions yet to enter the staircase.");
                        return;
                    }
                    CollectItem._enteredStaircase = true;
                    _collectItem.SceneChange.ChangeScene();
                    _collectItem.PlayerController.enabled = false;
                    _collectItem.Enemies.SetActive(false);
                }
                
                _collectItem.CollectibleSprite.gameObject.SetActive(false);
                _collectItem.ItemImage.SetActive(true);
                _collectItem.ItemCollected = true;
                    
                // Temporary solution: If it's the secret passage, do NOT deactivate the game object
                if (_collectItem.StaircaseToCellar || _collectItem.ThroneCompartment)
                {
                    return;
                }
                _collectItem.gameObject.SetActive(false);
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")  && !_collectItem.ItemCollected)
            {
                _collectItem.CollectibleSprite.gameObject.SetActive(true);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player")  && !_collectItem.ItemCollected)
            {
                _collectItem.ItemCollectible = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (other.CompareTag("Player") && !_collectItem.ItemCollected)
                {
                    _collectItem.CollectibleSprite.gameObject.SetActive(false);
                    _collectItem.ItemCollectible = false;
                }
            }
        }
    }

