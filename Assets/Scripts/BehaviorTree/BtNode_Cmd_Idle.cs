using Unity.Mathematics;

public class BtNode_Cmd_Idle : IBtNode{
    CpRegisterer cp;
    AiCtrl aiCtrl;

    public string DbgName => typeof(BtNode_Cmd_Idle).Name;

    public BtNode_Cmd_Idle(CpRegisterer cp, AiCtrl aiCtrl) {
        this.cp = cp;
        this.aiCtrl = aiCtrl;
    }

    public BtResult Eval() {
        int cpId = cp.Id;
        int aiCtrlId = aiCtrl.Id;
        AiCtrlMgr.inst.ctrlInputData[aiCtrlId].input_Mov = float2.zero;
        Dbg.Log(
            $"{cpId} bt node: {typeof(BtNode_Cmd_Idle).Name}",
            cp,
            CpMgr.GetAos(cpId).enableDbgMsgs
        );
        return BtResult.Success;
    }
}
