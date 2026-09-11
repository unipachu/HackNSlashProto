using Unity.Mathematics;

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
        var classRefs = CpMgr.inst.classRefs[cpId];
        // If prev st is walk, we keep rotating towards the last inputted direction (other games do this too).
        if (classRefs.st_prev == classRefs.actSts.walk)
            CpUtils.UpdateMovInputData(
                cpId,
                CpMgr.GetAos(cpId).input_mov_LastNonZero,
                float3.zero,
                0,
                CpMgr.GetAos(cpId).walkYawSpd,
                float.PositiveInfinity
            );
        else
            CpUtils.UpdateMovInputData(
                cpId,
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
        if (math.all(CpMgr.GetAos(cpId).input_mov != float2.zero)) {
            CpMgr.inst.SwitchActSt(() => classRefs.actSts.walk.Enter(), cpId);
            return;
        }
    }
}
