using System.Collections.Generic;
using UnityEngine;

public static class HitSysUtils {
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
        Collider[] overlapCapsuleResults = new Collider[128];
        int numCols = Physics.OverlapCapsuleNonAlloc(
            wldCapsule.pt0,
            wldCapsule.pt1,
            wldCapsule.r,
            overlapCapsuleResults,
            layerMask,
            qryTrgIxn
        );
        HashSet<HitResult> results = new(4);
        for (int i = 0; i < numCols; i++) {
            IHitReceiver hitReceiver = overlapCapsuleResults[i].GetComponent<IHitReceiver>();
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
}
