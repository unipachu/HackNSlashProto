using Unity.Cinemachine;
using UnityEngine;

public static class CpFactory {
    public static CpHandle SpawnCpAtSpawnPt(CpHandle prefab, Transform spawnPt) {
        CpHandle cp = GameObject.Instantiate(prefab, spawnPt.position, spawnPt.rotation);
        Debug.Assert(CpMgr.inst != null, $"{typeof(CpMgr).Name} inst was null!");
        Debug.Assert(cp.so_cpData != null, "No data ref set!");
        CpMgr.inst.Register(cp);
        return cp;
    }

    public static CpHandle SpawnPlrCpAtSpawnPt(
        CpHandle prefab,
        Transform spawnPt,
        PlrCtrl ctrl,
        CinemachineCamera cam
    ) {
        //Debug.Log($"Spawnin player cp: {prefab.gameObject.name}.");
        CpHandle cp = SpawnCpAtSpawnPt(prefab, spawnPt);
        CpMgr.StartListeningToCtrlInput(cp.I, ctrl);
        cam.Target.TrackingTarget = cp.transform;
        PlrMgr.inst.SetPlr(cp);
        return cp;
    }
}
