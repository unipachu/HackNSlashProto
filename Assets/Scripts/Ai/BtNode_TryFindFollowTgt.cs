public class BtNode_TryFindFollowTgt : IBtNode {
    AiCtrlHandle aiCtrl;

    public BtNode_TryFindFollowTgt(AiCtrlHandle aiCtrl) {
        this.aiCtrl = aiCtrl;
    }

    public string DbgName => typeof(BtNode_TryFindFollowTgt).Name;

    public BtResult Eval() {
        bool result = AiCtrlMgr.TryFindFollowTgt(aiCtrl.Id);
        //Dbg.Log(
        //    $"{cp.Id} bt node: {typeof(BtNode_TryFindLockOnTgt).Name}: "
        //        + $"{result}",
        //    cp,
        //    CpMgr.GetAos(cp.Id).enableDbgMsgs
        //);
        return result ? BtResult.Success : BtResult.Failure;
    }
}
