using UnityEngine;

public class CpSt_Idle : IFsmSt_Cp {
    CpHumdHandle cpHumd;

    public CpSt_Idle(CpHumdHandle cpHumd) {
        this.cpHumd = cpHumd;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_Idle Enter() {
        AnimEventPlr.CrossFadeInFixedTimeNInitAnimEventPlr(
            ref cpHumd.CommonData.animEventPlrData,
            cpHumd.Anim,
            CpAnimInfoFactory.Construct(CpHumanoidAnimInfoT.idle),
            0.1f
        );
        return this;
    }

    public void Tick() {
        ref var commonData = ref cpHumd.CommonData;
        ref var humdData = ref cpHumd.HumdData;
        // If prev st is walk, we keep rotating towards the last inputted direction (other games do this too).
        if (commonData.classRefs.st_prev == humdData.classRefs.actSts.walk)
            CpUtils.UpdateMovInputData(
                ref commonData,
                commonData.input_mov_LastNonZero,
                Vector3.zero,
                0,
                cpHumd.so_cpCommonData.walkYawSpd,
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
        if (CpHumdUtils.SwitchToFallingStIfNotGrounded(ref commonData, ref humdData))
            return;
        // Try consume input
        if (CpHumdUtils.TrySwitchStFromNeutralStByBufferedInput(ref commonData, ref humdData))
            return;
        if (!commonData.input_mov.IsZeroOrNearlyZero()) {
            CpUtils.TrySwitchActSt(
                () => cpHumd.HumdData.classRefs.actSts.walk.Enter(),
                ref commonData,
                true
            );
            return;
        }
    }
}
