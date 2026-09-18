// TODO: Rename to TryLockOnTgt
public class BtNode_TryFindLockOnTgt : BtNode {
    CpRegisterer cp;

    public BtNode_TryFindLockOnTgt(CpRegisterer cp) {
        this.cp = cp;
    }

    public string DbgName => typeof(BtNode_TryFindLockOnTgt).Name;

    public BtResult Eval() {
        bool result = CpMgr.TryFindTgt(cp.Id);
        Dbg.Log(
            $"{cp.Id} bt node: {typeof(BtNode_TryFindLockOnTgt).Name}: "
                + $"{result}",
            cp,
            CpMgr.GetAos(cp.Id).enableDbgMsgs
        );
        return result ? BtResult.Success : BtResult.Failure;
    }
}
