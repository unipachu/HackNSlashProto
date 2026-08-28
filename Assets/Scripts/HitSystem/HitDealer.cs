using System;
using System.Collections.Generic;
using UnityEngine;

public class HitDealer : MonoBehaviour {
    public event Action<HitResult> hitReceiverHit;

    [SerializeField] CapsuleShape[] capsules = { };
    [SerializeField] LayerMask capsuleLayerMask = Physics.AllLayers;
    [Tooltip("Max colliders a phys query can save during one query.")]
    [SerializeField] int maxColliders = 100;

    // Update these before activating the hit dealer and during activation if needed.
    [HideInInspector] public AtkData atkData;
    [HideInInspector] public Vector3 hitWldDir;

    bool isActive;
    Collider[] overlapCapsuleResults;
    HashSet<HitReceiver> hitReceiversHitDuringLastActivation = new();

    public bool IsActive => isActive;

    // ------------------------------------------------------------------
    // Unity Callbacks
    // ------------------------------------------------------------------

    void Awake() {
        overlapCapsuleResults = new Collider[maxColliders];
    }

    void Update() {
        if (isActive) {
            TryHitAllOverlappingHitRecievers(
                capsuleLayerMask
            );
        }
    }

    void OnDrawGizmos() {
        Color color = isActive ? Color.red : Color.green;
        for (int i = 0; i < capsules.Length; i++) {
            CapsuleShape capsule = capsules[i];
            DebugUtils.OnDrawGizmos_DrawCapsule(
                TrfMathUtils.TrfPtUnscaled(transform, capsule.pt0),
                TrfMathUtils.TrfPtUnscaled(transform, capsule.pt1),
                capsule.r,
                color
            );
        }
    }

    // ------------------------------------------------------------------
    // Public Methods
    // ------------------------------------------------------------------

    public void Activate() {
        isActive = true;
        hitReceiversHitDuringLastActivation.Clear();
    }

    public void Deactivate() {
        isActive = false;
    }

    public bool TryDealHit(HitReceiver hitReceiver, HitData hitData) {
        if (hitReceiversHitDuringLastActivation.Contains(hitReceiver))
            return false;
        HitResult hitResult = hitReceiver.ReceiveHit(this, hitData);
        hitReceiversHitDuringLastActivation.Add(hitReceiver);
        hitReceiverHit?.Invoke(hitResult);
        return true;
    }

    // TODO: Make a version of this which uses capsule cast from previous location to current location
    // TODO C: instead. This will allow the weapon to make fast linear movements without going through
    // TODO C: enemies. You need to save the previous capsule world locations for the sweeps.
    // TODO C: Then you can choose between OverlapCapsule or CapsuleCast or use both at the same time!
    public void TryHitAllOverlappingHitRecievers(
        int layerMask,
        QueryTriggerInteraction qryTrgIxn = QueryTriggerInteraction.Collide
    ) {
        for (int capsuleIndex = 0; capsuleIndex < capsules.Length; capsuleIndex++) {
            CapsuleShape capsule = capsules[capsuleIndex];
            Vector3 pt0 = TrfMathUtils.TrfPtUnscaled(transform, capsule.pt0);
            Vector3 pt1 = TrfMathUtils.TrfPtUnscaled(transform, capsule.pt1);
            int numCols = Physics.OverlapCapsuleNonAlloc(
                pt0,
                pt1,
                capsule.r,
                overlapCapsuleResults,
                layerMask,
                qryTrgIxn
            );
            for (int colliderIndex = 0; colliderIndex < numCols; colliderIndex++) {
                HitReceiver hitReceiver =
                    overlapCapsuleResults[colliderIndex].GetComponent<HitReceiver>();
                if (hitReceiver != null)
                    TryDealHit(hitReceiver, new HitData(atkData, hitWldDir));
            }
        }
    }


}
