using Unity.Mathematics;

public class CpSt_Walk : IFsmSt_Cp {
    CpHandle cp;

    public CpSt_Walk(CpHandle cp) {
        this.cp = cp;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_Walk Enter() {
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cp.I],
            CpMgr.inst.unityComps[cp.I].anim,
            CpAnimInfoFactory.Construct(CpAnimInfoT.walk),
            0.5f
        );
        return this;
    }

    public void Tick() {
        var classRefs = CpMgr.inst.classRefs[cp.I];
        if (CpUtils.SwitchToFallingStIfNotGrounded(cp.I))
            return;
        CpUtils.UpdateMovInputData(
            cp.I,
            CpMgr.GetData(cp.I).input_mov,
            float3.zero,
            CpMgr.GetData(cp.I).walkMaxLinSpd,
            CpMgr.GetData(cp.I).walkYawSpd,
            CpMgr.GetData(cp.I).walkLinAcc
        );
        if (CpUtils.TrySwitchStByBufferedInput(cp.I))
            return;
        if (math.all(CpMgr.GetData(cp.I).input_mov == float2.zero)) {
            CpMgr.inst.SwitchActSt(() => classRefs.actSts.idle.Enter(), cp.I);
            return;
        }
    }
}
