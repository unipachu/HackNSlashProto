using UnityEngine;

/// <summary>
/// Utility and extension methods for Physics Objects such as Rigidbodies and physics joints, and
/// other PhysX related things such as physics queries.
/// </summary>
public static class PhysUtils{
    /// <summary>
    /// Interpolates rb's pose with rb.Move to align the specified child transform with a target pose.<br/>
    /// NOTE: Call this in FixedUpdate()!<br/>
    /// NOTE #2: Since rigidbodies and transforms can get out of sync, the child pose should be
    /// cached instead of just using a child Transform reference.
    /// </summary>
    /// <param name="rb">Rigidbody to be moved.</param>
    /// <param name="childLclPos">
    /// Local positiong ot the child of the rigidbody we want to align with the target.
    /// </param>
    /// <param name="t">Lerp parameter (0-1).</param>
    public static void InterpRbSoChildAlignsWithTgtPose(
        this Rigidbody rb,
        Vector3 childLclPos,
        Quaternion childLclRot,
        Vector3 tgtWldPos,
        Quaternion tgtWldRot,
        float t
    ) {
        var targetPose = MathUtils.AlignLclPoseToTgtPose(childLclPos, childLclRot, tgtWldPos, tgtWldRot);
        t = Mathf.Clamp01(t);
        Vector3 newPos = Vector3.Lerp(rb.position, targetPose.Item1, t);
        Quaternion newRot = Quaternion.Slerp(rb.rotation, targetPose.Item2, t);
        rb.Move(newPos, newRot);
    }

    /// <summary>
    /// Transforms a point from world space to unscaled Rigidbody local space,
    /// ignoring Rigidbody's scale.
    /// </summary>
    public static Vector3 InvTrfPtUnscaled(this Vector3 ptInWldSpc, Rigidbody rb)
        => ptInWldSpc.InvTrfPtUnscaled(rb.position, rb.rotation);

    /// <summary>
    /// Converts a world space rotation into the rigidbody's local space rotation.
    /// </summary>
    public static Quaternion InvTrfRot(this Quaternion rotInWorldSpace, Rigidbody rb)
        => rotInWorldSpace.InvTrfRot(rb.rotation);

    /// <summary>
    /// Draws small sphere where the joint anchor is.<br/>
    /// NOTE: Call this in OnDrawGizmos!
    /// </summary>
    public static void OnDrawGizmos_DrawJntAnch(ConfigurableJoint jnt) {
        if (jnt == null) {
            Debug.LogWarning("ConfigurableJoint was null.");
            return;
        }
        Gizmos.color = Color.yellow;
        Vector3 worldAnchorPos = jnt.anchor.TrfPtUnscaled(jnt.transform);
        Gizmos.DrawWireSphere(worldAnchorPos, 0.01f);
    }

    /// <summary>
    /// Draws small sphere where the joint anchor is.<br/>
    /// NOTE: Call this in OnDrawGizmos!
    /// </summary>
    public static void OnDrawGizmos_DrawJntConnectedAnch(ConfigurableJoint jnt) {
        if (jnt != null && jnt.connectedBody != null) {
            Gizmos.color = Color.darkOrange;
            Vector3 worldAnchorPos = jnt.connectedAnchor.TrfPtUnscaled(jnt.connectedBody.transform);
            Gizmos.DrawWireSphere(worldAnchorPos, 0.01f);
        }
    }

    /// <summary>
    /// Transforms a point from unscaled Rigidbody local space to world space,
    /// using the Rigidbody's position and rotation.
    /// </summary>
    public static Vector3 TrfPtUnscaled(this Vector3 ptInRbSpace, Rigidbody rb)
        => ptInRbSpace.TrfPt(rb.position, rb.rotation);

    /// <summary>
    /// Converts a rigidbody's local space rotation into world space rotation.
    /// </summary>
    public static Quaternion TrfRot(this Quaternion rotInRbSpace, Rigidbody rb)
        => rotInRbSpace.TrfRot(rb.rotation);
}
