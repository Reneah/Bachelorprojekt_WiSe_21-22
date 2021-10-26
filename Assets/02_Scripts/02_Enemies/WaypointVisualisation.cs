using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointVisualisation : MonoBehaviour
{ 
    [Tooltip("the sphere size of the waypoint")]
   [SerializeField] float wayPointsSize;

    [Tooltip(" the color of the waypoints and ways")]
    [SerializeField] private Color32 _wayColor;

    private void OnDrawGizmos()
    {        
        for (int i = 0; i < transform.childCount; i++)
        {
            // draws the waypoints and the way between the points
            Gizmos.color = _wayColor;
            Gizmos.DrawSphere(transform.GetChild(i).position, wayPointsSize);
            Gizmos.DrawLine(transform.GetChild(GetWaypoints(i)).position, transform.GetChild(GetWaypoints(i + 1)).position);
        }
    }

    /// <summary>
    /// Get the waypoints in the children of the parent
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    private int GetWaypoints(int i)
    {
        if(i >= transform.childCount)
        {
            return 0;
        }

        else
        {
            return i;
        }
    }
}
