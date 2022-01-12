using UnityEngine;

namespace Enemy.SoundItem
{
    public class NoisyItemCloseActivation : MonoBehaviour
    {
        private NoisyItem _noisyItem;

        private void Start()
        {
            _noisyItem = GetComponentInParent<NoisyItem>();
        }
        
        void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit _hit;
            
            // when the player hovers over the item, is in close range and the item is available, it can be used
            if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out _hit, Mathf.Infinity, _noisyItem.NoisyItemLayer) && _noisyItem.PlayerThrowTrigger.Close && !_noisyItem.ItemUsed && _noisyItem.ItemUsable)
            {
                _noisyItem.PlayerThrowTrigger.PlayerThrew = false;
                
                if (_noisyItem.OneTimeUsed)
                {
                    _noisyItem.Stage++;

                    if (_noisyItem.Stage >= 3)
                    {
                        _noisyItem.Stage = 3;
                    }
                }
                
                _noisyItem.SoundRangeCollider.gameObject.SetActive(true);
                _noisyItem.CollectibleSprite.gameObject.SetActive(false);
                _noisyItem.NegativeSprite.gameObject.SetActive(true);

                _noisyItem.ItemUsable = false;
                _noisyItem.ItemUsed = true;
            }
        }
        
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player") && !_noisyItem.ItemUsed)
            {
                _noisyItem.PlayerThrowTrigger.Close = true;
                _noisyItem.ItemUsable = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (other.CompareTag("Player") && !_noisyItem.ItemUsed)
                {
                    _noisyItem.ItemUsable = false;
                    _noisyItem.PlayerThrowTrigger.Close = false;
                    _noisyItem.CollectibleSprite.gameObject.SetActive(false);
                }
            }
        }
    }
}
