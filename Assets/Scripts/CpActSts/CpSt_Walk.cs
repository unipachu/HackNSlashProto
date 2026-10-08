using Unity.Mathematics;

public class CpSt_Walk : IFsmSt_Cp {
    CpHumdHandle cp;

    public CpSt_Walk(CpHumdHandle cp) {
        this.cp = cp;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_Walk Enter() {
        AnimEventPlr.CrossFadeInFixedTimeNInitAnimEventPlr(
            ref cp.CommonData.animEventPlrData,
            cp.anim,
            CpAnimInfoFactory.Construct(CpHumanoidAnimInfoT.walk),
            0.5f
        );
        return this;
    }

    public void Tick() {
        var classRefs = cp.HumdData.classRefs;
        if (CpUtils.SwitchToFallingStIfNotGrounded(ref cp.CommonData, ref cp.HumdData))
            return;
        CpUtils.UpdateMovInputData(
            ref cp.CommonData,
            cp.CommonData.input_mov,
            float3.zero,
            cp.so_cpCommonData.walkTgtHorSpd,
            cp.so_cpCommonData.walkYawSpd,
            cp.so_cpCommonData.walkHorAcc
        );
        if (CpUtils.CpHumd_TrySwitchStFromNeutralStByBufferedInput(ref cp.CommonData, ref cp.HumdData))
            return;
        if (math.all(cp.CommonData.input_mov == float2.zero)) {
            CpHumdMgr.TrySwitchActSt(() => classRefs.actSts.idle.Enter(), ref cp.CommonData, true);
            return;
        }
    }
}
