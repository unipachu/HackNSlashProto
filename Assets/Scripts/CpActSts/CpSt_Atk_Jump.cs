using Unity.Mathematics;
using UnityEngine;

public class CpSt_Atk_Jump : IFsmSt_Cp {
    int cpId;
    HitDealer hitDealer;
    HitEffects hitEffects;

    public CpSt_Atk_Jump(int cpId) {
        this.cpId = cpId;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => typeof(TState) == typeof(CpSt_Falling) ? false : true;

    public CpSt_Atk_Jump Enter(HitEffects hitEffects, HitDealer hitDealer) {
        this.hitEffects = hitEffects;
        this.hitDealer = hitDealer;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cpId],
            CpMgr.inst.unityComps[cpId].anim,
            CpAnimInfo.Get(CpAnimInfoT.atk_JumpVerSlam),
            0.1f
        );
        return this;
    }

    public void Exit() {
        Cp_SoaData data = CpMgr.inst.soaData;
        data.isAffectedByGravity[cpId] = true;
        hitDealer.Deactivate();
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        var unityComps = CpMgr.inst.unityComps[cpId];
        Cp_SoaData data = CpMgr.inst.soaData;
        switch (animEvent) {
            case CpAnimEventT.AirtimeEnded:
                data.isAffectedByGravity[cpId] = true;
                data.vel_Ver[cpId] = -data.st_AtkJump_DownSpeedAfterJumpFinished[cpId];
                break;
            case CpAnimEventT.AirtimeStarted:
                data.isAffectedByGravity[cpId] = false;
                break;
            case CpAnimEventT.Finished:
                Debug.Log("Went here asdasdasd");
                CpUtils.TransitionToFallIdleOrWalk(cpId);
                break;
            case CpAnimEventT.HitDealerActivated:
                hitDealer.hitEffects = hitEffects;
                hitDealer.hitWldDir = unityComps.rootTrf.forward;
                hitDealer.Activate();
                break;
            case CpAnimEventT.HitDealerDeactivated:
                hitDealer.Deactivate();
                break;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}");
                break;
        }
    }

    public void LateTick() {}

    public void PhysicsTick() {}

    public void Tick() {
        Cp_SoaData data = CpMgr.inst.soaData;
        Cp_UnityComps[] unityComps = CpMgr.inst.unityComps;
        CpUtils.UpdateMovInputData(
            cpId,
            data,
            float2.zero,
            data.animDPos[cpId],
            0,
            0,
            float.PositiveInfinity
        );
    }
}
