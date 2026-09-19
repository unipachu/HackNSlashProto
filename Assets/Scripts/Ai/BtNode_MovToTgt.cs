using Unity.Mathematics;

public class BtNode_MovToTgt : IBtNode{
    AiCtrl aiCtrl;

    public string DbgName => typeof(BtNode_MovToTgt).Name;

    public BtNode_MovToTgt(AiCtrl aiCtrl) {
        this.aiCtrl = aiCtrl;
    }

    public BtResult Eval() {
        ref AiCtrlData data = ref AiCtrlMgr.GetData(aiCtrl);
        int cpId = data.cp.Id;
        int aiCtrlId = aiCtrl.Id;
        float2 horDesiredVel = new(
            AiCtrlMgr.inst.aos[aiCtrlId].agentDesiredVel.x,
            AiCtrlMgr.inst.aos[aiCtrlId].agentDesiredVel.z
        );
        // Agent can have 0 desired velocity, thus to avoid NaNs:
        if (math.lengthsq(horDesiredVel) > 0.0001f)
            // Movement input should always be max 1 length.
            data.ctrlInputData.input_Mov = math.normalize(horDesiredVel);
        else
            data.ctrlInputData.input_Mov = float2.zero;
        //Dbg.Log($"{cpId} bt node: {typeof(BtNode_MovToTgt).Name} mov input: "
        //        + $"{data.ctrlInputData.input_Mov}",
        //    data.cp,
        //    CpMgr.GetAos(cpId).enableDbgMsgs
        //);
        return BtResult.Success;
    }
}
