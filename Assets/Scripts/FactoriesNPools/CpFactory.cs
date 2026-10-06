using Unity.Cinemachine;
using UnityEngine;

public static class CpFactory {
    public static CpHandle SpawnCpAtSpawnPt(CpHandle prefab, Transform spawnPt) {
        CpHandle cp = GameObject.Instantiate(prefab, spawnPt.position, spawnPt.rotation);
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
