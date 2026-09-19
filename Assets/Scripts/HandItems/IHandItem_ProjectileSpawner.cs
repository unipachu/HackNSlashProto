using UnityEngine;

public interface IHandItem_ProjectileSpawner : IHandItem {
    HomingProjData HomingProjData { get; }
    HitEffects HitEffects { get; }
    Transform ProjSpawnPoseTrf { get; }
}
