using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGizmoTest : MonoBehaviour
{
    public Vector2[] poses;

    private void OnDrawGizmos()
    {
    
        Color originColor = Gizmos.color;
        Gizmos.color = Color.blue;
        if (poses != null && poses.Length > 0)
        {
            //Draw Sphere
            for (int i = 0; i < poses.Length; i++)
            {
                Gizmos.DrawSphere(poses[i], 10f);
            }

            //Draw Line
            Gizmos.color = Color.yellow;
            Vector2 lastPos = Vector2.zero;
            for (int i = 0; i < poses.Length; i++)
            {
                if (i > 0)
                {
                    Gizmos.DrawLine(lastPos, poses[i]);
                }
                lastPos = poses[i];
            }
        }
        Gizmos.color = originColor;
    }
}
