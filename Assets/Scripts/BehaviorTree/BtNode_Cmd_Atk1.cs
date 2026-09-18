using Unity.Mathematics;

public class BtNode_Cmd_Atk1 : IBtNode{
    CpRegisterer cp;
    AiCtrl aiCtrl;

    public string DbgName => typeof(BtNode_Cmd_Atk1).Name;

    public BtNode_Cmd_Atk1(CpRegisterer cp, AiCtrl aiCtrl) {
        this.cp = cp;
        this.aiCtrl = aiCtrl;
    }

    public BtResult Eval() {
        int cpId = cp.Id;
        int aiCtrlId = aiCtrl.Id;
        AiCtrlMgr.inst.ctrlInputData[aiCtrlId].input_Atk_Light = true;
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
        //Dbg.Log(
        //    $"{cpId} bt node: {typeof(BtNode_Cmd_Atk1).Name} mov input: "
        //        + $"{AiCtrlMgr.inst.ctrlInputData[aiCtrlId].input_Mov}",
        //    cp,
        //    CpMgr.GetAos(cpId).enableDbgMsgs
        //);
        return BtResult.Success;
    }
}
