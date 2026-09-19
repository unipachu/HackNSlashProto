using Unity.Cinemachine;
using UnityEngine;

public static class CpSpawningUtils {
    static CpHandle SpawnCpAtSpawnPt(
        CpHandle prefab,
        Transform spawnPt
    ) {
        CpHandle cp = GameObject.Instantiate(prefab, spawnPt.position, spawnPt.rotation);
        Debug.Assert(CpMgr.inst != null, $"{typeof(CpMgr).Name} inst was null!");
        Debug.Assert(cp.so_cpData != null, "No data ref set!");
        CpMgr.inst.Register(cp);
        return cp;
    }

    public static void SpawnPlrCpAtSpawnPt(
        CpHandle prefab,
        Transform spawnPt,
        PlrCtrl ctrl,
        CinemachineCamera cam
    ) {
        //Debug.Log($"Spawnin player cp: {prefab.gameObject.name}.");
        CpHandle cp = SpawnCpAtSpawnPt(prefab, spawnPt);
        CpMgr.StartListeningToCtrlInput(cp.Id, ctrl);
        cam.Target.TrackingTarget = cp.transform;

    }

    public static void SpawnAiCpAtSpawnPt(
        So_AiCpConfig aiCpConfig,
        Transform spawnPt
    ) {
        //Debug.Log($"Spawnin ai cp: {cpPrefab.gameObject.name}, with brain: {btT}.");
        AiCtrlHandle aiCtrl = new AiCtrlHandle();
        CpHandle cp;
        IBtNode bt;
        cp = SpawnCpAtSpawnPt(aiCpConfig.cpPrefab, spawnPt);
        bt = BtDataFactory.Construct(aiCpConfig.btT, aiCtrl);
        AiCtrlMgr.inst.Register(aiCtrl, bt, cp, aiCpConfig.data);
        CpMgr.StartListeningToCtrlInput(cp.Id, aiCtrl);
    }
}
