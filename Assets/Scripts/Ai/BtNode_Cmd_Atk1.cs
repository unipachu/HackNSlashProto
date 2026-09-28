using Unity.Mathematics;

public class BtNode_Cmd_Atk1 : IBtNode{
    AiCtrlHandle aiCtrl;

    public string DbgName => typeof(BtNode_Cmd_Atk1).Name;

    public BtNode_Cmd_Atk1(AiCtrlHandle aiCtrl) {
        this.aiCtrl = aiCtrl;
    }

    public BtResult Eval() {
        ref AiCtrlData aiCtrlData = ref aiCtrl.Data;
        int cpI = aiCtrlData.cp.I;
        int aiCtrlId = aiCtrl.I;
        aiCtrlData.ctrlInputData.input_Atk_Light = true;
        float2 horDesiredVel = new(
            aiCtrlData.agentDesiredVel.x,
            aiCtrlData.agentDesiredVel.z
        );
        // Agent can have 0 desired velocity, thus to avoid NaNs:
        if (math.lengthsq(horDesiredVel) > 0.0001f)
            // Movement input should always be max 1 length.
            aiCtrlData.ctrlInputData.input_Mov = math.normalize(horDesiredVel);
        else
            aiCtrlData.ctrlInputData.input_Mov = float2.zero;
        //Dbg.Log(
        //    $"{cpI} bt node: {typeof(BtNode_Cmd_Atk1).Name} mov input: "
        //        + $"{aiCtrlData.ctrlInputData.input_Mov}",
        //    aiCtrlData.cp,
        //    aiCtrlData.cp.Data.enableDbgMsgs
        //);
        return BtResult.Success;
    }
}
