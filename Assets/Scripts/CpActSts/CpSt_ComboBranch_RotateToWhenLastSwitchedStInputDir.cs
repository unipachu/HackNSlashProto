using UnityEngine;

/// <summary>
/// NOTE: This name is not very descriptive. This has hor spd input = 0, but does have yaw. Yaw uses
/// input_mov_WhenLastSwitchedSt.
/// </summary>
public class CpSt_ComboBranch_RotateToWhenLastSwitchedStInputDir : IFsmSt_Cp {
    CpHandle cp;
    IComboNode comboNode;

    public CpSt_ComboBranch_RotateToWhenLastSwitchedStInputDir(CpHandle cp) {
        this.cp = cp;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_ComboBranch_RotateToWhenLastSwitchedStInputDir Enter(IComboNode comboNode) {
        this.comboNode = comboNode;
        InputBufferUtils.Clear(
            ref cp.Data.inputBuffer_BufferedInput,
            ref cp.Data.inputBuffer_RemainingTime
        );
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.aos[cp.I].animEventPlrData,
            cp.anim,
            comboNode.AnimInfo,
            0.1f
        );
        return this;
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        var classRefs = cp.Data.classRefs;
        switch (animEvent) {
            case CpAnimEventT.Finished:
                if (comboNode.GetNextNode(BufferableInput.None) != null) {
                    CpMgr.inst.SwitchActSt(
                        comboNode.GetNextNode(BufferableInput.None).GetEnterFunc(cp.I),
                        cp.I
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
        ref var cpData = ref cp.Data;
        //Dbg.Log(
        //    $"Cp {cp.I}: {nameof(cpData.act_BasicWindup_MaxAngSpd)}: {cpData.act_BasicWindup_MaxAngSpd}",
        //    cpData.enableDbgMsgs
        //);
        CpUtils.UpdateMovInputData(
            cp.I,
            cpData.input_mov_WhenLastSwitchedSt,
            cpData.animDPos,
            0,
            cpData.handle.so_cpData.windup_YawSpd,
            float.PositiveInfinity
        );
        // NOTE: Windup can be optionally canceled. (5.9.2026)
        if (CpUtils.SwitchToFallingStIfNotGrounded(cp.I))
            return;
        if (cpData.comboAllowed && CpUtils.TryAnyComboInputTransition(cp.I, comboNode))
            return;
    }
}
