using UnityEngine;

/// <summary>
/// Basic branching combo state. Basically any combo move that doesn't have any extra functionality (in
/// addition to the animation events) except to switch to next combo node (by input or by animation events).
/// </summary>
public class CpSt_Atk_BasicBranch : IFsmSt_Cp {
    CpHandle cp;
    IComboNode comboNode;

    public CpSt_Atk_BasicBranch(CpHandle cp) {
        this.cp = cp;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_Atk_BasicBranch Enter(IComboNode comboNode) {
        this.comboNode = comboNode;
        InputBufferUtils.Clear(
            ref cp.Data.inputBuffer_BufferedInput,
            ref cp.Data.inputBuffer_RemainingTime
        );
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.aos[cp.I].animEventPlrData,
            cp.Data.unityObjs.anim,
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
        CpUtils.UpdateMovInputData(
            cp.I,
            cp.Data.input_mov_WhenLastSwitchedSt,
            cp.Data.animDPos,
            0,
            cp.Data.act_BasicWindup_MaxAngSpd,
            float.PositiveInfinity
        );
        // NOTE: Windup can be optionally canceled. (5.9.2026)
        if (CpUtils.SwitchToFallingStIfNotGrounded(cp.I))
            return;
        if (cp.Data.comboAllowed && CpUtils.TryAnyComboInputTransition(cp.I, comboNode))
            return;
    }
}
