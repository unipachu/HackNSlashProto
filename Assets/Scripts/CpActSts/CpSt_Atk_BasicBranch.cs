using UnityEngine;

/// <summary>
/// Basic branching combo state. Basically any combo move that doesn't have any extra functionality (in
/// addition to the animation events) except to switch to next combo node (by input or by animation events).
/// </summary>
public class CpSt_Atk_BasicBranch : IFsmSt_Cp {
    int cpId;
    IComboNode comboNode;

    public CpSt_Atk_BasicBranch(int cpId) {
        this.cpId = cpId;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_Atk_BasicBranch Enter(IComboNode comboNode) {
        this.comboNode = comboNode;
        Cp_UnityComps[] unityComps = CpMgr.inst.unityComps;
        CpMgr.GetSoa(cpId).actStSt_AtkPhase = AtkPhase.Windup;
        CpInputBuffer.Clear(
            ref CpMgr.GetSoa(cpId).inputBuffer_BufferedInput,
            ref CpMgr.GetSoa(cpId).inputBuffer_RemainingTime
        );
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cpId],
            unityComps[cpId].anim,
            comboNode.AnimInfo,
            0.1f
        );
        return this;
    }

    public void Exit() {}

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        var classRefs = CpMgr.inst.classRefs[cpId];
        ref Cp_AosData aosData = ref CpMgr.inst.aosData[cpId];
        switch (animEvent) {
            case CpAnimEventT.Finished:
                if (comboNode.GetNextNode(BufferableInput.None) != null) {
                    CpMgr.inst.SwitchActSt(
                        comboNode.GetNextNode(BufferableInput.None).GetEnterFunc(cpId),
                        cpId
                    );
                    return;
                }
                break;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                break;
        }
    }

    public void LateTick() {}

    public void PhysicsTick() {}

    public void Tick() {
        CpUtils.UpdateMovInputData(
            cpId,
            CpMgr.GetSoa(cpId).input_mov_WhenLastSwitchedSt,
            CpMgr.GetSoa(cpId).animDPos,
            0,
            CpMgr.GetSoa(cpId).st_AtkHorSlash_Windup_MaxAngSpd,
            float.PositiveInfinity
        );
        // NOTE: Windup can be optionally canceled. (5.9.2026)
        if (CpUtils.SwitchToFallingStIfNotGrounded(cpId))
            return;
        if (CpMgr.GetSoa(cpId).actStSt_ComboAllowed && CpUtils.TryAnyComboInputTransition(cpId, comboNode))
            return;
    }
}
