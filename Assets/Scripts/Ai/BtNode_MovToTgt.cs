using Unity.Mathematics;

public class BtNode_MovToTgt : IBtNode{
    AiCtrlHandle aiCtrl;

    public string DbgName => typeof(BtNode_MovToTgt).Name;

    public BtNode_MovToTgt(AiCtrlHandle aiCtrl) {
        this.aiCtrl = aiCtrl;
    }

    public BtResult Eval() {
        ref AiCtrlData data = ref AiCtrlMgr.GetData(aiCtrl);
        int cpI = data.cp.I;
        int aiCtrlId = aiCtrl.I;
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
        //Dbg.Log($"{cpI} bt node: {typeof(BtNode_MovToTgt).Name} mov input: "
        //        + $"{data.ctrlInputData.input_Mov}",
        //    data.cp,
        //    CpMgr.GetAos(cpI).enableDbgMsgs
        //);
        return BtResult.Success;
    }
}
