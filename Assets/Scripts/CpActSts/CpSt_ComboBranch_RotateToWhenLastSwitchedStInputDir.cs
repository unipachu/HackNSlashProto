using UnityEngine;

/// <summary>
/// NOTE: This name is not very descriptive. This has hor spd input = 0, but does have yaw. Yaw uses
/// input_mov_WhenLastSwitchedSt.
/// </summary>
public class CpSt_ComboBranch_RotateToWhenLastSwitchedStInputDir : IFsmSt_Cp {
    CpHandle cpHumd;
    IComboNode_CpHumanoid comboNode;

    public CpSt_ComboBranch_RotateToWhenLastSwitchedStInputDir(CpHandle cpHumd) {
        this.cpHumd = cpHumd;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_ComboBranch_RotateToWhenLastSwitchedStInputDir Enter(IComboNode_CpHumanoid comboNode) {
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
        ref var commonData = ref cpHumd.CommonData;
        switch (animEvent) {
            case CpAnimEventT.Finished:
                if (comboNode.GetNextNode(BufferableInput.None) != null) {
                    CpMgr.TrySwitchActSt(
                        comboNode.GetNextNode(BufferableInput.None).GetEnterFunc(cpHumd),
                        ref commonData,
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
            commonData.input_mov_WhenLastSwitchedSt,
            commonData.animDPose.position,
            0,
            cpHumd.so_cpCommonData.windup_YawSpd,
            float.PositiveInfinity
        );
        // NOTE: Windup can be optionally canceled. (5.9.2026)
        if (CpUtils.SwitchToFallingStIfNotGrounded(ref commonData, ref cpHumd.HumdData))
            return;
        if (commonData.comboAllowed && CpUtils.TryAnyComboInputTransition(cpHumd, comboNode))
            return;
    }
}
