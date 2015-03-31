using UnityEngine;
using System.Collections;

[AddComponentMenu("Util/Gizmo (Draw Cube)")]
public class GizmoDrawCube : MonoBehaviour
{
    #region Fields

    public Color GizmoColor = Color.red;
    public float GizmoAlpha = 0.5f;

    #endregion

    #region Unity Callbacks

    private void OnDrawGizmos()
    {
        GizmoColor.a = GizmoAlpha;
        Gizmos.color = GizmoColor;
        Gizmos.DrawCube(transform.position, transform.localScale);
    }

    #endregion
}