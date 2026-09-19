public class BtNode_Cond_InAggroRange : IBtNode {
    AiCtrlHandle aiCtrl;

    public BtNode_Cond_InAggroRange(AiCtrlHandle aiCtrl) {
        this.aiCtrl = aiCtrl;
    }

    public string DbgName => typeof(BtNode_Cond_InAggroRange).Name;

    public BtResult Eval() {
        ref AiCtrlData data = ref AiCtrlMgr.GetData(aiCtrl);
        //Dbg.Log(
        //    $"{data.cp.Id} bt node: {typeof(BtNode_Cond_InAggroRange).Name}: "
        //        + $"{CpMgr.IsWithinDistToLockOnTgt(data.cp.Id, data.aggroRange)}",
        //    data.cp,
        //    CpMgr.GetAos(data.cp.Id).enableDbgMsgs
        //);
        return CpMgr.IsWithinDistToLockOnTgt(
            data.cp.Id,
            data.aggroRange
        )
            ? BtResult.Success
            : BtResult.Failure;
    }
}
