using System.Collections.Generic;
using UnityEngine;

public static class HitSysUtils {
    const int overlapResultsArraySize = 256;

    public static HitResult DealHit(HitData hitData, IHitReceiver hitReceiver)
        => hitReceiver.ReceiveHit(hitData);

    /// <summary>
    /// Uses a <see cref="Physics.OverlapCapsuleNonAlloc"/> to try and hit <see cref="IHitReceiver"/>s.
    /// </summary>
    /// <param name="hitMaxOnce">
    /// Should we only hit first found eligible <see cref="IHitReceiver"/>?
    /// </param>
    /// <param name="allowFriendlyFire">
    /// Should allow hits that would be otherwise premitted by <see cref="PawnTeam"/> setup?
    /// </param>
    public static HashSet<HitResult> TryHitHitRecievers_OverlapCapsule(
        HitData hitData,
        bool hitMaxOnce,
        HashSet<IHitReceiver> ignoreHitRecievers,
        int layerMask,
        CapsuleShape wldCapsule,
        bool allowFriendlyFire = false,
        QueryTriggerInteraction qryTrgIxn = QueryTriggerInteraction.Collide
    ) {
        Collider[] overlapCapsuleResults = new Collider[overlapResultsArraySize];
        int numCols = Physics.OverlapCapsuleNonAlloc(
            wldCapsule.pt0,
            wldCapsule.pt1,
            wldCapsule.r,
            overlapCapsuleResults,
            layerMask,
            qryTrgIxn
        );
        return ProcessCollisionQueryResults(
            hitData,
            hitMaxOnce,
            ignoreHitRecievers,
            allowFriendlyFire,
            overlapCapsuleResults,
            numCols
        );
    }

    static HashSet<HitResult> ProcessCollisionQueryResults(
        HitData hitData,
        bool hitMaxOnce,
        HashSet<IHitReceiver> ignoreHitRecievers,
        bool allowFriendlyFire,
        Collider[] overlapShapeResults,
        int numCols
    ) {
        HashSet<HitResult> results = new(4);
        for (int i = 0; i < numCols; i++) {
            IHitReceiver hitReceiver = overlapShapeResults[i].GetComponent<IHitReceiver>();
            if (hitReceiver == null)
                continue;
            if (hitReceiver.IgnoreAllHits)
                continue;
            PawnTeam receiverTeam = hitReceiver.GetTeam;
            if (receiverTeam == PawnTeam.FriendToAll)
                continue;
            if (
                receiverTeam != PawnTeam.EnemyToAll
                    && receiverTeam == hitData.team
                    && !allowFriendlyFire
            )
                continue;
            if (ignoreHitRecievers.Contains(hitReceiver))
                continue;
            results.Add(DealHit(hitData, hitReceiver));
            if (hitMaxOnce)
                break;
        }
        return results;
    }

    static HashSet<HitResult> ProcessCollisionQueryResults(
        HitData hitData,
        bool hitMaxOnce,
        HashSet<IHitReceiver> ignoreHitRecievers,
        bool allowFriendlyFire,
        RaycastHit[] castResults,
        int numHits
    ) {
        HashSet<HitResult> results = new(4);
        for (int i = 0; i < numHits; i++) {
            IHitReceiver hitReceiver = castResults[i].collider.GetComponent<IHitReceiver>();
            if (hitReceiver == null)
                continue;
            if (hitReceiver.IgnoreAllHits)
                continue;
            PawnTeam receiverTeam = hitReceiver.GetTeam;
            if (receiverTeam == PawnTeam.FriendToAll)
                continue;
            if (
                receiverTeam != PawnTeam.EnemyToAll
                    && receiverTeam == hitData.team
                    && !allowFriendlyFire
            )
                continue;
            if (ignoreHitRecievers.Contains(hitReceiver))
                continue;
            results.Add(DealHit(hitData, hitReceiver));
            if (hitMaxOnce)
                break;
        }
        return results;
    }

    /// <summary>
    /// Uses a <see cref="Physics.OverlapSphereNonAlloc"/> to try and hit <see cref="IHitReceiver"/>s.
    /// </summary>
    /// <param name="hitMaxOnce">
    /// Should we only hit first found eligible <see cref="IHitReceiver"/>?
    /// </param>
    /// <param name="allowFriendlyFire">
    /// Should allow hits that would be otherwise permitted by <see cref="PawnTeam"/> setup?
    /// </param>
    public static HashSet<HitResult> TryHitHitRecievers_OverlapSphere(
        HitData hitData,
        bool hitMaxOnce,
        HashSet<IHitReceiver> ignoreHitRecievers,
        int layerMask,
        SphereShape wldSphere,
        bool allowFriendlyFire = false,
        QueryTriggerInteraction qryTrgIxn = QueryTriggerInteraction.Collide
    ) {
        Collider[] overlapSphereResults = new Collider[overlapResultsArraySize];
        int numCols = Physics.OverlapSphereNonAlloc(
            wldSphere.center,
            wldSphere.r,
            overlapSphereResults,
            layerMask,
            qryTrgIxn
        );
        return ProcessCollisionQueryResults(
            hitData,
            hitMaxOnce,
            ignoreHitRecievers,
            allowFriendlyFire,
            overlapSphereResults,
            numCols
        );
    }

    /// <summary>
    /// Uses a <see cref="Physics.SphereCastNonAlloc"/> from <paramref name="prevWldSphere"/> to <paramref name="curWldSphere"/> to try and hit <see cref="IHitReceiver"/>s.
    /// </summary>
    /// <param name="hitMaxOnce">
    /// Should we only hit first found eligible <see cref="IHitReceiver"/>?
    /// </param>
    /// <param name="allowFriendlyFire">
    /// Should allow hits that would be otherwise permitted by <see cref="PawnTeam"/> setup?
    /// </param>
    public static HashSet<HitResult> TryHitHitRecievers_SphereCast(
        HitData hitData,
        bool hitMaxOnce,
        HashSet<IHitReceiver> ignoreHitRecievers,
        int layerMask,
        SphereShape prevWldSphere,
        SphereShape curWldSphere,
        bool allowFriendlyFire = false,
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
        return ProcessCollisionQueryResults(
            hitData,
            hitMaxOnce,
            ignoreHitRecievers,
            allowFriendlyFire,
            sphereCastResults,
            numHits
        );
    }
}
