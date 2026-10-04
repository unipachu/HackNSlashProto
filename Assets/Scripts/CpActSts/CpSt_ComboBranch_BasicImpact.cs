using System.Collections.Generic;
using UnityEngine;

public class CpSt_ComboBranch_BasicImpact : IFsmSt_Cp {
    IComboNode comboNode;
    CpHandle cp;
    IHitDealer hitDealer;
    HitEffects hitEffects;

    public CpSt_ComboBranch_BasicImpact(CpHandle cp) {
        this.cp = cp;
    }

    public CpSt_ComboBranch_BasicImpact Enter(
        HitEffects hitEffects,
        IComboNode comboNode,
        IHitDealer hitDealer
    ) {
        this.comboNode = comboNode;
        this.hitDealer = hitDealer;
        this.hitEffects = hitEffects;
        cp.Data.comboAllowed = false;
        cp.Data.yawAllowed = false;
        AnimEventPlr.CrossFadeInFixedTimeNInitAnimEventPlr(
            ref CpMgr.inst.aos[cp.I].animEventPlrData,
            cp.anim,
            comboNode.AnimInfo,
            0.1f
        );
        return this;
    }

    public bool CanSwitchTo<TState>() where TState : IFsmSt
        => true;

    public void Exit() {
        hitDealer.Deactivate();
    }

    public void HandleAnimEvent(CpAnimEventT animEvent) {
        switch (animEvent) {
            case CpAnimEventT.Finished:
                if (comboNode.GetNextNode(BufferableInput.None) != null) {
                    CpMgr.inst.TrySwitchActSt(
                        comboNode.GetNextNode(BufferableInput.None).GetEnterFunc(cp.I),
                        cp.I,
                        true
                    );
                    return;
                }
                break;
            case CpAnimEventT.HitDealerActivated:
                //Debug.Log($"rHandEquippable null: {classRefs.rHandEquippable == null}");
                hitDealer.ResetNActivate(
                    cp.lockOnTrf, // NOTE: = character center point.
                    HitDirMode.FromHitSourceTrfToHitReciever,
                    hitEffects,
                    new HashSet<IHitReceiver>{cp.hitReciever},
                    cp.so_cpData.team,
                    Vector3.zero
                );
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
        ref var cpData = ref cp.Data;
        float angSpd = 0;
        if (cpData.yawAllowed)
            angSpd = cpData.handle.so_cpData.impact_YawSpd;
        CpUtils.UpdateMovInputData(
            cp.I,
            cpData.input_mov,
            cpData.animDPos,
            0,
            angSpd,
            float.PositiveInfinity
        );
        if (CpUtils.SwitchToFallingStIfNotGrounded(cp.I))
            return;
        if (cpData.comboAllowed && CpUtils.TryAnyComboInputTransition(cp.I, comboNode))
            return;
    }
}
