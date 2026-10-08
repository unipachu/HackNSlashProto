using UnityEngine;

/// <summary>
/// Creates cp's with ai controllers.
/// </summary>
public class AiCpFactory {
    /// <summary>
    /// Spawns and registers an entity for <see cref="CpHumdMgr"/> and an entity for
    /// <see cref="AiCtrlMgr"/>. Returns handle to the spawned entity.
    /// </summary>
    public static AiCtrlHandle SpawnAiCpAtSpawnPt(So_AiCpConfig aiCpConfig, Transform spawnPt) {
        //Debug.Log($"Spawnin ai cp: {cpPrefab.gameObject.name}, with brain: {btT}.");
        AiCtrlHandle aiCtrl = new AiCtrlHandle();
        ICp cp;
        IBtNode bt;
        cp = CpFactory.SpawnCpAtSpawnPt(aiCpConfig.cpPrefab.Value, spawnPt);
        bt = BtDataFactory.Construct(aiCpConfig.btT, aiCtrl);
        AiCtrlMgr.inst.Register(aiCpConfig.aggroRange, aiCpConfig.atkRange, aiCtrl, bt, cp);
        CpUtils.StartListeningToCtrlInput(ref cp.CommonData, aiCtrl);
        // Ui related
        WldHpBarMgr.inst.Register(cp.WldHpBarPos, cp);
        return aiCtrl;
    }
}
