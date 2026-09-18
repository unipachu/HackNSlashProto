public class BtNode_Cond_InAggroRange : IBtNode {
    AiCtrl aiCtrl;
    CpRegisterer cp;

    public BtNode_Cond_InAggroRange(AiCtrl aiCtrl, CpRegisterer cp) {
        this.aiCtrl = aiCtrl;
        this.cp = cp;
    }

    public string DbgName => typeof(BtNode_Cond_InAggroRange).Name;

    public BtResult Eval() {
        //Dbg.Log(
        //    $"{cp.Id} bt node: {typeof(BtNode_Cond_InAggroRange).Name}: "
        //        + $"{CpMgr.IsWithinDistToLockOnTgt(cp.Id, AiCtrlMgr.GetData(aiCtrl).aggroRange)}",
        //    cp,
        //    CpMgr.GetAos(cp.Id).enableDbgMsgs
        //);
        return CpMgr.IsWithinDistToLockOnTgt(cp.Id, AiCtrlMgr.GetData(aiCtrl).aggroRange)
            ? BtResult.Success
            : BtResult.Failure;
    }
}
