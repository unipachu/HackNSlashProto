using UnityEngine;

public interface IHandItem_ProjectileSpawner : IHandItem {
    AimLaser AimLaser { get; }
    HomingProjData HomingProjData { get; }
    HitEffects ProjHitEffects { get; }
    Transform ProjSpawnPoseTrf { get; }
    public ProjT ProjT {get;}
}
