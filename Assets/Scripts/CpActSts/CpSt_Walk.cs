using Unity.Mathematics;
using Unity.VisualScripting.FullSerializer;

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
            CpAnimInfo.walk,
            0.5f
        );
        return this;
    }

    public void Exit() {}

    public void HandleAnimEvent(CpAnimEventT animEvent) {}

    public void LateTick() {}

    public void PhysicsTick() {}

    public void Tick() {
        Cp_SoaData data = CpMgr.inst.soaData;
        ref Cp_AosData aosData = ref CpMgr.inst.aosData[cpId];
        var classRefs = CpMgr.inst.classRefs[cpId];
        if (CpUtils.SwitchToFallingStIfNotGrounded(cpId))
            return;
        CpUtils.UpdateMovData(
            cpId,
            data,
            data.input_mov[cpId],
            float3.zero,
            data.st_Walk_MaxLinSpd[cpId],
            data.st_Walk_YawSpd[cpId],
            data.st_Walk_LinAcc[cpId]
        );
        if (CpUtils.BaseTrySwitchStByBufferedInput(cpId))
            return;
        if (math.all(data.input_mov[cpId] == float2.zero)) {
            CpMgr.inst.SwitchToActSt(() => classRefs.actSts.idle.Enter(), cpId);
            return;
        }
    }
}
