using UnityEngine;

public class CpSt_Dodge : IFsmSt_Cp {
    CpHumdHandle cpHumd;

    public CpSt_Dodge(CpHumdHandle cpHumd) {
        this.cpHumd = cpHumd;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_Dodge Enter() {
        ref var commonData = ref cpHumd.CommonData;
        ref var humdData = ref cpHumd.HumdData;
        commonData.yawAllowed = false;
        commonData.bufferedInputStSwitchAllowed = false;
        commonData.ignoreHits = true;
        humdData.cooldownTimer_Dodge = cpHumd.so_cpHumdConfig.cooldownDur_Dodge;
        humdData.cooldownFreezed_Dodge = true;
        AnimEventPlr.CrossFadeInFixedTimeNInitAnimEventPlr(
            ref commonData.animEventPlrData,
            cpHumd.anim,
            CpAnimInfoFactory.Construct(CpHumanoidAnimInfoT.dodge),
            0.1f
        );
        // NOTE: We instantly rotate towards movement input direction.
        cpHumd.transform.rotation = TrfMathUtils.RotateFwdTowardsTgt(
            cpHumd.transform.rotation,
            commonData.movInput_tgtHorDir
        );
        return this;
    }

    public void Exit() {
        cpHumd.CommonData.ignoreHits = false;
        cpHumd.HumdData.cooldownFreezed_Dodge = false;
    }

    public void Tick() {
        ref var commonData = ref cpHumd.CommonData;
        float angSpd = 0;
        if (commonData.yawAllowed)
            angSpd = cpHumd.so_cpHumdConfig.act_Dodge_YawAngSpd;
        CpUtils.UpdateMovInputData(
            ref commonData,
            commonData.input_mov,
            commonData.animDPose.position * cpHumd.so_cpHumdConfig.act_Dodge_HorMovSpdMult,
            0,
            angSpd,
            float.PositiveInfinity
        );
        if (
            commonData.bufferedInputStSwitchAllowed
                && CpHumdUtils.TrySwitchStFromNeutralStByBufferedInput(
                    ref cpHumd.CommonData,
                    ref cpHumd.HumdData
                )
        )
            return;
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        switch (animEvent) {
            case CpAnimEventT.Finished:
                CpHumdUtils.TransitionToFallIdleOrWalk(ref cpHumd.CommonData, ref cpHumd.HumdData);
                break;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                break;
        }
    }
}
