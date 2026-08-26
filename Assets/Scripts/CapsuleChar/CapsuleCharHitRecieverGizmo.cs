using UnityEngine;

// TODO BEFORE BUILD: Remove instances from the game.
public class CapsuleCharHitRecieverGizmo : MonoBehaviour{
    [Header("Settings")]
    [SerializeField] bool drawGizmo = true;
    [SerializeField] Color vulnerableColor = Color.darkViolet;
    [SerializeField] Color invulnerableColor = Color.cyan;

    [Header("External Refs")]
    [SerializeField] Pc pc;
    [SerializeField] Collider col;

    private void OnDrawGizmos() {
        CapsuleCharMgr caMgr = CapsuleCharMgr.inst;
        // NOTE: No warning, no error. You need to remember to set the references!
        if (pc == null || col == null || !drawGizmo || CapsuleCharMgr.inst == null)
            return;
        Color color = caMgr.data.invul[pc.Id] ? invulnerableColor : vulnerableColor;
        if (col is CapsuleCollider capsuleCollider) {
            float radius = capsuleCollider.radius;
            float height = Mathf.Max(capsuleCollider.height, radius * 2f);
            float cylinderHeight = height - radius * 2f;
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
                sphereCollider.radius,
                color
            );
        }
        else if (col is BoxCollider boxCollider) {
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.color = color;
            Gizmos.matrix = col.transform.localToWorldMatrix;
            DebugUtils.OnDrawGizmos_DrawWireCube(boxCollider.center, boxCollider.size);
            Gizmos.matrix = oldMatrix;
        }
    }
}
