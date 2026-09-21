using Unity.Mathematics;

public class CpSt_Idle : IFsmSt_Cp {
    CpHandle cp;

    public CpSt_Idle(CpHandle cp) {
        this.cp = cp;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
    => true;

    public CpSt_Idle Enter() {
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cp.Id],
            CpMgr.inst.unityComps[cp.Id].anim,
            CpAnimInfoFactory.Construct(CpAnimInfoT.idle),
            0.1f
        );
        return this;
    }

    public void Tick() {
        var classRefs = CpMgr.inst.classRefs[cp.Id];
        // If prev st is walk, we keep rotating towards the last inputted direction (other games do this too).
        if (classRefs.st_prev == classRefs.actSts.walk)
            CpUtils.UpdateMovInputData(
                cp.Id,
                CpMgr.GetData(cp.Id).input_mov_LastNonZero,
                float3.zero,
                0,
                CpMgr.GetData(cp.Id).walkYawSpd,
                float.PositiveInfinity
            );
        else
            CpUtils.UpdateMovInputData(
                cp.Id,
                float2.zero,
                float3.zero,
                0,
                0,
                float.PositiveInfinity
            );
        if (CpUtils.SwitchToFallingStIfNotGrounded(cp.Id))
            return;
        // Try consume input
        if (CpUtils.TrySwitchStByBufferedInput(cp.Id))
            return;
        if (math.all(CpMgr.GetData(cp.Id).input_mov != float2.zero)) {
            CpMgr.inst.SwitchActSt(() => classRefs.actSts.walk.Enter(), cp.Id);
            return;
        }
    }
}
