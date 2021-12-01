using UnityEngine;

namespace Enemy.WaypointVisualisation
{
    public class PointVisualization : MonoBehaviour
    {
        [Tooltip(" the color of the visualization")]
        [SerializeField] private Color32 _color;
        [Tooltip("the size of the agent visualization")]
        [SerializeField] private float _radius;
    
        private void OnDrawGizmos()
        {
            Gizmos.color = _color;
            Gizmos.DrawSphere(transform.position, _radius);
        }
    }
}

