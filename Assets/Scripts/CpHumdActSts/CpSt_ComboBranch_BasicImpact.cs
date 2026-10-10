using System.Collections.Generic;
using UnityEngine;

public class CpSt_ComboBranch_BasicImpact : IFsmSt_Cp {
    IComboNode_CpHumanoid comboNode;
    CpHumdHandle cpHumd;
    IHitDealer hitDealer;
    HitEffects hitEffects;

    public CpSt_ComboBranch_BasicImpact(CpHumdHandle cpHumd) {
        this.cpHumd = cpHumd;
    }

    public CpSt_ComboBranch_BasicImpact Enter(
        HitEffects hitEffects,
        IComboNode_CpHumanoid comboNode,
        IHitDealer hitDealer
    ) {
        this.comboNode = comboNode;
        this.hitDealer = hitDealer;
        this.hitEffects = hitEffects;
        cpHumd.CommonData.comboAllowed = false;
        cpHumd.CommonData.yawAllowed = false;
        AnimEventPlr.CrossFadeInFixedTimeNInitAnimEventPlr(
            ref cpHumd.CommonData.animEventPlrData,
            cpHumd.anim,
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
                    CpUtils.TrySwitchActSt(
                        comboNode.GetNextNode(BufferableInput.None).GetEnterFunc(cpHumd),
                        ref cpHumd.CommonData,
                        true
                    );
                    return;
                }
                break;
            case CpAnimEventT.HitDealerActivated:
                //Debug.Log($"rHandEquippable null: {classRefs.rHandEquippable == null}");
                hitDealer.ResetNActivate(
                    cpHumd.lockOnTrf, // NOTE: = character center point.
                    HitDirMode.FromHitSourceTrfToHitReciever,
                    hitEffects,
                    new HashSet<IHitReceiver>{cpHumd.hitReciever},
                    cpHumd.CpCommonConfig.team,
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
        ref var commonData = ref cpHumd.CommonData;
        float angSpd = 0;
        if (commonData.yawAllowed)
            angSpd = cpHumd.CpCommonConfig.impact_YawSpd;
        CpUtils.UpdateMovInputData(
            ref commonData,
            commonData.input_mov,
            commonData.animDPose.position,
            0,
            angSpd,
            float.PositiveInfinity
        );
        if (CpHumdUtils.SwitchToFallingStIfNotGrounded(ref commonData, ref cpHumd.HumdData))
            return;
        if (commonData.comboAllowed && CpHumdUtils.TryAnyComboInputTransition(cpHumd, comboNode))
            return;
    }
}
