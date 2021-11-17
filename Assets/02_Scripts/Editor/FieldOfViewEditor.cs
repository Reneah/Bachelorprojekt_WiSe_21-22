using System;
using UnityEditor;
using UnityEngine;

// The Source of the code: https://www.youtube.com/watch?v=j1-OyLo77ss&t=973s
[CustomEditor(typeof(EnemyController))]
public class FieldOfViewEditor : Editor
{
   private void OnSceneGUI()
   {
     /* // the reference of the FieldOfView script
      EnemyController fov = (EnemyController) target;
      // the color of the circle of the enemy vision range
      Handles.color = Color.white;
      // the circle of the enemy vision range
      Handles.DrawWireArc(fov.EnemyHead.position, Vector3.up,Vector3.forward, 360, fov.Radius);

      // the direction of the angle size
      Vector3 viewAngle01 = DirectionFromAngle(fov.EnemyHead.eulerAngles.y, - fov.Angle / 2);
      Vector3 viewAngle02 = DirectionFromAngle(fov.EnemyHead.eulerAngles.y, + fov.Angle / 2);
      
      // the visualization of the angle
      Handles.color = Color.yellow;
      Handles.DrawLine(fov.EnemyHead.position, fov.EnemyHead.position + viewAngle01 * fov.Radius);
      Handles.DrawLine(fov.EnemyHead.position, fov.EnemyHead.position + viewAngle02 * fov.Radius);

      // if the enemy can see the player, the raycast will be called
      if (fov.CanSeePlayer)
      {
         Handles.color = Color.green;
         Handles.DrawLine(fov.ObstacleRaycastTransform.position, fov.LookPositionAtSpotted.position);
      }
      */
   }

   /// <summary>
   /// the direction of the angle size
   /// </summary>
   /// <param name="eulerY">the current euler angle of the enemy</param>
   /// <param name="angleInDegress">the fixed vision angle of the enemy in degrees</param>
   /// <returns></returns>
   private Vector3 DirectionFromAngle(float eulerY, float angleInDegress)
   {
      angleInDegress += eulerY;

      return new Vector3(Mathf.Sin(angleInDegress * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegress * Mathf.Deg2Rad));
   }
}
