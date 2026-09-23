using System;
using System.Collections.Generic;
using UnityEngine;

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

    CapsuleShape[] prevCapsuleWldPoses;
    HitEffects hitEffects;
    Transform hitSource;
    /// <summary>
    /// We use this to ignore hit recievers already hit during one activation
    /// </summary>
    // TODO MAYBE: You could basically use pre-allocated field arrays for all containers used in HitDealer logic
    // TODO MAYBE C: for less indirection and heap allocation but what ever.
    HashSet<IHitReceiver> ignoredHitRecievers = new(4);
    bool isActive;
    PawnTeam team;
    List<CapsuleShape> dbgPrevSubsteppedWldCapsules = new(4);

    public bool IsActive => isActive;

    /// <summary>
    /// If the pt0 of a hit capsule linearily moves this much away from the previous substepped hitcapsule,
    /// we make another substep hitcapsule.
    /// </summary>
    const float substepLinDist = 0.4f;
    /// <summary>
    /// If the capsule rotates around pt0 this many degrees from the prev substep capsule rotation,
    /// we make another substep hitcapsule.
    /// </summary>
    const float substepAngDist = 5; 

    // ------------------------------------------------------------------
    // Unity Callbacks
    // ------------------------------------------------------------------

    private void Awake() {
        prevCapsuleWldPoses = new CapsuleShape[capsules.Length];
    }

    void LateUpdate() {
        dbgPrevSubsteppedWldCapsules.Clear();
        if (isActive) {
            HashSet<HitResult> allHits = new(4);  
            for (int capsuleI = 0; capsuleI < capsules.Length; capsuleI++) {
                CapsuleShape capsule = capsules[capsuleI];
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
                SubstepHitCapsules(allHits, capsule, prevCapsuleWldPoses[capsuleI], hitData);
                prevCapsuleWldPoses[capsuleI] = capsule;
            }
            if (allHits.Count != 0)
                hitSomething?.Invoke(allHits);
        }
    }

    void OnDrawGizmos() {
        Color color = isActive ? Color.red : Color.green;
        if (isActive) {
            for (int i = 0; i < dbgPrevSubsteppedWldCapsules.Count; i++) {
                CapsuleShape capsule = dbgPrevSubsteppedWldCapsules[i];
                DebugUtils.OnDrawGizmos_DrawCapsule(
                    capsule.pt0,
                    capsule.pt1,
                    capsule.r,
                    color
                );
            }
        } else {
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
    }

    // ------------------------------------------------------------------
    // Public Methods
    // ------------------------------------------------------------------

    public void Deactivate() {
        isActive = false;
    }

    /// <summary>
    /// Call this when you want to activate the hit capsule. This can also be called when hit capsule is
    /// already activated - it will then act as if it started the activation from the beginning.
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
            $"{nameof(HitDealer)} was already active when {nameof(ResetNActivate)} was called. This"
                + $"should be fine, so ignore this message!",
            this,
            isActive
        );
        isActive = true;
        this.hitEffects = hitEffects;
        this.hitSource = hitSource;
        this.team = team;
        ignoredHitRecievers.Clear();
        // NOTE: We set the initial capsule world locations. During the first update of the hit dealer,
        // NOTE C: there should be no substepped capsules since previous capsule positions equal to the
        // NOTE C: current ones. (24.9.2026) 
        for (int capsuleI = 0; capsuleI < capsules.Length; capsuleI++) {
            CapsuleShape capsule = capsules[capsuleI];
            // Transform capsule into world space.
            capsule.pt0 = capsule.pt0.TrfPtUnscaled(transform);
            capsule.pt1 = capsule.pt1.TrfPtUnscaled(transform);
            prevCapsuleWldPoses[capsuleI] = capsule;
        }
        if (ignoredHitRecievers != null )
            ignoredHitRecievers.UnionWith(ignoreHitRecievers);
    }

    /// <summary>
    /// Deals hits with hit capsules, first creating intermediate capsules between
    /// <paramref name="prevWldCapsule"/> (exclusive) and <paramref name="curWldCapsule"/>, finally
    /// creating a hit capsule to <paramref name="curWldCapsule"/>.<br/>
    /// NOTE: Substeps are calcualted by interpolating <see cref="CapsuleShape.pt0"/> linearly between
    /// <paramref name="prevWldCapsule"/> and <paramref name="curWldCapsule"/>, and by using quaternion slerp
    /// to interpolate capsule rotation, <see cref="CapsuleShape.pt0"/> as a pivot, so that
    /// <see cref="CapsuleShape.pt1"/> draws an arc.
    /// </summary>
    void SubstepHitCapsules(
        HashSet<HitResult> allHits,
        CapsuleShape curWldCapsule,
        CapsuleShape prevWldCapsule,
        HitData hitData
    ) {
        Vector3 prevAxis = prevWldCapsule.pt1 - prevWldCapsule.pt0;
        Vector3 curAxis = curWldCapsule.pt1 - curWldCapsule.pt0;
        float linDist = Vector3.Distance(prevWldCapsule.pt0, curWldCapsule.pt0);
        float prevAxisLen = prevAxis.magnitude;
        float curAxisLen = curAxis.magnitude;
        float angDist = 0f;
        // If capsule axis is 0, we cannot calculate angle.
        if (prevAxisLen > Mathf.Epsilon && curAxisLen > Mathf.Epsilon)
            angDist = Vector3.Angle(prevAxis, curAxis);
        int numSubsteps = Mathf.Max(
            1,
            Mathf.CeilToInt(Mathf.Max(
                linDist / substepLinDist,
                angDist / substepAngDist
            ))
        );
        Quaternion axisRot = Quaternion.identity;
        if (prevAxisLen > Mathf.Epsilon && curAxisLen > Mathf.Epsilon)
            axisRot = Quaternion.FromToRotation(prevAxis, curAxis);
        // NOTE: The last substep is the cur pose of the capsule.
        for (int substepI = 1; substepI <= numSubsteps; substepI++) {
            float t = substepI / (float)numSubsteps;
            CapsuleShape substepCapsule = curWldCapsule;
            // Linearly interpolate pt0.
            substepCapsule.pt0 = Vector3.Lerp(
                prevWldCapsule.pt0,
                curWldCapsule.pt0,
                t
            );
            if (prevAxisLen > Mathf.Epsilon && curAxisLen > Mathf.Epsilon) {
                // Slerp the rotation from the previous capsule orientation toward the current capsule
                // orientation.
                Quaternion substepRot = Quaternion.Slerp(Quaternion.identity, axisRot, t);
                float axisLen = Mathf.Lerp(prevAxisLen, curAxisLen, t);
                Vector3 substepAxis = substepRot * prevAxis.normalized * axisLen;
                substepCapsule.pt1 = substepCapsule.pt0 + substepAxis;
            } else
                substepCapsule.pt1 = Vector3.Lerp(prevWldCapsule.pt1, curWldCapsule.pt1, t);
            substepCapsule.r = Mathf.Lerp(prevWldCapsule.r, curWldCapsule.r, t);
            dbgPrevSubsteppedWldCapsules.Add(substepCapsule);
            HashSet<HitResult> hitResults = HitSysUtils.TryHitHitRecievers_OverlapCapsule(
                hitData,
                false,
                ignoredHitRecievers,
                capsuleLayerMask,
                substepCapsule
            );
            foreach (HitResult hitResult in hitResults) {
                allHits.Add(hitResult);
                ignoredHitRecievers.Add(hitResult.hitReceiver);
            }
        }
    }
}
