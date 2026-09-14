using Unity.Cinemachine;
using UnityEngine;

// TODO: Rename to CpSpawner or similar
public static class PlayerSpawner {
    /// <summary>
    /// Spawns a cp and calls its <see cref="CpRegisterer.Init"/>.
    /// </summary>
    public static void SpawnCpAtSpawnPt(
        CpRegisterer prefab,
        Transform spawnPt,
        ICpCtrlInputter ctrl
    ) {
        CpRegisterer plr = GameObject.Instantiate(prefab, spawnPt);
        plr.Init(ctrl);
    }

    public static void SpawnPlrCpAtSpawnPt(
        CpRegisterer prefab,
        Transform spawnPt,
        ICpCtrlInputter ctrl,
        CinemachineCamera cam
    ) {
        CpRegisterer plr = GameObject.Instantiate(prefab, spawnPt);
        plr.Init(ctrl);
        cam.Target.TrackingTarget = plr.transform;
    }
}
