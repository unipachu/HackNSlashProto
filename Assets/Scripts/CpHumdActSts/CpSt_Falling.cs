using Unity.Mathematics;
using UnityEngine;

public class CpSt_Falling : IFsmSt_Cp {
    CpHumdHandle cpHumd;

    public CpSt_Falling(CpHumdHandle cpHumd) {
        this.cpHumd = cpHumd;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public CpSt_Falling Enter() {
        cpHumd.HumdData.act_Falling_StartHgt = cpHumd.HumdData.handle.transform.position.y;
        AnimEventPlr.CrossFadeInFixedTimeNInitAnimEventPlr(
            ref cpHumd.CommonData.animEventPlrData,
            cpHumd.anim,
            CpHumdAnimInfoFactory.Construct(CpHumanoidAnimInfoT.falling),
            2 // NOTE: Transition is long to give a sense of accleration during falling. // TODO: So.
        );
        return this;
    }
    
    public void Tick() {
        ref var commonData = ref cpHumd.CommonData;
        ref var humdData = ref cpHumd.HumdData;
        CpUtils.UpdateMovInputData(
            ref commonData,
            float2.zero,
            float3.zero,
            cpHumd.CpCommonConfig.falling_TgtHorSpd,
            0,
            cpHumd.CpCommonConfig.falling_HorAcc
        );
        if (commonData.isGrounded){
            //Debug.Log("Is grounded");
            float fallDist = humdData.act_Falling_StartHgt - cpHumd.transform.position.y;
            if(fallDist > cpHumd.so_cpHumdConfig.act_Falling_LandingStFallDistThreshold) {
                CpUtils.TrySwitchActSt(
                    () => cpHumd.HumdData.classRefs.actSts.fallLanding.Enter(),
                    ref commonData,
                    true
                );
                return;
            }
            CpHumdUtils.TransitionToFallIdleOrWalk(ref commonData, ref humdData);
            return;
        }
        Debug.Assert(
            commonData.curStDur <= 15,
            $"{cpHumd.I} likely stuck falling as curStDur was: {commonData.curStDur}.",
            cpHumd
        );
    }
}
