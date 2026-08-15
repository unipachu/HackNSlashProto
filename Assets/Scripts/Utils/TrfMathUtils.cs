using UnityEngine;

/// <summary>
/// Math utility methods for Transform type.<br/>
/// NOTE: Math utilities for non-Object types are in MathUtils!
/// </summary>
public class TrfMathUtils : MonoBehaviour {
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
        Vector3 movedTrfPosInPivSpace = InvTrfPtUnscaled(pivTrf, movedTrf.position);
        Quaternion movedTrfRotInPivSpace = InvTrfRot(pivTrf, movedTrf.rotation);
        Quaternion pivFutureRot = dRotAroundPivRight * pivTrf.rotation;
        Vector3 movedTrfNextWorldPos = MathUtils.TrfPt(pivTrf.position, pivFutureRot, movedTrfPosInPivSpace);
        Quaternion movedTrfNextRot = MathUtils.TrfRot(pivFutureRot, movedTrfRotInPivSpace);
        return (movedTrfNextWorldPos, movedTrfNextRot);
    }

    /// <summary>
    /// Transforms a point from world space to unscaled local space,
    /// ignoring the transform's scale (unlike Transform.InverseTransformPoint).
    /// </summary>
    public static Vector3 InvTrfPtUnscaled(Transform trf, Vector3 ptInWldSpc) {
        return MathUtils.InvTrfPtUnscaled(trf.position, trf.rotation, ptInWldSpc);
    }

    /// <summary>
    /// Converts a world space rotation into the transform's local space rotation.
    /// </summary>
    public static Quaternion InvTrfRot(Transform trf, Quaternion rotInWorldSpace) {
        return MathUtils.InvTrfRot(trf.rotation, rotInWorldSpace);
    }

    /// <summary>
    /// Transforms a point from unscaled local space to world space,
    /// ignoring the transform's scale (unlike Transform.TransformPoint).
    /// </summary>
    public static Vector3 TrfPtUnscaled(Transform trf, Vector3 ptInTrfSpace) {
        return MathUtils.TrfPt(trf.position, trf.rotation, ptInTrfSpace);
    }

    /// <summary>
    /// Converts a transforms's local space rotation into world space rotation.
    /// </summary>
    public static Quaternion TrfRot(Transform trf, Quaternion rotInTrfSpace) {
        return MathUtils.TrfRot(trf.rotation, rotInTrfSpace);
    }
}
