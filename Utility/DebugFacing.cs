using UnityEngine;

public class DebugFacing : MonoBehaviour {
    private void OnDrawGizmos() {
        // Set the color for the gizmo
        Gizmos.color = Color.red;

        // Draw an arrow pointing in the object's forward direction
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2);

        // Draw an arrowhead
        var right = transform.position + (transform.forward + transform.right).normalized * 0.5f;
        var left = transform.position + (transform.forward - transform.right).normalized * 0.5f;
        Gizmos.DrawLine(transform.position + transform.forward * 2, right);
        Gizmos.DrawLine(transform.position + transform.forward * 2, left);
    }
}