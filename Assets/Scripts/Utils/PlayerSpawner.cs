using Unity.Cinemachine;
using UnityEngine;

// TODO: Rename to CpSpawner or similar
public static class PlayerSpawner {
    /// <summary>
    /// Spawns a cp and calls its <see cref="CpRegisterer.Init"/>.
    /// </summary>
    public static CpRegisterer SpawnCpAtSpawnPt(
        CpRegisterer prefab,
        Transform spawnPt,
        ICpCtrlInputter ctrl
    ) {
        CpRegisterer cp = GameObject.Instantiate(prefab, spawnPt);
        Debug.Assert(CpMgr.inst != null, $"{typeof(CpMgr).Name} inst was null!");
        Debug.Assert(cp.so_cpData != null, "No data ref set!");
        CpMgr.inst.Register(ctrl, cp);
        return cp;
    }

    public static void SpawnPlrCpAtSpawnPt(
        CpRegisterer prefab,
        Transform spawnPt,
        PlrCtrl ctrl,
        CinemachineCamera cam
    ) {
        cam.Target.TrackingTarget = SpawnCpAtSpawnPt(prefab, spawnPt, ctrl).transform;
    }

    public static void SpawnAiCpAtSpawnPt(
        AiCtrl prefab,
        BtNode btRoot,
        CpRegisterer cpPrefab,
        Transform spawnPt
    ) {
        AiCtrlMgr.inst.Register(prefab, btRoot);
        CpRegisterer cp = SpawnCpAtSpawnPt(cpPrefab, spawnPt, prefab);
    }
}
