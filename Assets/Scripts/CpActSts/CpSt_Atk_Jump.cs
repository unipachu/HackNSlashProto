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
            ref CpMgr.inst.animEventPlrData[cp.I],
            CpMgr.inst.unityComps[cp.I].anim,
            CpAnimInfoFactory.Construct(CpAnimInfoT.atk_JumpVerSlam),
            0.1f
        );
        return this;
    }

    public void Exit() {
        CpMgr.GetData(cp.I).isAffectedByGravity = true;
        hitDealer.Deactivate();
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        var unityComps = CpMgr.inst.unityComps[cp.I];
        switch (animEvent) {
            case CpAnimEventT.AirtimeEnded:
                CpMgr.GetData(cp.I).isAffectedByGravity = true;
                CpMgr.GetData(cp.I).vel_Ver = -CpMgr.GetData(cp.I).act_AtkJump_DownSpeedAfterJumpFinished;
                break;
            case CpAnimEventT.AirtimeStarted:
                CpMgr.GetData(cp.I).isAffectedByGravity = false;
                break;
            case CpAnimEventT.Finished:
                Debug.Log("Went here asdasdasd");
                CpUtils.TransitionToFallIdleOrWalk(cp.I);
                break;
            case CpAnimEventT.HitDealerActivated:
                hitDealer.hitData = new HitData(hitEffects, CpMgr.GetData(cp.I).team, cp.transform.forward);
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
            cp.I,
            float2.zero,
            CpMgr.GetData(cp.I).animDPos,
            0,
            0,
            float.PositiveInfinity
        );
    }
}
