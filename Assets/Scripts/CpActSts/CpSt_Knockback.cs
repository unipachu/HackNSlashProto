using Unity.Mathematics;
using UnityEngine;

public class CpSt_Knockback : IFsmSt_Cp{
    CpHandle cp;

    public CpSt_Knockback(CpHandle cp) {
        this.cp = cp;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt {
        //Debug.Log($"type of TState: {typeof(TState).Name}");
        return typeof(TState) == typeof(CpSt_Knockback)
            || typeof(TState) == typeof(CpSt_Death);
    }

    public CpSt_Knockback Enter(AnimInfo knockbackAnimInfo) {
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.aos[cp.I].animEventPlrData,
            CpMgr.inst.aos[cp.I].unityObjs.anim,
            knockbackAnimInfo,
            0.1f
        );
        return this;
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        switch (animEvent) {
            case CpAnimEventT.Finished:
                CpUtils.TransitionToFallIdleOrWalk(cp.I);
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
        CpUtils.UpdateMovInputData(
            cp.I,
            float2.zero,
            cp.Data.animDPos * cp.Data.lastKnockbackStr,
            0,
            0,
            float.PositiveInfinity
        );
    }
}
