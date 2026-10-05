using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hit dealer using a single point in world space. The point is swept from its previous
/// world position to its current world position using a raycast.
/// </summary>
public class HitDealer_Raycast : MonoBehaviour, IHitDealer {
    /// <summary>
    /// Param contains all <see cref="HitResult"/>s from one update.
    /// </summary>
    public event Action<HashSet<HitResult>> hitSomething;

    [Tooltip("Set this always to the HitReciever layer!")]
    [SerializeField] LayerMask rayLayerMask;

    Vector3 dbgPrevWldPt;
    Vector3 dbgCurWldPt;
    HitDirMode hitDirMode;
    HitEffects hitEffects;
    /// <summary>
    /// We use this to ignore hit recievers already hit during one activation.
    /// </summary>
    HashSet<IHitReceiver> ignoredHitRecievers = new(4);
    Vector3 prevWldPt;
    Transform srcTrf;
    Team team;

    public bool IsActive { get; private set; }
    public Vector3 HitDealerMovDir { get; set; }

    // ------------------------------------------------------------------
    // Unity Callbacks
    // ------------------------------------------------------------------

    void LateUpdate() {
        if (!IsActive)
            return;
        HashSet<HitResult> allHits = new(4);
        Vector3 curWldPt = transform.position;
        // NOTE: We ALWAYS also process the start position of the current raycast sweep.
        // Raycast ignores colliders that the ray starts inside, so this overlap catches receivers
        // already overlapping the point at the start of the sweep.
        ProcessOverlapHitPt(
            allHits,
            prevWldPt,
            HitDealerMovDir,
            hitDirMode,
            hitEffects,
            ignoredHitRecievers,
            srcTrf,
            team
        );
        ProcessSweptHitPoint(
            allHits,
            curWldPt,
            HitDealerMovDir,
            hitDirMode,
            hitEffects,
            ignoredHitRecievers,
            srcTrf,
            team
        );
        dbgPrevWldPt = prevWldPt;
        dbgCurWldPt = curWldPt;
        prevWldPt = curWldPt;
        if (allHits.Count != 0)
            hitSomething?.Invoke(allHits);
    }

    void OnDrawGizmos() {
        if (IsActive) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(dbgPrevWldPt, dbgCurWldPt);
            Gizmos.DrawSphere(dbgCurWldPt, 0.02f);
        } else {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 0.02f);
        }
    }

    // ------------------------------------------------------------------
    // Public Methods
    // ------------------------------------------------------------------

    public void Deactivate() {
        IsActive = false;
    }

    void ProcessOverlapHitPt(
        HashSet<HitResult> allHits,
        Vector3 hitDealerPtWld,
        Vector3 hitDealerMovDir,
        HitDirMode hitDirMode,
        HitEffects hitEffects,
        HashSet<IHitReceiver> ignoreHitRecievers,
        Transform srcTrf,
        Team team
    ) {
        HashSet<HitResult> hitResults = HitSysUtils.TryHitHitRecievers_OverlapPt(
            false,
            hitDealerMovDir,
            hitDirMode,
            hitEffects,
            false,
            ignoreHitRecievers,
            rayLayerMask,
            hitDealerPtWld,
            srcTrf,
            team
        );
        HitSysUtils.SaveHitResults(allHits, hitResults, ignoredHitRecievers);
    }

    void ProcessSweptHitPoint(
        HashSet<HitResult> allHits,
        Vector3 curWldPt,
        Vector3 hitDealerMovDir,
        HitDirMode hitDirMode,
        HitEffects hitEffects,
        HashSet<IHitReceiver> ignoreHitRecievers,
        Transform srcTrf,
        Team team
    ) {
        HashSet<HitResult> hitResults = HitSysUtils.TryHitHitRecievers_Raycast(
            false,
            curWldPt,
            hitDealerMovDir,
            hitDirMode,
            hitEffects,
            false,
            ignoreHitRecievers,
            rayLayerMask,
            prevWldPt,
            srcTrf,
            team
        );
        HitSysUtils.SaveHitResults(allHits, hitResults, ignoredHitRecievers);
    }

    /// <summary>
    /// Call this when you want to activate the raycast hit dealer. This can also be
    /// called when the hit dealer is already activated - it will then act as if it
    /// started the activation from the beginning.
    /// </summary>
    /// <param name="srcTrf">
    /// Used to calculate hit dir if using <see cref="HitDirMode.FromHitSourceTrfToHitReciever"/>.
    /// </param>
    /// <param name="ignoreHitRecievers">
    /// You should add the recievers owned by the hitter here (if you don't want it to hit itself).
    /// </param>
    public void ResetNActivate(
        Transform srcTrf,
        HitDirMode hitDirMode,
        HitEffects hitEffects,
        HashSet<IHitReceiver> ignoreHitRecievers,
        Team team,
        Vector3 wldDir
    ) {
        Dbg.Log(
            $"{nameof(HitDealer_Raycast)} was already active when {nameof(ResetNActivate)} was called. This"
                + $" should be fine, so ignore this message!",
            this,
            IsActive
        );
        IsActive = true;
        this.hitDirMode = hitDirMode;
        this.hitEffects = hitEffects;
        this.srcTrf = srcTrf;
        this.team = team;
        HitDealerMovDir = wldDir;
        ignoredHitRecievers.Clear();
        prevWldPt = transform.position;
        if (ignoreHitRecievers != null)
            ignoredHitRecievers.UnionWith(ignoreHitRecievers);
    }
}