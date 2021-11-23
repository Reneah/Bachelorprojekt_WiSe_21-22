using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleItemRaycast : MonoBehaviour
{
    private Collider _item;
    
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit _hit;
        
        if (Physics.Raycast(ray, out _hit, Mathf.Infinity, LayerMask.GetMask("CollectibleItem")))
        {
            _item = _hit.collider;
            _item.GetComponent<CollectItem>().HitCollectable = true;
        }
        else if(!Physics.Raycast(ray, out _hit, Mathf.Infinity, LayerMask.GetMask("CollectibleItem")) && _item != null)
        {
            _item.GetComponent<CollectItem>().HitCollectable = false;
        }
    }
}
