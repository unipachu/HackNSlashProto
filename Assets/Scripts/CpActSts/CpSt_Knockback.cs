using Unity.Mathematics;
using UnityEngine;

public class CpSt_Knockback : IFsmSt_Cp{
    int cpId;

    public CpSt_Knockback(int cpId) {
        this.cpId = cpId;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => typeof(TState) == typeof(CpSt_Knockback)
            || typeof(TState) == typeof(CpSt_Death);

    public CpSt_Knockback Enter(AnimInfo knockbackAnimInfo) {
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cpId],
            CpMgr.inst.unityComps[cpId].anim,
            knockbackAnimInfo,
            0.1f
        );
        return this;
    }

    public void Exit() {}

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        switch (animEvent) {
            case CpAnimEventT.Finished:
                CpUtils.TransitionToFallIdleOrWalk(cpId);
                break;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                break;
        }
    }

    public void LateTick() {}

    public void PhysicsTick() {}

    public void Tick() {
        //if (CpUtils.SwitchToFallingStIfNotGrounded(cpId))
        //    return;
        //Debug.Log($"knocback: {data.lastKnockbackStr[cpId]}\nanim delta: {data.animDPos[cpId]}");
        CpUtils.UpdateMovInputData(
            cpId,
            float2.zero,
            CpMgr.GetAos(cpId).animDPos * CpMgr.GetAos(cpId).lastKnockbackStr,
            0,
            0,
            float.PositiveInfinity
        );
    }
}
