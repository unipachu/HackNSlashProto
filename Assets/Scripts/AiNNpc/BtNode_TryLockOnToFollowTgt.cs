public class BtNode_TryLockOnToFollowTgt : IBtNode{
    AiCtrlHandle aiCtrl;

    public BtNode_TryLockOnToFollowTgt(AiCtrlHandle aiCtrl) {
        this.aiCtrl = aiCtrl;
    }

    public string DbgName => typeof(BtNode_TryLockOnToFollowTgt).Name;

    public BtResult Eval() {
        bool result = AiCtrlMgr.TryLockOnToFollowTgt(aiCtrl.I);
        //Dbg.Log(
        //    $"{nameof(aiCtrl)} {aiCtrl.Id}: {DbgName}: {result}",
        //    AiCtrlMgr.GetData(aiCtrl.Id).cp,
        //    CpMgr.GetAos(AiCtrlMgr.GetData(aiCtrl.Id).cp.Id).enableDbgMsgs
        //);
        return result ? BtResult.Success : BtResult.Failure;
    }
}
