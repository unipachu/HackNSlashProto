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
    [HideInInspector] public HitData hitData;

    bool isActive;
    Collider[] overlapCapsuleResults;
    HashSet<IHitReceiver> hitReceiversHitDuringLastActivation = new();

    public bool IsActive => isActive;

    // ------------------------------------------------------------------
    // Unity Callbacks
    // ------------------------------------------------------------------

    void Awake() {
        overlapCapsuleResults = new Collider[maxColliders];
    }

    void Update() {
        if (isActive)
            TryHitAllOverlappingHitRecievers(capsuleLayerMask);
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

    public void Activate() {
        isActive = true;
        hitReceiversHitDuringLastActivation.Clear();
    }

    public void Deactivate() {
        isActive = false;
    }

    public bool TryDealHit(IHitReceiver hitReceiver, HitData hitData) {
        if (hitReceiversHitDuringLastActivation.Contains(hitReceiver))
            return false;
        HitResult hitResult = hitReceiver.ReceiveHit(this, hitData);
        hitReceiversHitDuringLastActivation.Add(hitReceiver);
        hitReceiverHit?.Invoke(hitResult);
        return true;
    }

    public void TryHitAllOverlappingHitRecievers(
        int layerMask,
        bool allowFriendlyFire = false,
        QueryTriggerInteraction qryTrgIxn = QueryTriggerInteraction.Collide
    ) {
        for (int capsuleIndex = 0; capsuleIndex < capsules.Length; capsuleIndex++) {
            CapsuleShape capsule = capsules[capsuleIndex];
            Vector3 pt0 = capsule.pt0.TrfPtUnscaled(transform);
            Vector3 pt1 = capsule.pt1.TrfPtUnscaled(transform);
            int numCols = Physics.OverlapCapsuleNonAlloc(
                pt0,
                pt1,
                capsule.r,
                overlapCapsuleResults,
                layerMask,
                qryTrgIxn
            );
            for (int colliderIndex = 0; colliderIndex < numCols; colliderIndex++) {
                IHitReceiver hitReceiver =
                    overlapCapsuleResults[colliderIndex].GetComponent<IHitReceiver>();
                if (hitReceiver == null)
                    continue;
                if(hitReceiver.GetTeam() == hitData.team && !allowFriendlyFire)
                    continue;
                TryDealHit(hitReceiver, hitData);
            }
        }
    }


}
