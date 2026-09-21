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
            ref CpMgr.inst.animEventPlrData[cp.Id],
            CpMgr.inst.unityComps[cp.Id].anim,
            CpAnimInfoFactory.Construct(CpAnimInfoT.walk),
            0.5f
        );
        return this;
    }

    public void Tick() {
        var classRefs = CpMgr.inst.classRefs[cp.Id];
        if (CpUtils.SwitchToFallingStIfNotGrounded(cp.Id))
            return;
        CpUtils.UpdateMovInputData(
            cp.Id,
            CpMgr.GetAos(cp.Id).input_mov,
            float3.zero,
            CpMgr.GetAos(cp.Id).walkMaxLinSpd,
            CpMgr.GetAos(cp.Id).walkYawSpd,
            CpMgr.GetAos(cp.Id).walkLinAcc
        );
        if (CpUtils.TrySwitchStByBufferedInput(cp.Id))
            return;
        if (math.all(CpMgr.GetAos(cp.Id).input_mov == float2.zero)) {
            CpMgr.inst.SwitchActSt(() => classRefs.actSts.idle.Enter(), cp.Id);
            return;
        }
    }
}
