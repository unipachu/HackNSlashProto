// TODO: Add "Cond" to name
public class BtNode_InAggroRange : BtNode {
    CpRegisterer cp;

    public BtNode_InAggroRange(CpRegisterer cp) {
        this.cp = cp;
    }

    public string DbgName => typeof(BtNode_InAggroRange).Name;

    public BtResult Eval()
        // TODO: Have a IsInAggro range method which caches the result if not used this frame.
        // TODO C: Or maybe caching is useless.
        => CpMgr.inst.brainData[cp.Id].inAggroRange ? BtResult.Success: BtResult.Failure;
}
