using UnityEngine;

namespace Enemy.SpottedBarRotation
{
    public class SpottedBarTowardsCamera : MonoBehaviour
    {
        private Transform _spottedBar;
        
        void Start()
        {
            _spottedBar = GetComponent<Transform>();
        }
        
        void Update()
        {
            //rotate the spotted bar towards the main camera
            var direction = Camera.main.transform.position  - transform.position;
            _spottedBar.rotation = Quaternion.LookRotation(new Vector3(direction.x,0,direction.z));
        }
    }
}
