using Unity.Mathematics;

public class BtNode_MovToTgt : IBtNode{
    CpRegisterer cp;
    AiCtrl aiCtrl;

    public string DbgName => typeof(BtNode_MovToTgt).Name;

    public BtNode_MovToTgt(CpRegisterer cp, AiCtrl aiCtrl) {
        this.cp = cp;
        this.aiCtrl = aiCtrl;
    }

    public BtResult Eval() {
        int cpId = cp.Id;
        int aiCtrlId = aiCtrl.Id;
        float2 horDesiredVel = new(
            AiCtrlMgr.inst.aosData[aiCtrlId].agentDesiredVel.x,
            AiCtrlMgr.inst.aosData[aiCtrlId].agentDesiredVel.z
        );
        // Agent can have 0 desired velocity, thus to avoid NaNs:
        if (math.lengthsq(horDesiredVel) > 0.0001f)
            // Movement input should always be max 1 length.
            AiCtrlMgr.inst.ctrlInputData[aiCtrlId].input_Mov = math.normalize(horDesiredVel);
        else
            AiCtrlMgr.inst.ctrlInputData[aiCtrlId].input_Mov = float2.zero;
        Dbg.Log($"{cpId} bt node: {typeof(BtNode_MovToTgt).Name} mov input: "
                + $"{AiCtrlMgr.inst.ctrlInputData[aiCtrlId].input_Mov}",
            cp,
            CpMgr.GetAos(cpId).enableDbgMsgs
        );
        return BtResult.Success;
    }
}
