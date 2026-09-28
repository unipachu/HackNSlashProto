using System.Collections.Generic;
using UnityEngine;

public class RayProj : MonoBehaviour, IProj{
    [Header("Refs")]
    public HitDealer_Linecast hitDealer;
    public TrailRenderer trailRenderer;

    public GameObject Go => gameObject;
    public IHitDealer HitDealer => hitDealer;

    /// <summary>
    /// Index for a active projectile in a proj manager.
    /// </summary>
    public int IInMgr { get; set; } = -1;
    public Transform Tgt { get; set; }
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
