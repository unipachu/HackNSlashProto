using Unity.Mathematics;
using UnityEngine;

// TODO: Rename to CpHumd
public class CpSt_Knockback : IFsmSt_Cp{
    CpHandle cpHumd;

    public CpSt_Knockback(CpHandle cpHumd) {
        this.cpHumd = cpHumd;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt {
        //Debug.Log($"type of TState: {typeof(TState).Name}");
        return typeof(TState) == typeof(CpSt_Knockback)
            || typeof(TState) == typeof(CpSt_Death);
    }

    public CpSt_Knockback Enter(AnimInfo knockbackAnimInfo) {
        AnimEventPlr.CrossFadeInFixedTimeNInitAnimEventPlr(
            ref cpHumd.CommonData.animEventPlrData,
            cpHumd.anim,
            knockbackAnimInfo,
            0.1f
        );
        return this;
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        switch (animEvent) {
            case CpAnimEventT.Finished:
                CpUtils.TransitionToFallIdleOrWalk(ref cpHumd.CommonData, ref cpHumd.HumdData);
                break;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                break;
        }
    }

    public void Tick() {
        //if (CpUtils.SwitchToFallingStIfNotGrounded(cp.Id))
        //    return;
        //Debug.Log($"knocback: {data.lastKnockbackStr[cp.Id]}\nanim delta: {data.animDPos[cp.Id]}");
        ref var commonData = ref cpHumd.CommonData;
        CpUtils.UpdateMovInputData(
            ref commonData,
            float2.zero,
            commonData.animDPose.position * commonData.lastKnockbackStr * cpHumd.so_cpCommonData.knockbackStrMult,
            0,
            0,
            float.PositiveInfinity
        );
    }
}
