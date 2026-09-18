using Unity.Mathematics;

public class BtNode_Cmd_Atk1 : BtNode{
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
        CpMgr.GetAos(cpId).input_atk_Light = true;
        float2 horDesiredVel = new(
            CpMgr.inst.brainData[cpId].agentDesiredVel.x,
            CpMgr.inst.brainData[cpId].agentDesiredVel.z
        );
        // Agent can have 0 desired velocity, thus to avoid NaNs:
        if (math.lengthsq(horDesiredVel) > 0.0001f)
            // Movement input should always be max 1 length.
            AiCtrlMgr.inst.ctrlInputData[aiCtrlId].input_Mov = math.normalize(horDesiredVel);
        else
            AiCtrlMgr.inst.ctrlInputData[aiCtrlId].input_Mov = float2.zero;
        //Debug.Log($"{cpId} BtNodeT.Cmd_MovToTgt movement input: {ccMgr.input_mov[cpId]}", this);
        return BtResult.Success;
    }
}
