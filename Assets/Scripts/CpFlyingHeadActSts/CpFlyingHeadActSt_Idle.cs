using UnityEngine;

public class CpFlyingHeadActSt_Idle : IFsmSt_Cp{
    CpFlyingHeadHandle cpFlyingHead;

    public CpFlyingHeadActSt_Idle(CpFlyingHeadHandle cpFlyingHead) {
        this.cpFlyingHead = cpFlyingHead;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpFlyingHeadActSt_Idle Enter() {
        AnimEventPlr.CrossFadeInFixedTimeNInitAnimEventPlr(
            ref cpFlyingHead.CommonData.animEventPlrData,
            cpFlyingHead.Anim,
            CpFlyingHeadAnimInfoFactory.Construct(CpFlyingHeadAnimInfoT.idle),
            0.1f
        );
        return this;
    }

    public void Tick() {
        ref Cp_CommonData commonData = ref cpFlyingHead.CommonData;
        ref CpFlyingHead_Data flyingHeadData = ref cpFlyingHead.FlyingHeadData;
        // If prev st is walk, we keep rotating towards the last inputted direction (other games do this too).
        // TODO: Uncomment below.
        if (commonData.classRefs.st_prev == flyingHeadData.actSts.fly)
            CpUtils.UpdateMovInputData(
                ref commonData,
                commonData.input_mov_LastNonZero,
                Vector3.zero,
                0,
                cpFlyingHead.CpCommonConfig.walkYawSpd,
                float.PositiveInfinity
            );
        else
            CpUtils.UpdateMovInputData(
                ref commonData,
                Vector2.zero,
                Vector3.zero,
                0,
                0,
                float.PositiveInfinity
            );
        //// Try consume input
        if (CpFlyingHeadUtils.TrySwitchStFromNeutralStByBufferedInput(ref commonData, ref flyingHeadData))
            return;
        if (!commonData.input_mov.IsZeroOrNearlyZero()) {
            CpUtils.TrySwitchActSt(
                () => cpFlyingHead.FlyingHeadData.actSts.fly.Enter(),
                ref commonData,
                true
            );
            return;
        }
    }
}
