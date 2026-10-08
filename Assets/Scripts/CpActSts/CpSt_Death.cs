using Unity.Mathematics;
using UnityEngine;

public class CpSt_Death : IFsmSt_Cp {
    CpHandle cp;

    public CpSt_Death(CpHandle cp) {
        this.cp = cp;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => false; // Cannot change to anything when dying.

    public CpSt_Death Enter(AnimInfo deathAnim) {
        ref Cp_CommonData commonData = ref cp.CommonData;
        commonData.ignoreHits = true;
        commonData.action_Died?.Invoke(cp);
        AnimEventPlr.CrossFadeInFixedTimeNInitAnimEventPlr(
            ref commonData.animEventPlrData,
            cp.anim,
            deathAnim,
            0.1f
        );
        return this;
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        switch (animEvent) {
            case CpAnimEventT.Finished:
                CpMgr.MarkForPendingUnregister(ref cp.CommonData);
                return;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                return;
        }
    }

    public void Tick() {
        ref var commonData = ref cp.CommonData;
        CpUtils.UpdateMovInputData(
            ref commonData,
            float2.zero,
            commonData.animDPose.position * commonData.lastKnockbackStr * cp.so_cpCommonData.knockbackStrMult,
            0,
            0,
            float.PositiveInfinity
        );
    }
}
