using UnityEngine;

public class BtNode_Cond_InAtkRange : BtNode{
    CpRegisterer cp;

    public BtNode_Cond_InAtkRange(CpRegisterer cp) {
        this.cp = cp;
    }

    public string DbgName => typeof(BtNode_Cond_InAtkRange).Name;

    public BtResult Eval() {
        Dbg.Log(
            $"{cp.Id} bt node: {typeof(BtNode_Cond_InAtkRange).Name}: "
                + $"{CpMgr.IsInAtkRange(cp.Id)}",
            cp,
            CpMgr.GetAos(cp.Id).enableDbgMsgs
        );
        return CpMgr.IsInAtkRange(cp.Id) ? BtResult.Success : BtResult.Failure;
    }
}
