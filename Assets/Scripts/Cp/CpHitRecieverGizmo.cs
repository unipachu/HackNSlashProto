// BEFORE BUILD: Remove instances from the game.
using UnityEngine;

/// <summary>
/// Draws a debug gizmo representing hit reciever collision and wether the character is invulnerable.
/// </summary>
public class CpHitRecieverGizmo : MonoBehaviour{
    [Header("Settings")]
    [SerializeField] bool drawGizmo = true;
    [SerializeField] Color vulnerableColor = Color.darkViolet;
    [SerializeField] Color invulnerableColor = Color.cyan;
    [Tooltip("This expects the collider to not be scaled by parent transforms. If it is however, use is "
        + "this to scale the gizmo.")]
    [SerializeField] Vector3 sizeMult = new(1,1,1);

    [Header("External Refs")]
    [SerializeField] InterfaceReference<ICp> cp;
    [SerializeField] Collider col;

    private void OnDrawGizmos() {
        // NOTE: No warning, no error. You need to remember to set the references!
        if (cp == null || col == null || !drawGizmo || CpMgr.inst == null)
            return;
        Color color = cp.Value.CommonData.ignoreHits ? invulnerableColor : vulnerableColor;
        if (col is CapsuleCollider capsuleCollider) {
            var radiusMult = Mathf.Max(sizeMult.x, sizeMult.z);
            float radius = capsuleCollider.radius * radiusMult;
            float cylinderHeight = Mathf.Max(capsuleCollider.height - capsuleCollider.radius * 2f, 0f);
            Vector3 center = capsuleCollider.center;
            Vector3 top = center + Vector3.up * (cylinderHeight * 0.5f);
            Vector3 bottom = center - Vector3.up * (cylinderHeight * 0.5f);
            DebugUtils.OnDrawGizmos_DrawCapsule(
                col.transform.TransformPoint(bottom),
                col.transform.TransformPoint(top),
                radius,
                color
            );
        }
        else if (col is SphereCollider sphereCollider) {
            DebugUtils.OnDrawGizmos_DrawSphere(
                col.transform.TransformPoint(sphereCollider.center),
                sphereCollider.radius * Mathf.Max(sizeMult.x, sizeMult.z),
                color
            );
        }
        else if (col is BoxCollider boxCollider) {
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.color = color;
            Gizmos.matrix = col.transform.localToWorldMatrix;
            DebugUtils.OnDrawGizmos_DrawWireCube(
                boxCollider.center,
                Vector3.Scale(boxCollider.size, sizeMult)
            );
            Gizmos.matrix = oldMatrix;
        }
    }
}
