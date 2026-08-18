using UnityEngine;

/// <summary>
/// Mathematical utility functions for int, float, Vector, and Quaternion types.<br/>
/// NOTE: PhysUtils class has math functions for Rigidbodies!<br/>
/// NOTE #2: TrfMathUtils has corresponding util methods for Transforms!
/// </summary>
public static class MathUtils {
    /// <summary>
    /// Applies a rotation offset to a base rotation.
    /// </summary>
    public static Quaternion AddRotOfs(
        Quaternion baseRot,
        Quaternion ofsRot) {
        return ofsRot * baseRot;
    }

    /// <summary>
    /// Calculates the parent world pose that aligns the child with the target world pose.
    /// NOTE: Parameters use child WORLD position and rotation!
    /// </summary>
    public static (Vector3, Quaternion) AlignChildToTgtPose(
        Vector3 parentPos,
        Quaternion parentRot,
        Vector3 childWldPos,
        Quaternion childWldRot,
        Vector3 tgtWldPos,
        Quaternion tgtWldRot
    ) {
        // Compute the child's current local pose relative to the parent
        Vector3 childParentSpcPos = InvTrfPtUnscaled(parentPos, parentRot, childWldPos);
        Quaternion childParentSpcRot = InvTrfRot(parentRot, childWldRot);
        return AlignLclPoseToTgtPose(childParentSpcPos, childParentSpcRot, tgtWldPos, tgtWldRot);
    }

    /// <summary>
    /// Calculates the world pose that aligns a local pose with the target world pose.
    /// </summary>
    public static (Vector3, Quaternion) AlignLclPoseToTgtPose(
        Vector3 lclPos,
        Quaternion lclRot,
        Vector3 tgtWldPos,
        Quaternion tgtWldRot
    ) {
        Quaternion desiredRot = AlignLclRotToWldRot(tgtWldRot, lclRot);
        Vector3 desiredPos = AlignLclPtToWldPt(tgtWldPos, desiredRot, lclPos);
        return (desiredPos, desiredRot);
    }

    /// <summary>
    /// Returns the parent's new origin pos whose local space point is at the given world space position.
    /// Basically, this finds a position for parent where its child (localPoint) is aligned with the worldPoint. 
    /// </summary>
    public static Vector3 AlignLclPtToWldPt(Vector3 wldPt, Quaternion trfRot, Vector3 lclPt) {
        return wldPt - trfRot * lclPt;
    }

    /// <summary>
    /// Returns the new parent rotation whose local-space rotation matches the given world-space rotation.
    /// Basically, this finds a rotation for the parent where its child (localRot) is aligned with the worldRot.
    /// NOTE: This is equivalent to <see cref="DRot"/>.
    /// </summary>
    public static Quaternion AlignLclRotToWldRot(Quaternion wldRot, Quaternion lclRot) {
        return wldRot * Quaternion.Inverse(lclRot);
    }

    /// <summary>
    /// Calculates angular velocity from the change between two rotations. Small velocities are rounded to 0.
    /// </summary>
    public static Vector3 AngVel(
        Quaternion prevRot,
        Quaternion curRot,
        float dt
    ) {
        Quaternion dRot = DRot(prevRot, curRot);
        dRot.ToAngleAxis(out float angDeg, out Vector3 axis);
        if (angDeg > 180f)
            angDeg -= 360f;
        // Angle axis can apparently return small numerical noise values so:
        if (Mathf.Abs(angDeg) < 0.0001f)
            return Vector3.zero;
        return axis * (angDeg * Mathf.Deg2Rad / dt);
    }

    /// <summary>
    /// Returns the weighted average of angles in radians using circular interpolation.
    /// Correctly handles wrapping around -PI / PI.
    /// </summary>
    public static float AvgAngRad(float ang0, float wt0, float ang1, float wt1) {
        return Mathf.Atan2(
            wt0 * Mathf.Sin(ang0) + wt1 * Mathf.Sin(ang1),
            wt0 * Mathf.Cos(ang0) + wt1 * Mathf.Cos(ang1)
        );
    }

    /// <summary>
    /// Returns a rotation with the twist around <paramref name="axis"/> adjusted
    /// so that it matches the twist difference from <paramref name="fromRot"/> to
    /// <paramref name="toRot"/>.<br/>
    /// Basically takes <paramref name="fromRot"/> and applies the twist around
    /// <paramref name="axis"/> that exists between <paramref name="fromRot"/>
    /// and <paramref name="toRot"/>.
    /// </summary>
    public static Quaternion CalculateRelativeTwist(
        Quaternion fromRot,
        Quaternion toRot,
        Vector3 axis
    ) {
        float twistDeg = ExtractSignedTwistAng(fromRot, toRot, axis) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(twistDeg, axis) * fromRot;
    }

    /// <summary>
    /// Compute the relative rotation between two world space orientations.<br/>
    /// NOTE: This is equivalent to <see cref="AlignLclRotToWldRot"/>.
    /// </summary>
    public static Quaternion DRot(Quaternion fromRot, Quaternion toRot) {
        return toRot * Quaternion.Inverse(fromRot);
    }

    /// <summary>
    /// Returns signed angle around an axis (in radians).<br/>
    /// Can be used to e.g. see how a follow target hand rotation affects the rotation of an (axis-locked)
    /// key in a key hole.
    /// </summary>
    public static float ExtractSignedTwistAng(Quaternion rot, Vector3 axis) {
        // The dot product requires normalized axis.
        axis.Normalize();
        // Ensure equivalent quaternions are represented consistently. This prevents discontinuities where
        // the same rotation can appear as two different quaternions.
        if (rot.w < 0f)
            rot = new Quaternion(-rot.x, -rot.y, -rot.z, -rot.w);
        // Quaternion is projected onto the axis vector to only keep the part of the rotation
        // around the axis.
        Vector3 projected = Vector3.Project(new Vector3(rot.x, rot.y, rot.z), axis);
        Quaternion twist = new Quaternion(projected.x, projected.y, projected.z, rot.w);
        // Valid quaternion needs to have length 1.
        twist.Normalize();
        // NOTE: It is unintuitive that any orientation can be represented by angle axis.
        // TODO: Are 180 degree rotations undefined?
        twist.ToAngleAxis(out float angleDeg, out Vector3 twistAxis);
        // Make sure the original axis and twist axis point in the same direction.
        if (Vector3.Dot(twistAxis, axis) < 0f)
            angleDeg = -angleDeg;
        return angleDeg * Mathf.Deg2Rad;
    }

    /// <summary>
    /// Returns signed angle around an axis (in radians) between two rotations.<br/>
    /// Calculates the relative rotation from <paramref name="fromRot"/> to
    /// <paramref name="toRot"/> and extracts the twist around the given axis.
    /// Can be used to e.g. see how a follow target hand rotation affects the
    /// rotation of an (axis-locked) key in a key hole.<br/>
    /// In that case <paramref name="fromRot"/> would be the initial key rotation
    /// and <paramref name="toRot"/> would be the rotation the key would need to
    /// reach (e.g. based on the follow target hand rotation). The axis would be
    /// the direction into the keyhole.
    /// </summary>
    public static float ExtractSignedTwistAng(Quaternion fromRot, Quaternion toRot, Vector3 axis) {
        Quaternion twistResidual = DRot(fromRot, toRot);
        return ExtractSignedTwistAng(twistResidual, axis);
    }

    /// <summary>
    /// Integrates rotation using angular velocity over a time step.
    /// Basically returns new rotation that is rot rotated by the angVel for dt seconds.
    /// </summary>
    public static Quaternion IntegrateRot(Quaternion rot, Vector3 angVel, float dt) {
        if (IsNearlyZero(angVel))
            return rot;
        return Quaternion.AngleAxis(angVel.magnitude * Mathf.Rad2Deg * dt, angVel.normalized) * rot;
    }

    /// <summary>
    /// Transforms a point from world space to frame pose's local space.
    /// </summary>
    public static Vector3 InvTrfPtUnscaled(Vector3 framePos, Quaternion frameRot, Vector3 ptInWldSpc) {
        return Quaternion.Inverse(frameRot) * (ptInWldSpc - framePos);
    }

    /// <summary>
    /// Converts a world space rotation into the frame rotations's local space rotation.
    /// </summary>
    public static Quaternion InvTrfRot(Quaternion frameRot, Quaternion worldRot) {
        return Quaternion.Inverse(frameRot) * worldRot;
    }

    /// <summary>
    /// Returns whether <paramref name="x"/> is within the specified inclusive range.
    /// </summary>
    /// <param name="x">The value to test.</param>
    /// <param name="gte">The inclusive lower bound of the range.</param>
    /// <param name="lte">The inclusive upper bound of the range.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="x"/> is greater than or equal to
    /// <paramref name="gte"/> and less than or equal to <paramref name="lte"/>; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    public static bool IsInRange(float x, float gte, float lte) {
        Debug.Assert(
            gte <= lte,
            $"Invalid range: lower bound ({gte}) must be less than or equal to " +
            $"upper bound ({lte})."
        );
        return x >= gte && x <= lte;
    }

    /// <summary>
    /// Checks whether a vector is approximately zero using its squared
    /// magnitude, avoiding the less performant <see cref="Vector3.magnitude"/>.
    /// </summary>
    public static bool IsNearlyZero(Vector3 vec, float magThld = 1e-8f) {
        return vec.sqrMagnitude < magThld;
    }

    /// <summary>
    /// Calculates linear velocity from a previous and current position over a time step.
    /// </summary>
    public static Vector3 LinVel(Vector3 prevPos, Vector3 currPos, float dt)
        => (currPos - prevPos) / dt;

    /// <summary>
    /// Normalizes angle to 0-360 degrees.
    /// </summary>
    public static float Nrm360(float ang) {
        ang %= 360f;
        if (ang < 0f) ang += 360f;
        return ang;
    }

    /// <returns>
    /// Value mapped from range 1 to range 2.
    /// </returns>
    public static float Remap(float value, float from1, float to1, float from2, float to2) {
        return Mathf.Lerp(from2, to2, Mathf.InverseLerp(from1, to1, value));
    }

    /// <summary>
    /// Calculates angular acceleration using a spring-damper model between rotations.
    /// Each spring and damper acceleration is clamped independently.
    /// </summary>
    /// <param name="currRot">Current rotation.</param>
    /// <param name="tgtRot">Target rotation.</param>
    /// <param name="relAngVel">Relative angular velocity between the spring object and target.</param>
    /// <param name="angVel">Spring object's angular velocity.</param>
    /// <param name="spring">Angular spring strength.</param>
    /// <param name="maxSpringAcc">Maximum angular spring acceleration.</param>
    /// <param name="velMatchDamper">Damper strength for matching target angular velocity.</param>
    /// <param name="maxVelMatchDamperAcc">Maximum angular velocity-match damper acceleration.</param>
    /// <param name="dragDamper">Damper strength for reducing the spring object's angular velocity.</param>
    /// <param name="maxDragDamperAcc">Maximum angular drag damper acceleration.</param>
    public static Vector3 SpringAngAcc(
        Quaternion currRot,
        Quaternion tgtRot,
        Vector3 relAngVel,
        Vector3 angVel,
        float spring,
        float maxSpringAcc,
        float velMatchDamper,
        float maxVelMatchDamperAcc,
        float dragDamper,
        float maxDragDamperAcc
    ) {
        // Angular spring
        Quaternion dRot = DRot(currRot, tgtRot);
        dRot.ToAngleAxis(out float angleDeg, out Vector3 axis);
        Vector3 springAcc = Vector3.zero;
        if (!IsNearlyZero(axis)) {
            if (angleDeg > 180f)
                angleDeg -= 360f;
            springAcc = axis.normalized * (angleDeg * Mathf.Deg2Rad * spring);
        }
        springAcc = Vector3.ClampMagnitude(springAcc, maxSpringAcc);
        // Angular velocity match damper
        Vector3 velMatchDamperAcc = Vector3.ClampMagnitude(-relAngVel * velMatchDamper, maxVelMatchDamperAcc);
        // Angular drag damper
        Vector3 dragDamperAcc = Vector3.ClampMagnitude(-angVel * dragDamper, maxDragDamperAcc);
        return springAcc + velMatchDamperAcc + dragDamperAcc;
    }

    /// <summary>
    /// Transforms a 2D input vector into world space XZ coordinates using a horizontal
    /// basis whose forward dir is given by basisDir. Can be used to find horizontal input
    /// direction relative to camera fwd dir.
    /// </summary>
    public static Vector2 TrfInputByBasis(Vector2 horInput, Vector3 basisDir) {
        basisDir.y = 0;
        basisDir.Normalize();
        Vector3 basisRight = Vector3.Cross(Vector3.up, basisDir);
        Vector3 movDir = basisRight * horInput.x + basisDir * horInput.y;
        return new Vector2(movDir.x, movDir.z);
    }

    /// <summary>
    /// Returns a transformed point from local space to world space using the specified
    /// origin and rotation.
    /// </summary>
    public static Vector3 TrfPt(Vector3 framePos, Quaternion frameRot, Vector3 lclPt) {
        return frameRot * lclPt + framePos;
    }

    /// <summary>
    /// Transforms a local-space rotation into world space using the given frame rotation.
    /// </summary>
    public static Quaternion TrfRot(Quaternion frameWldRot, Quaternion lclRot) {
        return frameWldRot * lclRot;
    }

    /// <summary>
    /// Updates spring object position and rotation using spring-like movement towards a moving target.<br/>
    /// NOTE: Velocity match damper acceleration is calculated using the relative velocity
    /// between the spring object and the target.<br/>
    /// NOTE #2: Drag damper acceleration is calculated using only the spring object's velocity.<br/>
    /// NOTE #3: Each spring and damper acceleration is clamped independently before they are combined.<br/>
    /// NOTE #4: The combined acceleration is clamped by the total acceleration limit.<br/>
    /// NOTE #5: Large damper values can cause large accelerations that overshoot and reverse velocity direction!
    /// </summary>
    /// <param name="springObjPos">Position of the spring object to update.</param>
    /// <param name="springObjRot">Rotation of the spring object to update.</param>
    /// <param name="tgtPos">Target position.</param>
    /// <param name="tgtRot">Target rotation.</param>
    /// <param name="dt">Time step duration.</param>
    /// <param name="linSpring">Linear spring strength.</param>
    /// <param name="maxLinSpringAcc">Maximum linear spring acceleration.</param>
    /// <param name="linVelMatchDamper">Linear damper strength for matching target velocity.</param>
    /// <param name="maxLinVelMatchDamperAcc">Maximum linear velocity-match damper acceleration.</param>
    /// <param name="linDragDamper">Linear damper strength for reducing the spring object's velocity.</param>
    /// <param name="maxLinDragDamperAcc">Maximum linear drag damper acceleration.</param>
    /// <param name="maxTotalLinAcc">Maximum total linear acceleration.</param>
    /// <param name="angSpring">Angular spring strength.</param>
    /// <param name="maxAngSpringAcc">Maximum angular spring acceleration.</param>
    /// <param name="angVelMatchDamper">Angular damper strength for matching target angular velocity.</param>
    /// <param name="maxAngVelMatchDamperAcc">Maximum angular velocity-match damper acceleration.</param>
    /// <param name="angDragDamper">Angular damper strength for reducing the spring object's angular velocity.</param>
    /// <param name="maxAngDragDamperAcc">Maximum angular drag damper acceleration.</param>
    /// <param name="maxTotalAngAcc">Maximum total angular acceleration.</param>
    public static void UpdateSpringTrf(
        ref Vector3 springObjPos,
        ref Quaternion springObjRot,
        ref Vector3 springObjLinVel,
        ref Vector3 springObjAngVel,
        in Vector3 tgtLinVel,
        in Vector3 tgtAngVel,
        Vector3 tgtPos,
        Quaternion tgtRot,
        float dt,
        float linSpring,
        float maxLinSpringAcc,
        float linVelMatchDamper,
        float maxLinVelMatchDamperAcc,
        float linDragDamper,
        float maxLinDragDamperAcc,
        float maxTotalLinAcc,
        float angSpring,
        float maxAngSpringAcc,
        float angVelMatchDamper,
        float maxAngVelMatchDamperAcc,
        float angDragDamper,
        float maxAngDragDamperAcc,
        float maxTotalAngAcc
    ) {
        // Linear
        Vector3 relLinVel = springObjLinVel - tgtLinVel;
        Vector3 linSpringAcc = Vector3.ClampMagnitude((tgtPos - springObjPos) * linSpring, maxLinSpringAcc);
        Vector3 linVelMatchDamperAcc = Vector3.ClampMagnitude(
            -relLinVel * linVelMatchDamper,
            maxLinVelMatchDamperAcc
        );
        Vector3 linDragDamperAcc = Vector3.ClampMagnitude(-springObjLinVel * linDragDamper, maxLinDragDamperAcc);
        Vector3 totalLinAcc = Vector3.ClampMagnitude(linSpringAcc + linSpringAcc + linVelMatchDamperAcc, maxTotalLinAcc);
        springObjLinVel += totalLinAcc * dt;
        springObjPos += springObjLinVel * dt;
        // Angular
        Vector3 relAngVel = springObjAngVel - tgtAngVel;
        Vector3 angAcc = SpringAngAcc(
            springObjRot,
            tgtRot,
            relAngVel,
            springObjAngVel,
            angSpring,
            maxAngSpringAcc,
            angVelMatchDamper,
            maxAngVelMatchDamperAcc,
            angDragDamper,
            maxAngDragDamperAcc
        );
        angAcc = Vector3.ClampMagnitude(angAcc, maxTotalAngAcc);
        springObjAngVel += angAcc * dt;
        springObjRot = IntegrateRot(
            springObjRot,
            springObjAngVel,
            dt
        );
    }
}
