using UnityEngine;
using UnityEditor;

public class Brush : MonoBehaviour
{
    //[SerializeField] GameObject planet;

    private void OnDrawGizmos()
    {
        ShowBrush();
        HandleUtility.Repaint();
    }

    void ShowBrush()
    {
        if (EditorApplication.isPlaying) return;

        try
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(hit.point, 0.1f);
            }
        } catch { } // Happens when mouse is out of the scene's viewport
    }
}
