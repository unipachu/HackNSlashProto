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
    /// Performs a non-allocating capsule overlap query and calculates contact-like information for each
    /// overlapping collider. If the movement direction is not
    /// <see cref="MathUtils.IsZeroOrNearlyZero"/>, the capsule is moved
    /// 1.5 units opposite the movement direction and cast back toward its current position. If a capsule
    /// cast hit matches an overlapping collider, its hit point and normal are returned. If the movement
    /// direction is close to zero, a raycast is instead performed from the capsule center toward the
    /// collider position using half the capsule height as the maximum distance. If no raycast provides
    /// contact information, the capsule center is used as the hit point and <see cref="Vector3.zero"/>
    /// is used as the normal.
    /// </summary>
    /// <param name="capsule">World-space capsule to query with.</param>
    /// <param name="layerMask">Layer mask used by the overlap and capsule cast queries.</param>
    /// <param name="overlapResults">Preallocated collider result array.</param>
    /// <param name="hitPts">Preallocated array receiving the calculated hit points.</param>
    /// <param name="movDir">
    /// Movement direction used to perform the capsule cast. If its magnitude is less than or equal to
    /// 0.001f, the capsule cast is skipped and a center-based raycast is used instead.
    /// </param>
    /// <param name="normals">Preallocated array receiving the calculated normals.</param>
    /// <param name="qryTrgIxn">Specifies how trigger colliders are handled.</param>
    /// <returns>The number of colliders found by the overlap query.</returns>
    public static int OverlapCapsuleNonAllocWithContactInfo(
        CapsuleShape capsule,
        int layerMask,
        Collider[] overlapResults,
        Vector3[] hitPts,
        Vector3 movDir,
        Vector3[] normals,
        QueryTriggerInteraction qryTrgIxn = QueryTriggerInteraction.Collide
    ) {
        int numCols = Physics.OverlapCapsuleNonAlloc(
            capsule.pt0,
            capsule.pt1,
            capsule.r,
            overlapResults,
            layerMask,
            qryTrgIxn
        );
        Vector3 capsuleCenter = (capsule.pt0 + capsule.pt1) * 0.5f;
        if (!movDir.IsZeroOrNearlyZero()) {
            Vector3 normMovDir = movDir.normalized;
            Vector3 castOffset = -normMovDir * 1.5f;
            Vector3 castPt0 = capsule.pt0 + castOffset;
            Vector3 castPt1 = capsule.pt1 + castOffset;
            // NOTE: Cast results can have more hits than the overlap results. Here we make the assumption that
            // C: overlapResults.Length has enough room for both query results.
            // TODO MINOR: This could be provided by the method caller to avoid needless allocation.
            RaycastHit[] castResults = new RaycastHit[overlapResults.Length];
            int numCastHits = Physics.CapsuleCastNonAlloc(
                castPt0,
                castPt1,
                capsule.r,
                normMovDir,
                castResults,
                1.5f, // This is an arbitraty number. We cast 1.5 units away from the overlap capsule.
                layerMask,
                qryTrgIxn
            );
            for (int i = 0; i < numCols; i++) {
                hitPts[i] = capsuleCenter;
                normals[i] = Vector3.zero;
                Collider collider = overlapResults[i];
                for (int j = 0; j < numCastHits; j++) {
                    if (castResults[j].collider != collider)
                        continue;
                    hitPts[i] = castResults[j].point;
                    normals[i] = castResults[j].normal;
                    break;
                }
                Dbg.Log("Failed to find a normal.", collider, normals[i] == Vector3.zero);
            }
        } else {
            float capsuleHgt = (capsule.pt1 - capsule.pt0).magnitude + capsule.r * 2f;
            float rayDist = capsuleHgt * 0.5f;
            for (int i = 0; i < numCols; i++) {
                Collider col = overlapResults[i];
                Vector3 capsuleToCol = col.transform.position - capsuleCenter;
                if (!capsuleToCol.IsZeroOrNearlyZero() &&
                    col.Raycast(
                        new Ray(capsuleCenter, capsuleToCol.normalized),
                        out RaycastHit hit,
                        rayDist
                    )) {
                    hitPts[i] = hit.point;
                    normals[i] = hit.normal;
                }
                else {
                    hitPts[i] = capsuleCenter;
                    normals[i] = Vector3.zero;
                }
            }
        }
        return numCols;
    }

    /// <summary>
    /// Performs a non-allocating sphere overlap query and calculates contact-like information for each
    /// overlapping collider. If the sphere center is outside a collider, the closest point on the collider
    /// to the sphere center is used and a raycast is performed from the sphere center to that point. If the
    /// sphere center is inside the collider, a point on the sphere surface in the direction from the
    /// collider origin to the sphere center is tested. If that point is outside the collider, a raycast is
    /// performed from that point toward the sphere center. If no raycast provides contact information, the
    /// sphere center is used as the hit point and <see cref="Vector3.zero"/> is used as the normal.
    /// For a non-convex <see cref="MeshCollider"/>, a raycast is instead performed from the sphere surface
    /// opposite the movement direction toward the sphere center. If the movement direction is close to zero
    /// or the raycast fails, the sphere center and <see cref="Vector3.zero"/> are used as the hit point
    /// and normal.<br/>
    /// NOTE: When using this method, make sure to consider cases where the normal is ~zero (raycast failed
    /// or contact information could not be calculated)!
    /// </summary>
    /// <param name="sphere">World-space sphere to query with.</param>
    /// <param name="layerMask">Layer mask used by the overlap query.</param>
    /// <param name="overlapResults">Preallocated collider result array.</param>
    /// <param name="hitPts">Preallocated array receiving the calculated hit points.</param>
    /// <param name="movDir">
    /// Used for the <see cref="Collider.Raycast"/> direction to try calculate hit point and normal if the
    /// collider is a non-convex <see cref="MeshCollider"/>. The ray starts on the sphere surface opposite
    /// the movement direction and points toward the sphere center. If its magnitude is less than or equal
    /// to 0.001f, the hit point is set to <paramref name="sphere"/> center and the normal is set to
    /// <see cref="Vector3.zero"/>.
    /// </param>
    /// <param name="normals">Preallocated array receiving the calculated normals.</param>
    /// <param name="qryTrgIxn">Specifies how trigger colliders are handled.</param>
    /// <returns>The number of colliders found by the overlap query.</returns>
    public static int OverlapSphereNonAllocWithContactInfo(
        SphereShape sphere,
        int layerMask,
        Collider[] overlapResults,
        Vector3[] hitPts,
        Vector3 movDir,
        Vector3[] normals,
        QueryTriggerInteraction qryTrgIxn = QueryTriggerInteraction.Collide
    ) {
        int numCols = Physics.OverlapSphereNonAlloc(
            sphere.center,
            sphere.r,
            overlapResults,
            layerMask,
            qryTrgIxn
        );
        for (int i = 0; i < numCols; i++) {
            Collider collider = overlapResults[i];
            if (collider is MeshCollider meshCollider && !meshCollider.convex) {
                if (!movDir.IsZeroOrNearlyZero()) {
                    Vector3 normMovDir = movDir.normalized;
                    Vector3 sphereSurfacePt = sphere.center - normMovDir * sphere.r;
                    // NOTE: We use raycast 1.5 times the diameter to allow to find closeby hitpoint that
                    // C: doesn't necessarily exist where the sphere overlaps with the collider since finding
                    // C: a specific point on the overlapped surface can be hard.
                    float rayDistance = sphere.r * 3f;
                    if (collider.Raycast(
                        new Ray(sphereSurfacePt, normMovDir),
                        out RaycastHit hit,
                        rayDistance
                    )) {
                        hitPts[i] = hit.point;
                        normals[i] = hit.normal;
                        continue;
                    }
                }
                hitPts[i] = sphere.center;
                normals[i] = Vector3.zero;
                continue;
            }
            Vector3 closestToCenter = collider.ClosestPoint(sphere.center);
            bool centerOverlapping = closestToCenter == sphere.center;
            if (!centerOverlapping) {
                Vector3 centerToClosest = closestToCenter - sphere.center;
                float distance = centerToClosest.magnitude;
                if (collider.Raycast(
                    new Ray(sphere.center, centerToClosest / distance),
                    out RaycastHit hit,
                    distance
                )) {
                    hitPts[i] = hit.point;
                    normals[i] = hit.normal;
                    continue;
                }
            }else {
                Vector3 centerToCollider = collider.transform.position - sphere.center;
                Vector3 sphereSurfaceDir = -centerToCollider;
                if (sphereSurfaceDir.sqrMagnitude <= 0)
                    sphereSurfaceDir = Vector3.up;
                else
                    sphereSurfaceDir.Normalize();
                Vector3 sphereSurfacePt = sphere.center + sphereSurfaceDir * sphere.r;
                Vector3 closestToSurface = collider.ClosestPoint(sphereSurfacePt);
                bool surfaceOverlapping = closestToSurface == sphereSurfacePt;
                if (!surfaceOverlapping) {
                    Vector3 surfaceToCenter = sphere.center - sphereSurfacePt;
                    float distance = surfaceToCenter.magnitude;
                    if (collider.Raycast(
                        new Ray(sphereSurfacePt, surfaceToCenter / distance),
                        out RaycastHit hit,
                        distance
                    )) {
                        hitPts[i] = hit.point;
                        normals[i] = hit.normal;
                        continue;
                    }
                }
            }
            hitPts[i] = sphere.center;
            normals[i] = Vector3.zero;
        }
        return numCols;
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
