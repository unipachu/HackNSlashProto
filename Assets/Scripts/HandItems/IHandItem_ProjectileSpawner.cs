using UnityEngine;

public interface IHandItem_ProjectileSpawner : IHandItem {
    HomingProjData HomingProjData { get; }
    HitEffects ProjHitEffects { get; }
    Transform ProjSpawnPose { get; }
}
