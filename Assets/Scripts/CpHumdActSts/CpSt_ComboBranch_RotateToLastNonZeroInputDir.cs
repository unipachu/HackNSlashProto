using UnityEngine;

/// <summary>
/// NOTE: The name of this is not very descriptive. This will use last non zero movement input to yaw the
/// character but target hor movement is 0. So input still works for rotation.
/// </summary>
public class CpSt_ComboBranch_RotateToLastNonZeroInputDir : IFsmSt_Cp {
    CpHumdHandle cpHumd;
    IComboNode_CpHumanoid comboNode;

    public CpSt_ComboBranch_RotateToLastNonZeroInputDir(CpHumdHandle cpHumd) {
        this.cpHumd = cpHumd;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_ComboBranch_RotateToLastNonZeroInputDir Enter(IComboNode_CpHumanoid comboNode) {
        ref var commonData = ref cpHumd.CommonData;
        this.comboNode = comboNode;
        InputBufferUtils.Clear(
            ref commonData.inputBuffer_BufferedInput,
            ref commonData.inputBuffer_RemainingTime
        );
        AnimEventPlr.CrossFadeInFixedTimeNInitAnimEventPlr(
            ref commonData.animEventPlrData,
            cpHumd.anim,
            comboNode.AnimInfo,
            0.1f
        );
        return this;
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        switch (animEvent) {
            case CpAnimEventT.Finished:
                if (comboNode.GetNextNode(BufferableInput.None) != null) {
                    CpUtils.TrySwitchActSt(
                        comboNode.GetNextNode(BufferableInput.None).GetEnterFunc(cpHumd),
                        ref cpHumd.CommonData,
                        true
                    );
                    return;
                }
                break;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                break;
        }
    }

    public void Tick() {
        ref var commonData = ref cpHumd.CommonData;
        //Dbg.Log(
        //    $"Cp {cp.I}: {nameof(cpData.act_BasicWindup_MaxAngSpd)}: {cpData.act_BasicWindup_MaxAngSpd}",
        //    cpData.enableDbgMsgs
        //);
        CpUtils.UpdateMovInputData(
            ref commonData,
            commonData.input_mov_LastNonZero,
            commonData.animDPose.position,
            0,
            cpHumd.so_cpCommonData.windup_YawSpd, // TODO: tgtHorSpd, yawSpd, and horAcc should be set in the state Enter method, since these are currently are set to work with melee windup moves and nothing else.
            float.PositiveInfinity
        );
        // NOTE: Windup can be optionally canceled. (5.9.2026)
        if (CpHumdUtils.SwitchToFallingStIfNotGrounded(ref commonData, ref cpHumd.HumdData))
            return;
        if (commonData.comboAllowed && CpHumdUtils.TryAnyComboInputTransition(cpHumd, comboNode))
            return;
    }
}
