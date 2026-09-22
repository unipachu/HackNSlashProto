using Unity.Mathematics;
using UnityEngine;

public class CpSt_Atk_Jump : IFsmSt_Cp {
    CpHandle cp;
    HitDealer hitDealer;
    HitEffects hitEffects;

    public CpSt_Atk_Jump(CpHandle cp) {
        this.cp = cp;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => typeof(TState) == typeof(CpSt_Falling) ? false : true;

    public CpSt_Atk_Jump Enter(HitEffects hitEffects, HitDealer hitDealer) {
        this.hitEffects = hitEffects;
        this.hitDealer = hitDealer;
        AnimEventPlr.CrossfadeNInitAnimEventPlr(
            ref CpMgr.inst.aos[cp.I].animEventPlrData,
            CpMgr.inst.aos[cp.I].unityComps.anim,
            CpAnimInfoFactory.Construct(CpAnimInfoT.atk_JumpVerSlam),
            0.1f
        );
        return this;
    }

    public void Exit() {
        cp.Data.isAffectedByGravity = true;
        hitDealer.Deactivate();
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        var unityComps = CpMgr.inst.aos[cp.I].unityComps;
        switch (animEvent) {
            case CpAnimEventT.AirtimeEnded:
                cp.Data.isAffectedByGravity = true;
                cp.Data.vel_Ver = -cp.Data.act_AtkJump_DownSpeedAfterJumpFinished;
                break;
            case CpAnimEventT.AirtimeStarted:
                cp.Data.isAffectedByGravity = false;
                break;
            case CpAnimEventT.Finished:
                Debug.Log("Went here asdasdasd");
                CpUtils.TransitionToFallIdleOrWalk(cp.I);
                break;
            case CpAnimEventT.HitDealerActivated:
                hitDealer.hitData = new HitData(hitEffects, cp.Data.team, cp.transform.forward);
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

    public void Tick() {
        CpUtils.UpdateMovInputData(
            cp.I,
            float2.zero,
            cp.Data.animDPos,
            0,
            0,
            float.PositiveInfinity
        );
    }
}
