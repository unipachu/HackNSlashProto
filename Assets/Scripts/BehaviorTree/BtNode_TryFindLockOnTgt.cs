// TODO: Rename to TryLockOnTgt
public class BtNode_TryFindLockOnTgt : IBtNode {
    AiCtrl aiCtrl;

    public BtNode_TryFindLockOnTgt(AiCtrl aiCtrl) {
        this.aiCtrl = aiCtrl;
    }

    public string DbgName => typeof(BtNode_TryFindLockOnTgt).Name;

    public BtResult Eval() {
        bool result = CpMgr.TryFindTgt(AiCtrlMgr.GetData(aiCtrl).cp.Id);
        //Dbg.Log(
        //    $"{cp.Id} bt node: {typeof(BtNode_TryFindLockOnTgt).Name}: "
        //        + $"{result}",
        //    cp,
        //    CpMgr.GetAos(cp.Id).enableDbgMsgs
        //);
        return result ? BtResult.Success : BtResult.Failure;
    }
}
