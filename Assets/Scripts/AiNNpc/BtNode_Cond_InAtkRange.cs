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
        // TODO MINOR: We should probably check if the path to player is within certain distance, not just
        // C: distance, because that can make enemies try to hit player through walls.
        return AiCtrlMgr.IsWithinDistToFollowTgt(
            aiCtrl.I,
            data.atkRange
        )
            ? BtResult.Success
            : BtResult.Failure;
    }
}
