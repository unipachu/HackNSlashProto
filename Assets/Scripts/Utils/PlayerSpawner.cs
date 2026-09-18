using Unity.Cinemachine;
using UnityEngine;

// TODO: Rename to CpSpawner or similar
public static class PlayerSpawner {
    static CpRegisterer SpawnCpAtSpawnPt(
        CpRegisterer prefab,
        Transform spawnPt
    ) {
        CpRegisterer cp = GameObject.Instantiate(prefab, spawnPt.position, spawnPt.rotation);
        Debug.Assert(CpMgr.inst != null, $"{typeof(CpMgr).Name} inst was null!");
        Debug.Assert(cp.so_cpData != null, "No data ref set!");
        CpMgr.inst.Register(cp);
        return cp;
    }

    public static void SpawnPlrCpAtSpawnPt(
        CpRegisterer prefab,
        Transform spawnPt,
        PlrCtrl ctrl,
        CinemachineCamera cam
    ) {
        //Debug.Log($"Spawnin player cp: {prefab.gameObject.name}.");
        CpRegisterer cp = SpawnCpAtSpawnPt(prefab, spawnPt);
        CpMgr.StartListeningToCtrlInput(cp.Id, ctrl);
        cam.Target.TrackingTarget = cp.transform;

    }

    public static void SpawnAiCpAtSpawnPt(
        So_NpcSetup aiCpConfig,
        Transform spawnPt
    ) {
        //Debug.Log($"Spawnin ai cp: {cpPrefab.gameObject.name}, with brain: {btT}.");
        AiCtrl aiCtrl = new AiCtrl();
        CpRegisterer cp;
        IBtNode bt;
        cp = SpawnCpAtSpawnPt(aiCpConfig.cpPrefab, spawnPt);
        bt = CpBehaviorTreeData.Get(aiCpConfig.btT, cp, aiCtrl);
        AiCtrlMgr.inst.Register(aiCtrl, bt, cp, aiCpConfig.data);
        CpMgr.StartListeningToCtrlInput(cp.Id, aiCtrl);
    }
}
