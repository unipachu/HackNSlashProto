using System.Collections.Generic;
using UnityEngine;

// TODO: Rename to SphereProj. Do this after pushing, because this will likely break proj prefab.
public class HomingProj : MonoBehaviour, IProj {
    [Header("Refs")]
    public HitDealer_SphereCast hitDealer;
    public TrailRenderer trailRenderer;

    public GameObject Go => gameObject;
    public IHitDealer HitDealer => hitDealer;
    /// <summary>
    /// Index for a active projectile in a proj manager.
    /// </summary>
    public int IInMgr { get; set; } = -1;
    public Transform Tgt {get; set;}
    public TrailRenderer TrailRenderer => trailRenderer;
    public Transform Trf => transform;

    void OnEnable() {
        hitDealer.hitSomething += OnHitReceiverHit;
    }

    void OnDisable() {
        hitDealer.hitSomething -= OnHitReceiverHit;
    }

    void OnHitReceiverHit(HashSet<HitResult> hitResults) {
        HomingProjMgr.inst.DeactivateProj(IInMgr);
    }
}
