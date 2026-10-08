using Unity.Cinemachine;
using UnityEngine;

public static class CpFactory {
    public static ICp SpawnCpAtSpawnPt(ICp prefab, Transform spawnPt) {
        GameObject cpGo = GameObject.Instantiate(prefab.Go, spawnPt.position, spawnPt.rotation);
        ICp instantiatedDp = cpGo.GetComponent<ICp>();
        if (instantiatedDp is CpHumdHandle cpHandle) {
            CpHumdMgr.inst.Register(cpHandle);
        }else if (instantiatedDp is CpFlyingHeadHandle cpFlyingHeadHandle) {
            CpFlyingHeadMgr.inst.Register(cpFlyingHeadHandle);
        }else
            Debug.LogError($"Unkonwn {nameof(ICp)} implementation.");
        //Debug.Log($"Agent type ID: {cp.GetComponent<NavMeshAgent>().agentTypeID}");
        //Debug.Log($"Spawn position: {cp.transform.position}");
        //Debug.Log($"On NavMesh: {NavMesh.SamplePosition(cp.transform.position, out _, 1f, NavMesh.AllAreas)}");
        //bool debugFound = NavMesh.SamplePosition(
        //    cp.transform.position,
        //    out NavMeshHit hit,
        //    1f,
        //    1 << cp.navMeshAgent.agentTypeID
        //);
        //Debug.Log($"Agent type: {cp.navMeshAgent.agentTypeID}, NavMesh: {debugFound}, hit: {hit.position}");
        Debug.Assert(CpHumdMgr.inst != null, $"{typeof(CpHumdMgr).Name} inst was null!");
        return instantiatedDp;
    }

    public static CpHumdHandle SpawnPlrCpAtSpawnPt(
        CpHumdHandle prefab,
        Transform spawnPt,
        PlrCtrl ctrl,
        CinemachineCamera cam
    ) {
        //Debug.Log($"Spawnin player cp: {prefab.gameObject.name}.");
        ICp instantiatedCp = SpawnCpAtSpawnPt(prefab, spawnPt);
        CpHumdHandle instantiatedCpHumanoid = instantiatedCp as CpHumdHandle;
        CpHumdMgr.StartListeningToCtrlInput(ref instantiatedCpHumanoid.CommonData, ctrl);
        cam.Target.TrackingTarget = instantiatedCp.Go.transform;
        PlrMgr.inst.SetPlr(instantiatedCpHumanoid);
        return instantiatedCpHumanoid;
    }
}
