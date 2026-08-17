using UnityEngine;

// TODO BEFORE BUILD: Remove instances from the game.
public class CharacterControllerGizmo : MonoBehaviour{
    [Header("Settings")]
    [SerializeField] bool drawGizmo = true;
    [SerializeField] Color gizmoColor = Color.green;
    
    [Header("External Refs")]
    [SerializeField] CharacterController characterController;

    private void OnDrawGizmos(){
        // NOTE: No warning, no error. You need to remember to set the reference!
        if (characterController == null || !drawGizmo)
            return;
        float radius = characterController.radius;
        float height = Mathf.Max(characterController.height, radius * 2f);
        float cylinderHeight = height - radius * 2f;
        Vector3 center = characterController.center;
        Vector3 top = center + Vector3.up * (cylinderHeight * 0.5f);
        Vector3 bottom = center - Vector3.up * (cylinderHeight * 0.5f);
        DebugUtils.OnDrawGizmos_DrawCapsule(
            transform.TransformPoint(bottom),
            transform.TransformPoint(top),
            radius,
            gizmoColor
        );
    }
}
