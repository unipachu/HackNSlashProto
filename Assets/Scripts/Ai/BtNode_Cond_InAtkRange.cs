public class BtNode_Cond_InAtkRange : IBtNode{
    AiCtrlHandle aiCtrl;

    public BtNode_Cond_InAtkRange(AiCtrlHandle aiCtrl) {
        this.aiCtrl = aiCtrl;
    }

    public string DbgName => typeof(BtNode_Cond_InAtkRange).Name;

    public BtResult Eval() {
        ref AiCtrlData data = ref AiCtrlMgr.GetData(aiCtrl);
        //Dbg.Log(
        //    $"{data.cp.Id} bt node: {typeof(BtNode_Cond_InAtkRange).Name}: "
        //        + $"{CpMgr.IsWithinDistToLockOnTgt(data.cp.Id, data.atkRange)}",
        //    data.cp,
        //    CpMgr.GetAos(data.cp.Id).enableDbgMsgs
        //);
        return AiCtrlMgr.IsWithinDistToFollowTgt(
            aiCtrl.Id,
            data.atkRange
        )
            ? BtResult.Success
            : BtResult.Failure;
    }
}
