using Unity.Mathematics;

public class CpSt_Walk : IFsmSt_Cp {
    int cpId;

    public CpSt_Walk(int cpId) {
        this.cpId = cpId;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_Walk Enter() {
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cpId],
            CpMgr.inst.unityComps[cpId].anim,
            CpAnimInfo.Get(CpAnimInfoT.walk),
            0.5f
        );
        return this;
    }

    public void Exit() {}

    public void HandleAnimEvent(CpAnimEventT animEvent) {}

    public void LateTick() {}

    public void PhysicsTick() {}

    public void Tick() {
        ref Cp_AosData aosData = ref CpMgr.inst.aosData[cpId];
        var classRefs = CpMgr.inst.classRefs[cpId];
        if (CpUtils.SwitchToFallingStIfNotGrounded(cpId))
            return;
        CpUtils.UpdateMovInputData(
            cpId,
            CpMgr.GetSoa(cpId).input_mov,
            float3.zero,
            CpMgr.GetSoa(cpId).walkMaxLinSpd,
            CpMgr.GetSoa(cpId).walkYawSpd,
            CpMgr.GetSoa(cpId).walkLinAcc
        );
        if (CpUtils.BaseTrySwitchStByBufferedInput(cpId))
            return;
        if (math.all(CpMgr.GetSoa(cpId).input_mov == float2.zero)) {
            CpMgr.inst.SwitchActSt(() => classRefs.actSts.idle.Enter(), cpId);
            return;
        }
    }
}
