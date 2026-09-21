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
            ref CpMgr.inst.animEventPlrData[cp.Id],
            CpMgr.inst.unityComps[cp.Id].anim,
            CpAnimInfoFactory.Construct(CpAnimInfoT.atk_JumpVerSlam),
            0.1f
        );
        return this;
    }

    public void Exit() {
        CpMgr.GetAos(cp.Id).isAffectedByGravity = true;
        hitDealer.Deactivate();
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        var unityComps = CpMgr.inst.unityComps[cp.Id];
        switch (animEvent) {
            case CpAnimEventT.AirtimeEnded:
                CpMgr.GetAos(cp.Id).isAffectedByGravity = true;
                CpMgr.GetAos(cp.Id).vel_Ver = -CpMgr.GetAos(cp.Id).act_AtkJump_DownSpeedAfterJumpFinished;
                break;
            case CpAnimEventT.AirtimeStarted:
                CpMgr.GetAos(cp.Id).isAffectedByGravity = false;
                break;
            case CpAnimEventT.Finished:
                Debug.Log("Went here asdasdasd");
                CpUtils.TransitionToFallIdleOrWalk(cp.Id);
                break;
            case CpAnimEventT.HitDealerActivated:
                hitDealer.hitData = new HitData(hitEffects, CpMgr.GetAos(cp.Id).team, cp.transform.forward);
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
        Cp_UnityObjs[] unityComps = CpMgr.inst.unityComps;
        CpUtils.UpdateMovInputData(
            cp.Id,
            float2.zero,
            CpMgr.GetAos(cp.Id).animDPos,
            0,
            0,
            float.PositiveInfinity
        );
    }
}
