using UnityEngine;

public class CharacterControllerGizmo : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Color gizmoColor = Color.green;
    
    [Header("External Refs")]
    [SerializeField] private CharacterController characterController;

    private void OnDrawGizmos()
    {
        // NOTE: No warning, no error. You need to remember to set the reference!
        if (characterController == null) return;

        Gizmos.color = gizmoColor;

        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;

        Vector3 center = characterController.center;
        float radius = characterController.radius;
        float height = Mathf.Max(characterController.height, radius * 2f);

        float cylinderHeight = height - radius * 2f;

        Vector3 top = center + Vector3.up * (cylinderHeight * 0.5f);
        Vector3 bottom = center - Vector3.up * (cylinderHeight * 0.5f);

        // Top and bottom spheres
        Gizmos.DrawWireSphere(top, radius);
        Gizmos.DrawWireSphere(bottom, radius);

        // Vertical lines
        Vector3 forward = Vector3.forward * radius;
        Vector3 back = Vector3.back * radius;
        Vector3 right = Vector3.right * radius;
        Vector3 left = Vector3.left * radius;

        Gizmos.DrawLine(top + forward, bottom + forward);
        Gizmos.DrawLine(top + back, bottom + back);
        Gizmos.DrawLine(top + right, bottom + right);
        Gizmos.DrawLine(top + left, bottom + left);

        Gizmos.matrix = oldMatrix;
    }
}
