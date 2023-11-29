using UnityEditor;
using UnityEngine;
using GGG.Components.Enemies;

namespace GGG
{
    [CustomEditor(typeof(FieldOfViewUpgraded))]
    public class FOVEditor : Editor
    {
        private void OnSceneGUI()
        {
            FieldOfViewUpgraded fov = (FieldOfViewUpgraded)(target);
            Handles.color = Color.white;
            Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.radius);

            Vector3 viewAngle01 = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.angle / 2);
            Vector3 viewAngle02 = DirectionFromAngle(fov.transform.eulerAngles.y, fov.angle / 2);


            Handles.color = Color.yellow;
            Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle01 * fov.radius);
            Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle02 * fov.radius);

            //if(fov.canSeePlayer)
            //{
            //    Handles.color = Color.green;
            //    Handles.DrawLine(fov.transform.position, fov.playerRef.transform.position);
            //}
        }

        private Vector3 DirectionFromAngle(float eulerY, float angleDegrees)
        {
            angleDegrees += eulerY;

            return new(Mathf.Sin(angleDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleDegrees * Mathf.Deg2Rad));
        }
    }
}
