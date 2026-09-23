using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// TODO: Add ICollisisionShapeHitDealer which implements the public methods. Then rename this to HitDealer_CapsuleSubstepper or similar.
public class HitDealer : MonoBehaviour {
    /// <summary>
    /// Param contains all <see cref="HitResult"/>s from one update.
    /// </summary>
    public event Action<HashSet<HitResult>> hitSomething;

    /// <summary>
    /// Capsule shapes in local space.
    /// </summary>
    [SerializeField] CapsuleShape[] capsules = { };
    [SerializeField] LayerMask capsuleLayerMask = Physics.AllLayers;
    [Tooltip("Max colliders a phys query can save during one query.")]
    [SerializeField] int maxOverlapCapsuleResults = 256;

    //// Update these before activating the hit dealer and during activation if needed.
    //[HideInInspector] public HitData hitData;

    HitEffects hitEffects;
    Transform hitSource;
    // TODO MAYBE: You could basically use pre-allocated field arrays for all containers used in HitDealer logic
    // TODO MAYBE C: for less indirection and heap allocation but what ever.
    HashSet<IHitReceiver> ignoredHitRecievers = new(4);
    bool isActive;
    PawnTeam team;

    public bool IsActive => isActive;

    // ------------------------------------------------------------------
    // Unity Callbacks
    // ------------------------------------------------------------------

    void Update() {
        if (isActive) {
            HashSet<HitResult> allHits = new(4);  
            for (int capsuleIndex = 0; capsuleIndex < capsules.Length; capsuleIndex++) {
                CapsuleShape capsule = capsules[capsuleIndex];
                // Transform capsule into world space.
                capsule.pt0 = capsule.pt0.TrfPtUnscaled(transform);
                capsule.pt1 = capsule.pt1.TrfPtUnscaled(transform);
                var hitData = new HitData(
                    hitEffects,
                    team,
                    // NOTE: This always uses HitDirMode.FromHitSourceTrfToHitReciever.
                    HitDirMode.FromHitSourceTrfToHitReciever,
                    hitSource,
                    Vector3.zero
                );
                // TODO MAYBE: Use a field for array.
                HashSet<HitResult> hitResults = TryHitHitRecievers_OverlapCapsule(
                    hitData,
                    false,
                    ignoredHitRecievers,
                    capsuleLayerMask,
                    capsule
                );
                foreach(HitResult hitResult in hitResults) {
                    allHits.Add(hitResult);
                    ignoredHitRecievers.Add(hitResult.hitReceiver);
                }
            }
            if(allHits.Count != 0)
                hitSomething?.Invoke(allHits);
        }
    }

    void OnDrawGizmos() {
        Color color = isActive ? Color.red : Color.green;
        for (int i = 0; i < capsules.Length; i++) {
            CapsuleShape capsule = capsules[i];
            DebugUtils.OnDrawGizmos_DrawCapsule(
                capsule.pt0.TrfPtUnscaled(transform),
                capsule.pt1.TrfPtUnscaled(transform),
                capsule.r,
                color
            );
        }
    }

    // ------------------------------------------------------------------
    // Public Methods
    // ------------------------------------------------------------------

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hitSource">
    /// Used to calculate hit dir if using <see cref="HitDirMode.FromHitSourceTrfToHitReciever"/>.
    /// </param>
    /// <param name="ignoreHitRecievers">
    /// You should add the recievers owned by the hitter here (if you don't want it to hit itself).
    /// </param>
    public void Activate(
        Transform hitSource,
        HitDirMode hitDirMode,
        HitEffects hitEffects,
        HashSet<IHitReceiver> ignoreHitRecievers,
        PawnTeam team
    ) {
        Dbg.LogWrn(
            $"{nameof(HitDealer)} was already active when {nameof(Activate)} was called.",
            this,
            isActive
        );
        isActive = true;
        this.hitEffects = hitEffects;
        this.hitSource = hitSource;
        this.team = team;
        ignoredHitRecievers.Clear();
        if(ignoredHitRecievers != null )
            ignoredHitRecievers.UnionWith(ignoreHitRecievers);
    }

    public void Deactivate() {
        isActive = false;
    }

    // TODO: Move to util class.
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
        HashSet<HitResult> results = new (4);
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
