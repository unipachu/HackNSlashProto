using Unity.Mathematics;

public class BtNode_Cmd_Atk1 : IBtNode{
    AiCtrlHandle aiCtrl;

    public string DbgName => typeof(BtNode_Cmd_Atk1).Name;

    public BtNode_Cmd_Atk1(AiCtrlHandle aiCtrl) {
        this.aiCtrl = aiCtrl;
    }

    public BtResult Eval() {
        ref AiCtrlData data = ref AiCtrlMgr.GetData(aiCtrl);
        int cpId = data.cp.Id;
        int aiCtrlId = aiCtrl.Id;
        data.ctrlInputData.input_Atk_Light = true;
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
        //Dbg.Log(
        //    $"{cpId} bt node: {typeof(BtNode_Cmd_Atk1).Name} mov input: "
        //        + $"{AiCtrlMgr.inst.ctrlInputData[aiCtrlId].input_Mov}",
        //    cp,
        //    CpMgr.GetAos(cpId).enableDbgMsgs
        //);
        return BtResult.Success;
    }
}
