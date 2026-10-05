using UnityEngine;

public class CpSt_Dodge : IFsmSt_Cp {
    CpHandle cp;

    public CpSt_Dodge(CpHandle cp) {
        this.cp = cp;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_Dodge Enter() {
        ref var cpData = ref cp.Data;
        cpData.yawAllowed = false;
        cpData.bufferedInputStSwitchAllowed = false;
        cpData.ignoreHits = true;
        cpData.cooldownTimer_Dodge = cp.so_cpData.cooldownDur_Dodge;
        cpData.cooldownFreezed_Dodge = true;
        AnimEventPlr.CrossFadeInFixedTimeNInitAnimEventPlr(
            ref cpData.animEventPlrData,
            cp.anim,
            CpAnimInfoFactory.Construct(CpAnimInfoT.dodge),
            0.1f
        );
        // NOTE: We instantly rotate towards movement input direction.
        cpData.handle.transform.rotation = TrfMathUtils.RotateFwdTowardsTgt(
            cpData.handle.transform.rotation,
            cpData.movInput_tgtHorDir
        );
        return this;
    }

    public void Exit() {
        ref var cpData = ref cp.Data;
        cpData.ignoreHits = false;
        cpData.cooldownFreezed_Dodge = false;
    }

    public void Tick() {
        float angSpd = 0;
        if (cp.Data.yawAllowed)
            angSpd = cp.so_cpData.act_Dodge_YawAngSpd;
        CpUtils.UpdateMovInputData(
            cp.I,
            cp.Data.input_mov,
            cp.Data.animDPos * cp.so_cpData.act_Dodge_HorMovSpdMult,
            0,
            angSpd,
            float.PositiveInfinity
        );
        if (cp.Data.bufferedInputStSwitchAllowed && CpUtils.TrySwitchStFromNeutralStByBufferedInput(cp.I))
            return;
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        var classRefs = cp;
        switch (animEvent) {
            case CpAnimEventT.Finished:
                CpUtils.TransitionToFallIdleOrWalk(cp.I);
                break;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                break;
        }
    }
}
