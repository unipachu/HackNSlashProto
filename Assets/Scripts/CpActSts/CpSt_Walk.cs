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
        var classRefs = CpMgr.inst.classRefs[cpId];
        if (CpUtils.SwitchToFallingStIfNotGrounded(cpId))
            return;
        CpUtils.UpdateMovInputData(
            cpId,
            CpMgr.GetAos(cpId).input_mov,
            float3.zero,
            CpMgr.GetAos(cpId).walkMaxLinSpd,
            CpMgr.GetAos(cpId).walkYawSpd,
            CpMgr.GetAos(cpId).walkLinAcc
        );
        if (CpUtils.BaseTrySwitchStByBufferedInput(cpId))
            return;
        if (math.all(CpMgr.GetAos(cpId).input_mov == float2.zero)) {
            CpMgr.inst.SwitchActSt(() => classRefs.actSts.idle.Enter(), cpId);
            return;
        }
    }
}
