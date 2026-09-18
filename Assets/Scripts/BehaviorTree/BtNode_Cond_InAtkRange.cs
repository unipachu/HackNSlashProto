using UnityEngine;

public class BtNode_Cond_InAtkRange : IBtNode{
    CpRegisterer cp;
    AiCtrl aiCtrl;

    public BtNode_Cond_InAtkRange(AiCtrl aiCtrl, CpRegisterer cp) {
        this.aiCtrl = aiCtrl;
        this.cp = cp;
    }

    public string DbgName => typeof(BtNode_Cond_InAtkRange).Name;

    public BtResult Eval() {
        //Dbg.Log(
        //    $"{cp.Id} bt node: {typeof(BtNode_Cond_InAtkRange).Name}: "
        //        + $"{CpMgr.IsWithinDistToLockOnTgt(cp.Id, AiCtrlMgr.GetData(aiCtrl).atkRange)}",
        //    cp,
        //    CpMgr.GetAos(cp.Id).enableDbgMsgs
        //);
        return CpMgr.IsWithinDistToLockOnTgt(cp.Id, AiCtrlMgr.GetData(aiCtrl).atkRange)
            ? BtResult.Success
            : BtResult.Failure;
    }
}
