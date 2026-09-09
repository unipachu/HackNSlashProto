using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Math utility methods for Transform type.<br/>
/// NOTE: Math utilities for non-Object types are in MathUtils!
/// </summary>
public static class TrfMathUtils {
    /// <summary>
    /// Calculates the parent world pose that aligns the child with the target world pose.
    /// </summary>
    public static (Vector3, Quaternion) AlignChildToTgtPose(
        Transform parentTrf,
        Transform childTrf,
        Vector3 tgtWldPos,
        Quaternion tgtWldRot
    ) {
        return MathUtils.AlignChildToTgtPose(
            parentTrf.position,
            parentTrf.rotation,
            childTrf.position,
            childTrf.rotation,
            tgtWldPos,
            tgtWldRot
        );
    }

    // TODO: Perhaps expand this so that parameter takes in the axis and pivot pos and rot instead of
    // TODO C: the transform. Or actually create a separate method that doesn't take in pivot transform
    // TODO C: but instead uses only pivot wld pos and direction of the axis to rotate around.
    /// <summary>
    /// Returns world pos and rot of an object when rotated around the right-axis of a pivot object. 
    /// </summary>
    public static (Vector3, Quaternion) ComputeNewPoseByRotAroundPivTrfXAxis(
        Transform movedTrf,
        Transform pivTrf,
        float rotAroundAxis,
        float rotMult = 1
    ) {
        // NOTE: rotMult is used here to rotate the object slightly further.
        float dXAng = rotAroundAxis * rotMult;
        //Debug.Log("delta x angle: " + deltaXAngle);
        // TODO: Make the local axis of the pivot a parameter.
        Quaternion dRotAroundPivRight = Quaternion.AngleAxis(dXAng, pivTrf.right);
        Vector3 movedTrfPosInPivSpace = movedTrf.position.InvTrfPtUnscaled(pivTrf);
        Quaternion movedTrfRotInPivSpace = movedTrf.rotation.InvTrfRot(pivTrf);
        Quaternion pivFutureRot = dRotAroundPivRight * pivTrf.rotation;
        Vector3 movedTrfNextWorldPos = movedTrfPosInPivSpace.TrfPt(pivTrf.position, pivFutureRot);
        Quaternion movedTrfNextRot = movedTrfRotInPivSpace.TrfRot(pivFutureRot);
        return (movedTrfNextWorldPos, movedTrfNextRot);
    }

    /// <summary>
    /// Transforms a point from world space to unscaled local space,
    /// ignoring the transform's scale (unlike Transform.InverseTransformPoint).
    /// </summary>
    public static Vector3 InvTrfPtUnscaled(this Vector3 ptInWldSpc, Transform trf)
        => ptInWldSpc.InvTrfPtUnscaled(trf.position, trf.rotation);

    /// <summary>
    /// Converts a world space rotation into the transform's local space rotation.
    /// </summary>
    public static Quaternion InvTrfRot(this Quaternion rotInWorldSpace, Transform trf)
        => rotInWorldSpace.InvTrfRot(trf.rotation);

    /// <summary>
    /// Rotates forward towards the target vector in xz-plane.
    /// </summary>
    /// <param name="tgtInXZPlane">Forward direction in XZ-plane.</param>
    public static quaternion RotateFwdToTgt(quaternion rot, float maxAngSpd, float2 tgtInXZPlane) {
        if (math.all(tgtInXZPlane == float2.zero))
            return rot;
        float3 dir3D = new float3(tgtInXZPlane.x, 0, tgtInXZPlane.y);
        if (math.lengthsq(dir3D) < 0.0001f) {
            Debug.LogWarning("Look rotation viewing vector was zero");
            return rot;
        }
        quaternion tgtRot = quaternion.LookRotation(dir3D, Vector3.up);
        return rot.RotateTowards(tgtRot, maxAngSpd * Time.deltaTime);
    }

    /// <summary>
    /// Transforms a point from unscaled local space to world space,
    /// ignoring the transform's scale (unlike Transform.TransformPoint).
    /// </summary>
    public static Vector3 TrfPtUnscaled(this Vector3 ptInTrfSpace, Transform trf)
        => ptInTrfSpace.TrfPt(trf.position, trf.rotation);

    /// <summary>
    /// Converts a transforms's local space rotation into world space rotation.
    /// </summary>
    public static Quaternion TrfRot(this Quaternion rotInTrfSpace, Transform trf)
        => rotInTrfSpace.TrfRot(trf.rotation);
}
