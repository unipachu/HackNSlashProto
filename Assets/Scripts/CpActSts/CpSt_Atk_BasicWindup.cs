using UnityEngine;

/// <summary>
/// Basic attack windup state.
/// </summary>
// TODO: Maybe rename this to "BasicCombo" because it could be used by other than windups as well,
// TODO C: basically any combo move that doesn't have any extra functionality except to switch to next
// TODO C: combo move by action or by animation finished. (6.9.2025)
public class CpSt_Atk_BasicWindup : IFsmSt_Cp {
    int cpId;
    IComboNode comboNode;

    public CpSt_Atk_BasicWindup(int cpId) {
        this.cpId = cpId;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_Atk_BasicWindup Enter(IComboNode comboNode) {
        this.comboNode = comboNode;
        Cp_SoaData data = CpMgr.inst.soaData;
        Cp_UnityComps[] unityComps = CpMgr.inst.unityComps;
        data.actStSt_AtkPhase[cpId] = AtkPhase.Windup;
        CpInputBuffer.Clear(cpId, data.inputBuffer_BufferedInput, data.inputBuffer_RemainingTime);
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cpId],
            unityComps[cpId].anim,
            comboNode.GetAnimInfo(),
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
                if (comboNode.NextNodeI(BufferableInput.None) != -1) {
                    CpMgr.inst.SwitchToActSt(
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
        Cp_SoaData data = CpMgr.inst.soaData;
        CpUtils.UpdateMovData(
            cpId,
            data,
            data.input_mov_WhenLastSwitchedSt[cpId],
            data.animDPos[cpId],
            0,
            data.st_AtkHorSlash_Windup_MaxAngSpd[cpId],
            float.PositiveInfinity
        );
        // NOTE: Windup can be optionally canceled. (5.9.2026)
        if (CpUtils.SwitchToFallingStIfNotGrounded(cpId))
            return;
        if (CpUtils.TryComboTransition(BufferableInput.RShldr, comboNode, cpId))
            return;
        if (CpUtils.TryComboTransition(BufferableInput.RTrg, comboNode, cpId))
            return;
        if (CpUtils.TryComboTransition(BufferableInput.BtnE, comboNode, cpId))
            return;
        if (CpUtils.TryComboTransition(BufferableInput.LShldr, comboNode, cpId))
            return;
    }
}
