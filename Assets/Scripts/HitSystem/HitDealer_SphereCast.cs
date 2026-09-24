using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hit dealer using a single sphere. The sphere is swept from its previous world position
/// to its current world position using a sphere cast. On the first tick after activation,
/// an overlap sphere is used so that receivers already overlapping the sphere are hit.
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
    SphereShape prevSphereWld;
    HitEffects hitEffects;
    Transform hitSource;
    /// <summary>
    /// We use this to ignore hit recievers already hit during one activation.
    /// </summary>
    HashSet<IHitReceiver> ignoredHitRecievers = new(4);
    bool isActive;
    bool firstTickAfterActivation;
    PawnTeam team;

    public bool IsActive => isActive;

    // ------------------------------------------------------------------
    // Unity Callbacks
    // ------------------------------------------------------------------

    void LateUpdate() {
        if (!isActive)
            return;
        HashSet<HitResult> allHits = new(4);
        SphereShape curWldSphere = sphere;
        curWldSphere.center = curWldSphere.center.TrfPtUnscaled(transform);
        var hitData = new HitData(
            hitEffects,
            team,
            HitDirMode.FromHitSourceTrfToHitReciever,
            hitSource,
            Vector3.zero
        );
        if (firstTickAfterActivation) {
            ProcessInitialHitSphere(allHits, curWldSphere, hitData);
            firstTickAfterActivation = false;
        } else
            ProcessSweptHitSphere(allHits, curWldSphere, hitData);
        dbgPrevSphereWld = prevSphereWld;
        dbgCurSphereWld = curWldSphere;
        prevSphereWld = curWldSphere;
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
    
    void ProcessInitialHitSphere(
        HashSet<HitResult> allHits,
        SphereShape curWldSphere,
        HitData hitData
    ) {
        HashSet<HitResult> hitResults = HitSysUtils.TryHitHitRecievers_OverlapSphere(
            hitData,
            false,
            ignoredHitRecievers,
            sphereLayerMask,
            curWldSphere
        );
        foreach (HitResult hitResult in hitResults) {
            allHits.Add(hitResult);
            ignoredHitRecievers.UnionWith(hitResult.allEntityHitReceivers);
        }
    }

    void ProcessSweptHitSphere(
        HashSet<HitResult> allHits,
        SphereShape curWldSphere,
        HitData hitData
    ) {
        HashSet<HitResult> hitResults = HitSysUtils.TryHitHitRecievers_SphereCast(
            hitData,
            false,
            ignoredHitRecievers,
            sphereLayerMask,
            prevSphereWld,
            curWldSphere
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
        PawnTeam team
    ) {
        Dbg.Log(
            $"{nameof(HitDealer_SphereCast)} was already active when {nameof(ResetNActivate)} was called. This"
                + $" should be fine, so ignore this message!",
            this,
            isActive
        );
        isActive = true;
        firstTickAfterActivation = true;
        this.hitEffects = hitEffects;
        this.hitSource = hitSource;
        this.team = team;
        ignoredHitRecievers.Clear();
        SphereShape curWldSphere = sphere;
        curWldSphere.center = curWldSphere.center.TrfPtUnscaled(transform);
        prevSphereWld = curWldSphere;
        if (ignoreHitRecievers != null)
            ignoredHitRecievers.UnionWith(ignoreHitRecievers);
    }
}
