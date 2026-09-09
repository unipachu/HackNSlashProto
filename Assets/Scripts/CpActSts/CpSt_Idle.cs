using Unity.Mathematics;
using UnityEngine;

public class CpSt_Idle : IFsmSt_Cp {
    int cpId;

    public CpSt_Idle(int cpId) {
        this.cpId = cpId;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
    => true;

    public CpSt_Idle Enter() {
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cpId],
            CpMgr.inst.unityComps[cpId].anim,
            CpAnimInfo.Get(CpAnimInfoT.idle),
            0.1f
        );
        return this;
    }

    public void Exit() {}

    public void HandleAnimEvent(CpAnimEventT animEvent) {}

    public void LateTick() {}

    public void PhysicsTick() {}

    public void Tick() {
        Cp_SoaData data = CpMgr.inst.soaData;
        var classRefs = CpMgr.inst.classRefs[cpId];
        ref Cp_AosData aosData = ref CpMgr.inst.aosData[cpId];
        // If prev st is walk, we keep rotating towards the last inputted direction (other games do this too).
        if (classRefs.st_prev == classRefs.actSts.walk)
            CpUtils.UpdateMovData(
                cpId,
                data,
                data.input_mov_LastNonZero[cpId],
                float3.zero,
                0,
                data.walkYawSpd[cpId],
                float.PositiveInfinity
            );
        else
            CpUtils.UpdateMovData(
                cpId,
                data,
                float2.zero,
                float3.zero,
                0,
                0,
                float.PositiveInfinity
            );
        if (CpUtils.SwitchToFallingStIfNotGrounded(cpId))
            return;
        // Try consume input
        if (CpUtils.BaseTrySwitchStByBufferedInput(cpId))
            return;
        if (math.all(data.input_mov[cpId] != float2.zero)) {
            CpMgr.inst.SwitchActSt(() => classRefs.actSts.walk.Enter(), cpId);
            return;
        }
    }
}
