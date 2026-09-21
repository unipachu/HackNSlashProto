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
            ref CpMgr.inst.animEventPlrData[cp.Id],
            CpMgr.inst.unityComps[cp.Id].anim,
            knockbackAnimInfo,
            0.1f
        );
        return this;
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        switch (animEvent) {
            case CpAnimEventT.Finished:
                CpUtils.TransitionToFallIdleOrWalk(cp.Id);
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
            cp.Id,
            float2.zero,
            CpMgr.GetAos(cp.Id).animDPos * CpMgr.GetAos(cp.Id).lastKnockbackStr,
            0,
            0,
            float.PositiveInfinity
        );
    }
}
