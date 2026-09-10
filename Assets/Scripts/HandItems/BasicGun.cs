using UnityEngine;

public class BasicGun : MonoBehaviour, IHandItem_Hitter, IHandItem_ProjectileSpawner {
    [SerializeField] HitDealer meleeHitDealer;
    [SerializeField] HitEffects hitEffects = new HitEffects(1, KnockbackT.Weak, 1);
    [SerializeField] HomingProjData homingProjData = new(10, 4, 0.5f);
    [SerializeField] HitEffects projHitEffects = new HitEffects(1, KnockbackT.Weak, 1);
    [SerializeField] Transform projSpawnPose;

    public HitDealer HitDealer => meleeHitDealer;
    public HitEffects HitEffects => hitEffects;
    public HomingProjData HomingProjData => homingProjData;
    public HitEffects ProjHitEffects => projHitEffects;
    public Transform ProjSpawnPose => projSpawnPose;
    public Transform Trf => transform;

}
