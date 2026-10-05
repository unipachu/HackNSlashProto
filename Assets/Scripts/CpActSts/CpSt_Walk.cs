using Unity.Mathematics;

public class CpSt_Walk : IFsmSt_Cp {
    CpHandle cp;

    public CpSt_Walk(CpHandle cp) {
        this.cp = cp;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_Walk Enter() {
        AnimEventPlr.CrossFadeInFixedTimeNInitAnimEventPlr(
            ref CpMgr.inst.aos[cp.I].animEventPlrData,
            cp.anim,
            CpAnimInfoFactory.Construct(CpAnimInfoT.walk),
            0.5f
        );
        return this;
    }

    public void Tick() {
        var classRefs = cp.Data.classRefs;
        if (CpUtils.SwitchToFallingStIfNotGrounded(cp.I))
            return;
        CpUtils.UpdateMovInputData(
            cp.I,
            cp.Data.input_mov,
            float3.zero,
            cp.so_cpData.walkTgtHorSpd,
            cp.so_cpData.walkYawSpd,
            cp.so_cpData.walkHorAcc
        );
        if (CpUtils.TrySwitchStFromNeutralStByBufferedInput(cp.I))
            return;
        if (math.all(cp.Data.input_mov == float2.zero)) {
            CpMgr.inst.TrySwitchActSt(() => classRefs.actSts.idle.Enter(), cp.I, true);
            return;
        }
    }
}
