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
    /// collider position using half the capsule height as the maximum distance. If no cast provides
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
            // NOTE: Unity only has Collider.Raycast, there's no Collider.CapsuleCast for some reason, so we
            // C: unfortunately have to use the more expensive Physics.CapsuleCastNonAlloc.
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
                Collider col = overlapResults[i];
                for (int j = 0; j < numCastHits; j++) {
                    if (castResults[j].collider != col)
                        continue;
                    hitPts[i] = castResults[j].point;
                    normals[i] = castResults[j].normal;
                    break;
                }
                //Dbg.Log(
                //    $"Overlap capsule with movDir {movDir} failed to find a normal for {col.name}.",
                //    col,
                //    normals[i] == Vector3.zero
                //);
            }
        } else {
            // We use half capsule height for the raycast.
            float halfCapsuleHgt = (capsule.pt1 - capsule.pt0).magnitude / 2f + capsule.r;
            for (int i = 0; i < numCols; i++) {
                Collider col = overlapResults[i];
                Vector3 capsuleToCol = col.transform.position - capsuleCenter;
                bool hasRayDir = !capsuleToCol.IsZeroOrNearlyZero();
                bool rayHit = false;
                if (hasRayDir) {
                    rayHit = col.Raycast(
                        new Ray(capsuleCenter, capsuleToCol.normalized),
                        out RaycastHit hit,
                        halfCapsuleHgt
                    );
                    if (rayHit) {
                        hitPts[i] = hit.point;
                        normals[i] = hit.normal;
                        continue;
                    }
                }
                hitPts[i] = capsuleCenter;
                normals[i] = Vector3.zero;
                //Dbg.Log(
                //    $"Zero-movDir contact failed for {col.name}. "
                //    + $"hasRayDir={hasRayDir}, rayHit={rayHit}, "
                //    + $"capsuleCenter={capsuleCenter}, colliderPos={col.transform.position}, "
                //    + $"rayDist={halfCapsuleHgt}"
                //    + $"capsuleToCol{capsuleToCol}",
                //    col
                //);
            }
        }
        //DbgGizmoFactory.DrawVectorGizmos(hitPts, normals, numCols);
        return numCols;
    }

    /// <summary>
    /// Performs a non-allocating point overlap query and calculates contact-like information for each
    /// overlapping collider. If the movement direction is not
    /// <see cref="MathUtils.IsZeroOrNearlyZero"/>, a ray is cast from 1.5 units opposite the movement
    /// direction toward the point against each overlapping collider. If a raycast hits, its hit point and
    /// normal are returned. If the movement direction is close to zero, a raycast is performed from the
    /// point toward the collider position. If no raycast provides contact information, the queried point
    /// is used as the hit point and <see cref="Vector3.zero"/> is used as the normal.
    /// </summary>
    /// <param name="pt">World-space point to query.</param>
    /// <param name="layerMask">Layer mask used by the overlap and raycast queries.</param>
    /// <param name="overlapResults">Preallocated collider result array.</param>
    /// <param name="hitPts">Preallocated array receiving the calculated hit points.</param>
    /// <param name="movDir">Movement direction used to determine the raycast direction.</param>
    /// <param name="normals">Preallocated array receiving the calculated normals.</param>
    /// <param name="qryTrgIxn">Specifies how trigger colliders are handled.</param>
    /// <returns>The number of colliders found by the overlap query.</returns>
    public static int OverlapPtNonAllocWithContactInfo(
        Vector3 pt,
        int layerMask,
        Collider[] overlapResults,
        Vector3[] hitPts,
        Vector3 movDir,
        Vector3[] normals,
        QueryTriggerInteraction qryTrgIxn = QueryTriggerInteraction.Collide
    ) {
        int numCols = Physics.OverlapSphereNonAlloc(
            pt,
            0f,
            overlapResults,
            layerMask,
            qryTrgIxn
        );
        if (!movDir.IsZeroOrNearlyZero()) {
            Vector3 normMovDir = movDir.normalized;
            Vector3 rayStartPt = pt - normMovDir * 1.5f;
            for (int i = 0; i < numCols; i++) {
                hitPts[i] = pt;
                normals[i] = Vector3.zero;
                Collider col = overlapResults[i];
                if (col.Raycast(
                    new Ray(rayStartPt, normMovDir),
                    out RaycastHit hit,
                    1.5f
                )) {
                    hitPts[i] = hit.point;
                    normals[i] = hit.normal;
                }
            }
        }
        else {
            for (int i = 0; i < numCols; i++) {
                Collider col = overlapResults[i];
                Vector3 ptToCol = col.transform.position - pt;
                bool hasRayDir = !ptToCol.IsZeroOrNearlyZero();
                bool rayHit = false;
                if (hasRayDir) {
                    rayHit = col.Raycast(
                        new Ray(pt, ptToCol.normalized),
                        out RaycastHit hit,
                        ptToCol.magnitude
                    );
                    if (rayHit) {
                        hitPts[i] = hit.point;
                        normals[i] = hit.normal;
                        continue;
                    }
                }
                hitPts[i] = pt;
                normals[i] = Vector3.zero;
                Dbg.Log(
                    $"Zero-movDir point contact failed for {col.name}. "
                    + $"hasRayDir={hasRayDir}, rayHit={rayHit}, "
                    + $"pt={pt}, colliderPos={col.transform.position}, "
                    + $"rayDist={ptToCol.magnitude}, "
                    + $"ptToCol={ptToCol}",
                    col
                );
            }
        }
        //DbgGizmoFactory.DrawVectorGizmos(hitPts, normals, numCols);
        return numCols;
    }

    /// <summary>
    /// Performs a non-allocating sphere overlap query and calculates contact-like information for each
    /// overlapping collider. If the movement direction is not
    /// <see cref="MathUtils.IsZeroOrNearlyZero"/>, the sphere is moved 1.5 units opposite the movement
    /// direction and cast back toward its current position. If a sphere cast hit matches an overlapping
    /// collider, its hit point and normal are returned. If the movement direction is close to zero, a
    /// raycast is instead performed from the sphere center toward the collider position using the sphere
    /// radius as the maximum distance. If no cast provides contact information, the sphere center is
    /// used as the hit point and <see cref="Vector3.zero"/> is used as the normal.
    /// </summary>
    /// <param name="sphere">World-space sphere to query with.</param>
    /// <param name="layerMask">Layer mask used by the overlap and sphere cast queries.</param>
    /// <param name="overlapResults">Preallocated collider result array.</param>
    /// <param name="hitPts">Preallocated array receiving the calculated hit points.</param>
    /// <param name="movDir">
    /// Movement direction used to perform the sphere cast. If its magnitude is less than or equal to
    /// 0.001f, the sphere cast is skipped and a center-based raycast is used instead.
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
        if (!movDir.IsZeroOrNearlyZero()) {
            Vector3 normMovDir = movDir.normalized;
            Vector3 castOffset = -normMovDir * 1.5f;
            Vector3 castCenter = sphere.center + castOffset;
            // NOTE: Cast results can have more hits than the overlap results. Here we make the assumption that
            // C: overlapResults.Length has enough room for both query results.
            // TODO MINOR: This could be provided by the method caller to avoid needless allocation.
            RaycastHit[] castResults = new RaycastHit[overlapResults.Length];
            // NOTE: Unity only has Collider.Raycast, there's no Collider.SphereCast for some reason, so we
            // C: unfortunately have to use the more expensive Physics.SphereCastNonAlloc. 
            int numCastHits = Physics.SphereCastNonAlloc(
                castCenter,
                sphere.r,
                normMovDir,
                castResults,
                1.5f, // This is an arbitrary number. We cast 1.5 units away from the overlap sphere.
                layerMask,
                qryTrgIxn
            );
            for (int i = 0; i < numCols; i++) {
                hitPts[i] = sphere.center;
                normals[i] = Vector3.zero;
                Collider col = overlapResults[i];
                for (int j = 0; j < numCastHits; j++) {
                    if (castResults[j].collider != col)
                        continue;
                    hitPts[i] = castResults[j].point;
                    normals[i] = castResults[j].normal;
                    break;
                }
                //Dbg.Log(
                //    $"Overlap sphere with movDir {movDir} failed to find a normal for {col.name}.",
                //    col,
                //    normals[i] == Vector3.zero
                //);
            }
        }
        else {
            float rayDist = sphere.r;
            for (int i = 0; i < numCols; i++) {
                Collider col = overlapResults[i];
                Vector3 sphereToCol = col.transform.position - sphere.center;
                bool hasRayDir = !sphereToCol.IsZeroOrNearlyZero();
                bool rayHit = false;
                if (hasRayDir) {
                    rayHit = col.Raycast(
                        new Ray(sphere.center, sphereToCol.normalized),
                        out RaycastHit hit,
                        rayDist
                    );
                    if (rayHit) {
                        hitPts[i] = hit.point;
                        normals[i] = hit.normal;
                        continue;
                    }
                }
                hitPts[i] = sphere.center;
                normals[i] = Vector3.zero;
                //Dbg.Log(
                //    $"Zero-movDir contact failed for {col.name}. "
                //    + $"hasRayDir={hasRayDir}, rayHit={rayHit}, "
                //    + $"sphereCenter={sphere.center}, colliderPos={col.transform.position}, "
                //    + $"rayDist={rayDist}, "
                //    + $"sphereToCol={sphereToCol}",
                //    col
                //);
            }
        }
        //DbgGizmoFactory.DrawVectorGizmos(hitPts, normals, numCols);
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
