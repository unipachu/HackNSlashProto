using UnityEngine;

public interface IHandItem_ProjectileSpawner : IHandItem {
    HomingProjData HomingProjData { get; }
    HitEffects HitEffects { get; }
    Transform ProjSpawnPose { get; }
}
