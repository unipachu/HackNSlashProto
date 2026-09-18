using Unity.Mathematics;
using UnityEngine;

public class BtNode_MovToTgt : BtNode{
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
            // TODO: For all bt nodes queuring agent data, you should use a method which updates the data
            // TODO C: only if it requested and not yet updated this frame. Or maybe caching is useless.
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
