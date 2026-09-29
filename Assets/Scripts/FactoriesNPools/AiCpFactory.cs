using UnityEngine;

/// <summary>
/// Creates cp's with ai controllers.
/// </summary>
public class AiCpFactory {
    /// <summary>
    /// Spawns and registers an entity for <see cref="CpMgr"/> and an entity for
    /// <see cref="AiCtrlMgr"/>. Returns handle to the spawned entity.
    /// </summary>
    public static AiCtrlHandle SpawnAiCpAtSpawnPt(So_AiCpConfig aiCpConfig, Transform spawnPt) {
        //Debug.Log($"Spawnin ai cp: {cpPrefab.gameObject.name}, with brain: {btT}.");
        AiCtrlHandle aiCtrl = new AiCtrlHandle();
        CpHandle cp;
        IBtNode bt;
        cp = CpFactory.SpawnCpAtSpawnPt(aiCpConfig.cpPrefab, spawnPt);
        bt = BtDataFactory.Construct(aiCpConfig.btT, aiCtrl);
        AiCtrlMgr.inst.Register(aiCpConfig.aggroRange, aiCpConfig.atkRange, aiCtrl, bt, cp);
        CpMgr.StartListeningToCtrlInput(cp.I, aiCtrl);
        // Ui related
        WldHpBarMgr.inst.Register(cp.wldHpBarPos, cp);
        return aiCtrl;
    }
}
