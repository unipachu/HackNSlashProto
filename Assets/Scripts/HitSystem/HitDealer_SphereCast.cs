using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hit dealer using a single sphere. The sphere is swept from its previous world position to its current world
/// position using a sphere cast. An overlap sphere is also performed at the previous world position to detect
/// receivers already overlapping the sphere at the start of the sweep.
/// </summary>
public class HitDealer_SphereCast : MonoBehaviour, IHitDealer {
    /// <summary>
    /// Param contains all <see cref="HitResult"/>s from one update.
    /// </summary>
    public event Action<HashSet<HitResult>> hitSomething;

    /// <summary>
    /// Sphere shape in local space.
    /// </summary>
    [SerializeField] SphereShape sphere;
    [Tooltip("Set this always to the HitReciever layer!")]
    [SerializeField] LayerMask sphereLayerMask;

    SphereShape dbgPrevSphereWld;
    SphereShape dbgCurSphereWld;
    HitDirMode hitDirMode;
    HitEffects hitEffects;
    Transform srcTrf;
    /// <summary>
    /// We use this to ignore hit recievers already hit during one activation.
    /// </summary>
    HashSet<IHitReceiver> ignoredHitRecievers = new(4);
    bool isActive;
    SphereShape prevSphereWld;
    Team team;

    public bool IsActive => isActive;
    public Vector3 HitDealerMovDir { get; set; }

    // ------------------------------------------------------------------
    // Unity Callbacks
    // ------------------------------------------------------------------

    void LateUpdate() {
        if (!isActive)
            return;
        HashSet<HitResult> allHits = new(4);
        SphereShape hitDealerSphereWld =  new(sphere.center.TrfPtUnscaled(transform), sphere.r);
        // NOTE: We ALWAYS also process the start position of the current sphere sweep shape. CapsuleCast
        // C: IGNORES all colliders that are already overlapping with the start position of the cast. This can
        // C: lead to colliders moving into the path of the cast to be ignored and by doing an overlap sphere
        // C: at the cast start pos we mitigate this problem.
        ProcessOverlapHitSphere(
            allHits,
            prevSphereWld,
            HitDealerMovDir,
            hitDirMode,
            hitEffects,
            srcTrf,
            team
        );
        ProcessSweptHitSphere(
            allHits,
            hitDealerSphereWld,
            HitDealerMovDir,
            hitDirMode,
            hitEffects,
            srcTrf,
            team
        );
        dbgPrevSphereWld = prevSphereWld;
        dbgCurSphereWld = hitDealerSphereWld;
        prevSphereWld = hitDealerSphereWld;
        if (allHits.Count != 0)
            hitSomething?.Invoke(allHits);
    }

    void OnDrawGizmos() {
        if (isActive)
            DebugUtils.OnDrawGizmos_DrawCapsule(
                dbgPrevSphereWld.center,
                dbgCurSphereWld.center,
                dbgCurSphereWld.r,
                Color.red
            );
        else {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(
                sphere.center.TrfPtUnscaled(transform),
                sphere.r
            );
        }
    }

    // ------------------------------------------------------------------
    // Public Methods
    // ------------------------------------------------------------------

    public void Deactivate() {
        isActive = false;
    }
    
    void ProcessOverlapHitSphere(
        HashSet<HitResult> allHits,
        SphereShape hitDealerSphereWld,
        Vector3 hitDealerMovDir,
        HitDirMode hitDirMode,
        HitEffects hitEffects,
        Transform srcTrf,
        Team team
    ) {
        HashSet<HitResult> hitResults = HitSysUtils.TryHitHitRecievers_OverlapSphere(
            false,
            hitDealerMovDir,
            hitDirMode,
            hitEffects,
            false,
            ignoredHitRecievers,
            sphereLayerMask,
            hitDealerSphereWld,
            srcTrf,
            team
        );
        foreach (HitResult hitResult in hitResults) {
            allHits.Add(hitResult);
            ignoredHitRecievers.UnionWith(hitResult.allEntityHitReceivers);
        }
    }

    void ProcessSweptHitSphere(
        HashSet<HitResult> allHits,
        SphereShape curWldSphere,
        Vector3 hitDealerMovDir,
        HitDirMode hitDirMode,
        HitEffects hitEffects,
        Transform srcTrf,
        Team team
    ) {
        HashSet<HitResult> hitResults = HitSysUtils.TryHitHitRecievers_SphereCast(
            false,
            curWldSphere, hitDealerMovDir, hitDirMode, hitEffects, false,
            ignoredHitRecievers,
            sphereLayerMask,
            prevSphereWld,
            srcTrf,
            team
        );
        foreach (HitResult hitResult in hitResults) {
            allHits.Add(hitResult);
            ignoredHitRecievers.UnionWith(hitResult.allEntityHitReceivers);
        }
    }

    /// <summary>
    /// Call this when you want to activate the hit sphere. This can also be called when
    /// the hit sphere is already activated - it will then act as if it started the
    /// activation from the beginning.
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
        Team team,
        Vector3 wldDir
    ) {
        Dbg.Log(
            $"{nameof(HitDealer_SphereCast)} was already active when {nameof(ResetNActivate)} was called. This"
                + $" should be fine, so ignore this message!",
            this,
            isActive
        );
        isActive = true;
        this.hitDirMode = hitDirMode;
        this.hitEffects = hitEffects;
        this.srcTrf = hitSource;
        this.team = team;
        HitDealerMovDir = wldDir;
        ignoredHitRecievers.Clear();
        SphereShape curWldSphere = sphere;
        curWldSphere.center = curWldSphere.center.TrfPtUnscaled(transform);
        prevSphereWld = curWldSphere;
        if (ignoreHitRecievers != null)
            ignoredHitRecievers.UnionWith(ignoreHitRecievers);
    }
}
