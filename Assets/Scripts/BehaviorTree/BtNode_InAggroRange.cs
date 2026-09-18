// TODO: Add "Cond" to name
public class BtNode_InAggroRange : BtNode {
    CpRegisterer cp;

    public BtNode_InAggroRange(CpRegisterer cp) {
        this.cp = cp;
    }

    public string DbgName => typeof(BtNode_InAggroRange).Name;

    public BtResult Eval() {
        Dbg.Log(
            $"{cp.Id} bt node: {typeof(BtNode_InAggroRange).Name}: "
                + $"{CpMgr.IsInAggroRange(cp.Id)}",
            cp,
            CpMgr.GetAos(cp.Id).enableDbgMsgs
        );
        return CpMgr.IsInAggroRange(cp.Id) ? BtResult.Success: BtResult.Failure;
    }
}
