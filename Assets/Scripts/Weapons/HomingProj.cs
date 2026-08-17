using UnityEngine;

public class HomingProj : MonoBehaviour {
    [Header("Refs")]
    public HitDealer hitDealer;

    [HideInInspector] public int poolI;

    Transform tgt;

    private void OnEnable() {
        hitDealer.hitReceiverHit += OnHitReceiverHit;
    }

    private void OnDisable() {
        hitDealer.hitReceiverHit -= OnHitReceiverHit;
    }

    void OnHitReceiverHit(HitResult hitResult) {
        HomingProjMgr.inst.DeactivateProj(poolI);
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
}
