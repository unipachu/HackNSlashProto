using System.Collections.Generic;
using UnityEngine;

public static class HitSysUtils {
    const int overlapResultsArraySize = 256;

    static HashSet<HitResult> ProcessOverlapShapeResults(
        bool allowFriendlyFire,
        Vector3 hitDealerMovDir,
        HitDirMode hitDirMode,
        HitEffects hitEffects,
        bool hitMaxOnce,
        Vector3[] hitPt,
        HashSet<IHitReceiver> ignoreHitRecievers,
        int numCols,
        Collider[] overlapShapeResults,
        Vector3[] separationDir,
        Transform srcTrf,
        Team team
    ) {
        HashSet<HitResult> results = new(4);
        for (int i = 0; i < numCols; i++) {
            IHitReceiver hitReceiver = overlapShapeResults[i].GetComponent<IHitReceiver>();
            if (!TryProcessHitReceiver(
                allowFriendlyFire,
                hitDealerMovDir,
                hitDirMode,
                hitEffects,
                hitPt[i],
                hitReceiver,
                out HitResult hitResult, 
                ignoreHitRecievers,
                separationDir[i],
                srcTrf,
                team
            ))
                continue;
            results.Add(hitResult);
            if (hitMaxOnce)
                break;
        }
        return results;
    }

    /// <param name="hitMaxOnce">
    /// Only hit first eligible hit reciever (ignore other hit recievers in the collision query result.
    /// </param>
    /// <param name="ignoreHitRecievers">
    /// All hit recievers we do not want to hit, e.g. the hit recievers of the hitter.
    /// </param>
    /// <param name="allowFriendlyFire">
    /// Allow hitting hit recievers of the same <see cref="Team"/>.
    /// </param>
    static HashSet<HitResult> ProcessCastResults(
        bool allowFriendlyFire,
        RaycastHit[] castResults,
        Vector3 hitDealerMovDir,
        HitDirMode hitDirMode,
        HitEffects hitEffects,
        bool hitMaxOnce,
        HashSet<IHitReceiver> ignoreHitRecievers,
        int numHits,
        Transform srcTrf,
        Team team
    ) {
        HashSet<HitResult> results = new(4);
        for (int i = 0; i < numHits; i++) {
            if (!TryProcessHitReceiver(
                allowFriendlyFire,
                hitDealerMovDir,
                hitDirMode,
                hitEffects,
                castResults[i].point,
                castResults[i].collider.GetComponent<IHitReceiver>(), 
                out HitResult hitResult,
                ignoreHitRecievers,
                castResults[i].normal,
                srcTrf,
                team
            ))
                continue;
            results.Add(hitResult);
            if (hitMaxOnce)
                break;
        }
        return results;
    }

    /// <summary>
    /// NOTE: Uses the tip of the capsule closest to pt1 to calculate hit point (with Collider.ClosestPoint)
    /// and separation direction (from the capsule tip to the closest point). This does not give as nice
    /// results as using raycast hit dealers but I don't know any better solution (except maybe trying to
    /// use raycasts, but that also isn't very neat).
    /// </summary>
    public static HashSet<HitResult> TryHitHitRecievers_OverlapCapsule(
        bool allowFriendlyFire,
        CapsuleShape hitDealerCapsuleWld,
        Vector3 hitDealerMovDir,
        HitDirMode hitDirMode,
        HitEffects hitEffects,
        bool hitMaxOnce,
        HashSet<IHitReceiver> ignoreHitRecievers,
        int layerMask,
        Transform srcTrf,
        Team team,
        QueryTriggerInteraction qryTrgIxn = QueryTriggerInteraction.Collide
    ) {
        Collider[] overlapCapsuleResults = new Collider[overlapResultsArraySize];
        Vector3[] hitPts = new Vector3[overlapResultsArraySize];
        Vector3[] separationDirs = new Vector3[overlapResultsArraySize];
        int numCols = PhysUtils.OverlapCapsuleNonAllocWithContactInfo(
            hitDealerCapsuleWld,
            layerMask,
            overlapCapsuleResults,
            hitPts,
            hitDealerMovDir,
            separationDirs,
            qryTrgIxn
        );
        return ProcessOverlapShapeResults(
            allowFriendlyFire,
            hitDealerMovDir,
            hitDirMode,
            hitEffects,
            hitMaxOnce,
            hitPts,
            ignoreHitRecievers,
            numCols,
            overlapCapsuleResults,
            separationDirs,
            srcTrf,
            team
        );
    }


    public static HashSet<HitResult> TryHitHitRecievers_OverlapSphere(
        bool allowFriendlyFire,
        Vector3 hitDealerMovDir,
        HitDirMode hitDirMode,
        HitEffects hitEffects,
        bool hitMaxOnce,
        HashSet<IHitReceiver> ignoreHitRecievers,
        int layerMask,
        SphereShape hitDealerSphereWld,
        Transform srcTrf,
        Team team,
        QueryTriggerInteraction qryTrgIxn = QueryTriggerInteraction.Collide
    ) {
        Collider[] overlapSphereResults = new Collider[overlapResultsArraySize];
        Vector3[] hitPts = new Vector3[overlapResultsArraySize];
        Vector3[] separationDirs = new Vector3[overlapResultsArraySize];
        int numCols = PhysUtils.OverlapSphereNonAllocWithContactInfo(
            hitDealerSphereWld,
            layerMask,
            overlapSphereResults,
            hitPts,
            hitDealerMovDir,
            separationDirs,
            qryTrgIxn
        );
        return ProcessOverlapShapeResults(
            allowFriendlyFire,
            hitDealerMovDir,
            hitDirMode,
            hitEffects,
            hitMaxOnce,
            hitPts,
            ignoreHitRecievers,
            numCols,
            overlapSphereResults,
            separationDirs,
            srcTrf,
            team
        );
    }

    /// <summary>
    /// Uses a <see cref="Physics.SphereCastNonAlloc"/> from <paramref name="prevWldSphere"/> to <paramref name="curWldSphere"/> to try and hit <see cref="IHitReceiver"/>s.
    /// </summary>
    /// <param name="hitMaxOnce">
    /// Should we only hit first found eligible <see cref="IHitReceiver"/>?
    /// </param>
    /// <param name="allowFriendlyFire">
    /// Should allow hits that would be otherwise permitted by <see cref="Team"/> setup?
    /// </param>
    public static HashSet<HitResult> TryHitHitRecievers_SphereCast(
        bool allowFriendlyFire,
        SphereShape curWldSphere,
        Vector3 hitDealerMovDir,
        HitDirMode hitDirMode,
        HitEffects hitEffects,
        bool hitMaxOnce,
        HashSet<IHitReceiver> ignoreHitRecievers,
        int layerMask,
        SphereShape prevWldSphere,
        Transform srcTrf,
        Team team,
        QueryTriggerInteraction qryTrgIxn = QueryTriggerInteraction.Collide
    ) {
        Vector3 castDir = curWldSphere.center - prevWldSphere.center;
        float castDist = castDir.magnitude;
        if (castDist <= Mathf.Epsilon)
            return new HashSet<HitResult>(4);
        castDir /= castDist;
        RaycastHit[] sphereCastResults = new RaycastHit[overlapResultsArraySize];
        int numHits = Physics.SphereCastNonAlloc(
            prevWldSphere.center,
            curWldSphere.r,
            castDir,
            sphereCastResults,
            castDist,
            layerMask,
            qryTrgIxn
        );
        return ProcessCastResults(
            allowFriendlyFire,
            sphereCastResults,
            hitDealerMovDir,
            hitDirMode,
            hitEffects,
            hitMaxOnce,
            ignoreHitRecievers,
            numHits,
            srcTrf,
            team
        );
    }

    /// <summary>
    /// Uses a <see cref="Physics.RaycastNonAlloc"/> from <paramref name="prevWldPt"/> to
    /// <paramref name="curWldPt"/> to try and hit <see cref="IHitReceiver"/>s.
    /// </summary>
    /// <param name="hitMaxOnce">
    /// Should we only hit first found eligible <see cref="IHitReceiver"/>?
    /// </param>
    /// <param name="allowFriendlyFire">
    /// Should allow hits that would be otherwise permitted by <see cref="Team"/> setup?
    /// </param>
    public static HashSet<HitResult> TryHitHitRecievers_Raycast(
        bool allowFriendlyFire,
        Vector3 curWldPt,
        Vector3 hitDealerMovDir,
        HitDirMode hitDirMode,
        HitEffects hitEffects,
        bool hitMaxOnce,
        HashSet<IHitReceiver> ignoreHitRecievers,
        int layerMask,
        Vector3 prevWldPt,
        Transform srcTrf,
        Team team,
        QueryTriggerInteraction qryTrgIxn = QueryTriggerInteraction.Collide
    ) {
        Vector3 castDir = curWldPt - prevWldPt;
        float castDist = castDir.magnitude;
        if (castDist <= Mathf.Epsilon)
            return new HashSet<HitResult>(4);
        // Normalize dir. Not sure if necessary for the raycast but jic.
        castDir /= castDist;
        RaycastHit[] raycastResults = new RaycastHit[overlapResultsArraySize];
        int numHits = Physics.RaycastNonAlloc(
            prevWldPt,
            castDir,
            raycastResults,
            castDist,
            layerMask,
            qryTrgIxn
        );
        return ProcessCastResults(
            allowFriendlyFire,
            raycastResults,
            hitDealerMovDir,
            hitDirMode,
            hitEffects,
            hitMaxOnce,
            ignoreHitRecievers,
            numHits,
            srcTrf,
            team
        );
    }

    static bool TryProcessHitReceiver(
        bool allowFriendlyFire,
        Vector3 hitDealerMovDir,
        HitDirMode hitDirMode,
        HitEffects hitEffects,
        Vector3 hitPt,
        IHitReceiver hitReceiver,
        out HitResult hitResult,
        HashSet<IHitReceiver> ignoreHitRecievers,
        Vector3 separationDir,
        Transform srcTrf,
        Team team
    ) {
        hitResult = default;
        if (hitReceiver == null)
            return false;
        if (hitReceiver.IgnoreAllHits)
            return false;
        Team receiverTeam = hitReceiver.GetTeam;
        if (receiverTeam == Team.FriendToAll)
            return false;
        if (
            receiverTeam != Team.EnemyToAll
                && receiverTeam == team
                && !allowFriendlyFire
        )
            return false;
        if (ignoreHitRecievers.Contains(hitReceiver))
            return false;
        // Deal hit.
        HitData hitData = new(hitDealerMovDir, hitDirMode, hitEffects, hitPt,separationDir, srcTrf, team);
        hitResult = hitReceiver.ReceiveHit(hitData);
        return true;
    }
}
