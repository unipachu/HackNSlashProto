using Unity.Mathematics;
using UnityEngine;

public class CpFlyingHeadActSt_Knockback : IFsmSt_Cp{
    CpFlyingHeadHandle cpFlyingHead;

    public CpFlyingHeadActSt_Knockback(CpFlyingHeadHandle cpFlyingHead) {
        this.cpFlyingHead = cpFlyingHead;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt {
        //Debug.Log($"type of TState: {typeof(TState).Name}");
        return typeof(TState) == typeof(CpFlyingHeadActSt_Knockback)
            || typeof(TState) == typeof(CpFlyingHeadActSt_Death);
    }

    public CpFlyingHeadActSt_Knockback Enter(AnimInfo knockbackAnimInfo) {
        AnimEventPlr.CrossFadeInFixedTimeNInitAnimEventPlr(
            ref cpFlyingHead.CommonData.animEventPlrData,
            cpFlyingHead.anim,
            knockbackAnimInfo,
            0.1f
        );
        return this;
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        switch (animEvent) {
            case CpAnimEventT.Finished:
                CpFlyingHeadUtils.TransitionToIdleOrFly(
                    ref cpFlyingHead.CommonData,
                    ref cpFlyingHead.FlyingHeadData
                );
                break;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                break;
        }
    }

    public void Tick() {
        //if (CpHumdUtils.SwitchToFallingStIfNotGrounded(cp.Id))
        //    return;
        //Debug.Log($"knocback: {data.lastKnockbackStr[cp.Id]}\nanim delta: {data.animDPos[cp.Id]}");
        ref var commonData = ref cpFlyingHead.CommonData;
        CpUtils.UpdateMovInputData(
            ref commonData,
            float2.zero,
            commonData.animDPose.position
                * commonData.lastKnockbackStr
                * cpFlyingHead.so_CpCommonConfig.knockbackStrMult,
            0,
            0,
            float.PositiveInfinity
        );
    }
}
