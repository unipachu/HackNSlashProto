using UnityEngine;

public class CpFlyingHeadActSt_Fly : IFsmSt_Cp{
    CpFlyingHeadHandle cpFlyingHead;

    public CpFlyingHeadActSt_Fly(CpFlyingHeadHandle cpFlyingHead) {
        this.cpFlyingHead = cpFlyingHead;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpFlyingHeadActSt_Fly Enter() {
        AnimEventPlr.CrossFadeInFixedTimeNInitAnimEventPlr(
            ref cpFlyingHead.CommonData.animEventPlrData,
            cpFlyingHead.anim,
            CpFlyingHeadAnimInfoFactory.Construct(CpFlyingHeadAnimInfoT.idle),
            0.5f
        );
        return this;
    }

    public void Tick() {
        CpUtils.UpdateMovInputData(
            ref cpFlyingHead.CommonData,
            cpFlyingHead.CommonData.input_mov,
            Vector3.zero,
            cpFlyingHead.so_CpCommonConfig.walkTgtHorSpd,
            cpFlyingHead.so_CpCommonConfig.walkYawSpd,
            cpFlyingHead.so_CpCommonConfig.walkHorAcc
        );
        if (
            CpFlyingHeadUtils.TrySwitchStFromNeutralStByBufferedInput(
                ref cpFlyingHead.CommonData,
                ref cpFlyingHead.FlyingHeadData
            )
        )
            return;
        if (cpFlyingHead.CommonData.input_mov.IsZeroOrNearlyZero()) {
            CpUtils.TrySwitchActSt(
                () => cpFlyingHead.FlyingHeadData.actSts.idle.Enter(),
                ref cpFlyingHead.CommonData,
                true
            );
            return;
        }
    }
}
