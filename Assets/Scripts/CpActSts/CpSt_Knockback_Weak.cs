using Unity.Mathematics;
using UnityEngine;

// TODO: This can be a generic knockback state. Animation should decide the "knockback type".
public class CpSt_Knockback_Weak : IFsmSt_Cp{
    int cpId;

    public CpSt_Knockback_Weak(int cpId) {
        this.cpId = cpId;
    }

    // TODO: Allow change to death state and maybe falling state. Basically all states where
    // TODO C: player loses control.
    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => typeof(TState) == typeof(CpSt_Knockback_Weak) ? true : false;

    // TODO: Maybe choose the animation before entering?
    public CpSt_Knockback_Weak Enter(AnimInfo knockbackAnimInfoFwd, AnimInfo knockbackAnimInfoBwd) {
        Cp_SoaData data = CpMgr.inst.soaData;
        Vector3 viewVec = new Vector3(
            data.lastRecievedHitDir[cpId].x,
            0,
            data.lastRecievedHitDir[cpId].z
        );
        // If you, for some reason, set the hit direction to Vector3.zero.
        if (viewVec.sqrMagnitude < 0.0001f)
            viewVec = Vector3.down;
        else
            viewVec.Normalize();
        if (Vector3.Dot(data.lastRecievedHitDir[cpId], CpMgr.inst.unityComps[cpId].rootTrf.forward) > 0) {
            AnimEventPlr.CrossfadeNInitAnimEventPlr(
                ref CpMgr.inst.animEventPlrData[cpId],
                CpMgr.inst.unityComps[cpId].anim,
                knockbackAnimInfoFwd,
                0.1f
            );
        }else
            AnimEventPlr.CrossfadeNInitAnimEventPlr(
                ref CpMgr.inst.animEventPlrData[cpId],
                CpMgr.inst.unityComps[cpId].anim,
                knockbackAnimInfoBwd,
                0.1f
            );
        return this;
    }

    public void Tick() {
        Cp_SoaData data = CpMgr.inst.soaData;
        //if (CpUtils.SwitchToFallingStIfNotGrounded(cpId))
        //    return;
        //Debug.Log($"knocback: {data.lastKnockbackStr[cpId]}\nanim delta: {data.animDPos[cpId]}");
        CpUtils.UpdateMovInputData(
            cpId,
            data,
            float2.zero,
            data.animDPos[cpId] * data.lastKnockbackStr[cpId],
            0,
            0,
            float.PositiveInfinity
        );
    }

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

    public void Exit() {}

    public void PhysicsTick() {}

    public void LateTick() {}
}
