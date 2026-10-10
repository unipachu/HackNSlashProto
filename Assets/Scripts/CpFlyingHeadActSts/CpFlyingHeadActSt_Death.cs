using Unity.Mathematics;
using UnityEngine;

// TODO MINOR: This is very similar to death state of the other cp. Could some action states be generalized
// C: for several cp types?
public class CpFlyingHeadActSt_Death : IFsmSt_Cp{
    CpFlyingHeadHandle cpFlyingHead;

    public CpFlyingHeadActSt_Death(CpFlyingHeadHandle cpFlyingHead) {
        this.cpFlyingHead = cpFlyingHead;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => false; // Cannot change to anything when dying.

    public CpFlyingHeadActSt_Death Enter(AnimInfo deathAnim) {
        ref Cp_CommonData commonData = ref cpFlyingHead.CommonData;
        commonData.ignoreHits = true;
        commonData.action_Died?.Invoke(cpFlyingHead);
        AnimEventPlr.CrossFadeInFixedTimeNInitAnimEventPlr(
            ref commonData.animEventPlrData,
            cpFlyingHead.anim,
            deathAnim,
            0.1f
        );
        return this;
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        switch (animEvent) {
            case CpAnimEventT.Finished:
                CpUtils.MarkForPendingUnregister(ref cpFlyingHead.CommonData);
                return;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                return;
        }
    }

    public void Tick() {
        ref var commonData = ref cpFlyingHead.CommonData;
        CpUtils.UpdateMovInputData(
            ref commonData,
            float2.zero,
            commonData.animDPose.position
                * commonData.lastKnockbackStr
                * cpFlyingHead.CpCommonConfig.knockbackStrMult,
            0,
            0,
            float.PositiveInfinity
        );
    }
}
