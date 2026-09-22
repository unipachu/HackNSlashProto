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
            ref CpMgr.inst.animEventPlrData[cp.I],
            CpMgr.inst.unityComps[cp.I].anim,
            CpAnimInfoFactory.Construct(CpAnimInfoT.idle),
            0.1f
        );
        return this;
    }

    public void Tick() {
        var classRefs = CpMgr.inst.classRefs[cp.I];
        // If prev st is walk, we keep rotating towards the last inputted direction (other games do this too).
        if (classRefs.st_prev == classRefs.actSts.walk)
            CpUtils.UpdateMovInputData(
                cp.I,
                CpMgr.GetData(cp.I).input_mov_LastNonZero,
                float3.zero,
                0,
                CpMgr.GetData(cp.I).walkYawSpd,
                float.PositiveInfinity
            );
        else
            CpUtils.UpdateMovInputData(
                cp.I,
                float2.zero,
                float3.zero,
                0,
                0,
                float.PositiveInfinity
            );
        if (CpUtils.SwitchToFallingStIfNotGrounded(cp.I))
            return;
        // Try consume input
        if (CpUtils.TrySwitchStByBufferedInput(cp.I))
            return;
        if (math.all(CpMgr.GetData(cp.I).input_mov != float2.zero)) {
            CpMgr.inst.SwitchActSt(() => classRefs.actSts.walk.Enter(), cp.I);
            return;
        }
    }
}
