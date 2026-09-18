using UnityEngine;

public class BtNode_Cond_InAtkRange : BtNode{
    CpRegisterer cp;

    public BtNode_Cond_InAtkRange(CpRegisterer cp) {
        this.cp = cp;
    }

    public string DbgName => typeof(BtNode_Cond_InAtkRange).Name;

    public BtResult Eval()
        // TODO: Have a IsInAtkRange method which caches the result if not used this frame.
        // TODO C: Or maybe caching is useless.
        => CpMgr.inst.brainData[cp.Id].inAtkRange ? BtResult.Success : BtResult.Failure;
}
