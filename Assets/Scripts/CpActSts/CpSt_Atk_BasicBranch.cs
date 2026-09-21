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
        Cp_UnityObjs[] unityComps = CpMgr.inst.unityComps;
        CpMgr.GetData(cp.Id).act_AtkPhase = AtkPhase.Windup;
        InputBufferUtils.Clear(
            ref CpMgr.GetData(cp.Id).inputBuffer_BufferedInput,
            ref CpMgr.GetData(cp.Id).inputBuffer_RemainingTime
        );
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cp.Id],
            unityComps[cp.Id].anim,
            comboNode.AnimInfo,
            0.1f
        );
        return this;
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        var classRefs = CpMgr.inst.classRefs[cp.Id];
        switch (animEvent) {
            case CpAnimEventT.Finished:
                if (comboNode.GetNextNode(BufferableInput.None) != null) {
                    CpMgr.inst.SwitchActSt(
                        comboNode.GetNextNode(BufferableInput.None).GetEnterFunc(cp.Id),
                        cp.Id
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
            cp.Id,
            CpMgr.GetData(cp.Id).input_mov_WhenLastSwitchedSt,
            CpMgr.GetData(cp.Id).animDPos,
            0,
            CpMgr.GetData(cp.Id).act_BasicWindup_MaxAngSpd,
            float.PositiveInfinity
        );
        // NOTE: Windup can be optionally canceled. (5.9.2026)
        if (CpUtils.SwitchToFallingStIfNotGrounded(cp.Id))
            return;
        if (CpMgr.GetData(cp.Id).comboAllowed && CpUtils.TryAnyComboInputTransition(cp.Id, comboNode))
            return;
    }
}
