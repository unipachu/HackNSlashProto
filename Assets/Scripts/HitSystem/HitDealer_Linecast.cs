using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hit dealer using a single point in world space. The point is swept from its previous
/// world position to its current world position using a linecast.
/// </summary>
// TODO: Rename to HitDealer_Raycast
public class HitDealer_Linecast : MonoBehaviour, IHitDealer {
    /// <summary>
    /// Param contains all <see cref="HitResult"/>s from one update.
    /// </summary>
    public event Action<HashSet<HitResult>> hitSomething;

    [Tooltip("Set this always to the HitReciever layer!")]
    [SerializeField] LayerMask lineLayerMask;

    Vector3 dbgPrevWldPt;
    Vector3 dbgCurWldPt;
    Vector3 prevWldPt;
    HitDirMode hitDirMode;
    HitEffects hitEffects;
    Transform hitSource;
    /// <summary>
    /// We use this to ignore hit recievers already hit during one activation.
    /// </summary>
    HashSet<IHitReceiver> ignoredHitRecievers = new(4);
    bool isActive;
    PawnTeam team;

    public bool IsActive => isActive;
    public Vector3 WldDir { get; set; }

    // ------------------------------------------------------------------
    // Unity Callbacks
    // ------------------------------------------------------------------

    void LateUpdate() {
        if (!isActive)
            return;
        HashSet<HitResult> allHits = new(4);
        Vector3 curWldPt = transform.position;
        // TODO: Where is the option for other hit modes? Wtf?
        var hitData = new HitData(
            hitEffects,
            team,
            hitDirMode,
            hitSource,
            WldDir
        );
        ProcessSweptHitPoint(allHits, curWldPt, hitData);
        dbgPrevWldPt = prevWldPt;
        dbgCurWldPt = curWldPt;
        prevWldPt = curWldPt;
        if (allHits.Count != 0)
            hitSomething?.Invoke(allHits);
    }

    void OnDrawGizmos() {
        if (isActive) {
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
        isActive = false;
    }

    void ProcessSweptHitPoint(
        HashSet<HitResult> allHits,
        Vector3 curWldPt,
        HitData hitData
    ) {
        HashSet<HitResult> hitResults = HitSysUtils.TryHitHitRecievers_Raycast(
            hitData,
            false,
            ignoredHitRecievers,
            lineLayerMask,
            prevWldPt,
            curWldPt
        );
        foreach (HitResult hitResult in hitResults) {
            allHits.Add(hitResult);
            ignoredHitRecievers.UnionWith(hitResult.allEntityHitReceivers);
        }
    }

    /// <summary>
    /// Call this when you want to activate the linecast hit dealer. This can also be
    /// called when the hit dealer is already activated - it will then act as if it
    /// started the activation from the beginning.
    /// </summary>
    /// <param name="hitSource">
    /// Used to calculate hit dir if using <see cref="HitDirMode.FromHitSourceTrfToHitReciever"/>.
    /// </param>
    /// <param name="ignoreHitRecievers">
    /// You should add the recievers owned by the hitter here (if you don't want it to hit itself).
    /// </param>
    public void ResetNActivate(
        Transform hitSource,
        HitDirMode hitDirMode,
        HitEffects hitEffects,
        HashSet<IHitReceiver> ignoreHitRecievers,
        PawnTeam team,
        Vector3 wldDir
    ) {
        Dbg.Log(
            $"{nameof(HitDealer_Linecast)} was already active when {nameof(ResetNActivate)} was called. This"
                + $" should be fine, so ignore this message!",
            this,
            isActive
        );
        isActive = true;
        this.hitDirMode = hitDirMode;
        this.hitEffects = hitEffects;
        this.hitSource = hitSource;
        this.team = team;
        WldDir = wldDir;
        ignoredHitRecievers.Clear();
        prevWldPt = transform.position;
        if (ignoreHitRecievers != null)
            ignoredHitRecievers.UnionWith(ignoreHitRecievers);
    }
}