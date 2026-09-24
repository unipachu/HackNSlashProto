using System.Collections.Generic;
using UnityEngine;

public class HomingProj : MonoBehaviour {
    [Header("Refs")]
    public HitDealer_SphereCast hitDealer;

    [HideInInspector] public int poolI;

    Transform tgt;

    void OnEnable() {
        hitDealer.hitSomething += OnHitReceiverHit;
    }

    void OnDisable() {
        hitDealer.hitSomething -= OnHitReceiverHit;
    }

    public void SetTgt(Transform tgt) {
        this.tgt = tgt;
    }

    public bool TryGetTgtPos(out Vector3 tgtPos) {
        if (tgt == null) {
            tgtPos = default;
            return false;
        }
        tgtPos = tgt.position;
        return true;
    }

    void OnHitReceiverHit(HashSet<HitResult> hitResult) {
        HomingProjMgr.inst.DeactivateProj(poolI);
    }
}
