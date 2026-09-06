using Unity.Mathematics;
using UnityEngine;

public class CpSt_Atk_Jump : IFsmSt_Cp {
    int cpId;
    HitDealer hitDealer;

    public CpSt_Atk_Jump(int cpId) {
        this.cpId = cpId;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => typeof(TState) == typeof(CpSt_Falling) ? false : true;

    public CpSt_Atk_Jump Enter() {
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.animEventPlrData[cpId],
            CpMgr.inst.unityComps[cpId].anim,
            CpAnimInfo.atk_JumpVerSlam,
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
                CpUtils.TransitionToFallIdleOrWalk(cpId);
                break;
            case CpAnimEventT.HitDealerActivated:
            //    // TODO: Item
            //    //unityComps.rHandItem.hitDealer.atkData = new(1, KnockbackT.Weak, 1);
            //    //unityComps.rHandItem.hitDealer.hitWldDir = unityComps.trf.forward;
            //    //unityComps.rHandItem.hitDealer.Activate();
            case CpAnimEventT.HitDealerDeactivated:
            //    //unityComps.rHandItem.hitDealer.Deactivate();
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
        CpUtils.UpdateMovData(
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
