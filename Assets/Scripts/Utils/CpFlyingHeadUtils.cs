using System;
using UnityEngine;

public static class CpFlyingHeadUtils{
    public static AnimInfo FindKnockbackAnim(Cp_CommonData commonData) {
        if (CpUtils.FindKnockBackDir(commonData) == Dir2DHor.Forward)
            // TODO MAYBE: Create different animation for "strong knockback".
            return CpFlyingHeadAnimInfoFactory.Construct(CpFlyingHeadAnimInfoT.knockback_Weak_Fwd);
        return CpFlyingHeadAnimInfoFactory.Construct(CpFlyingHeadAnimInfoT.knockback_Weak_Bwd);
    }

    /// <summary>
    /// Finds next state to transition to based on input and combo graph. Returns null if no applicable
    /// state found.<br/>
    /// </summary>
    public static Func<IFsmSt_Cp> FindStateEnterFunc(
        ref Cp_CommonData commonData,
        ref CpFlyingHead_Data flyingHeadData,
        BufferableInput input
    ) {
        var atk_Dash = flyingHeadData.actSts.atk_Dash;
        var hitDealer = flyingHeadData.handle.hitDealer;
        if (input == BufferableInput.Rb) {
            return () => atk_Dash.Enter(
                // TODO: So
                new HitEffects(1, HitT.Blunt, KnockbackT.Weak, 1),
                hitDealer
            );
        }
        return null;
    }

    public static void OnAnimEvent(CpFlyingHeadHandle cpFlyingHead, CpAnimEventT animEvent) {
        //Debug.Log($"Anim event {animEvent} for {id} called!", this);
        ref var commonData = ref cpFlyingHead.CommonData;
        switch (animEvent) {
            case CpAnimEventT.BufferedInputStSwitchAllowed:
                commonData.bufferedInputStSwitchAllowed = true;
                break;
            case CpAnimEventT.ComboAllowed:
                commonData.comboAllowed = true;
                break;
            case CpAnimEventT.ComboDisallowed:
                commonData.comboAllowed = false;
                break;
            case CpAnimEventT.Finished:
                commonData.classRefs.st_cur.HandleAnimEvent(CpAnimEventT.Finished);
                break;
            case CpAnimEventT.HitDealerActivated:
                commonData.classRefs.st_cur.HandleAnimEvent(CpAnimEventT.HitDealerActivated);
                break;
            case CpAnimEventT.HitDealerDeactivated:
                commonData.classRefs.st_cur.HandleAnimEvent(CpAnimEventT.HitDealerDeactivated);
                break;
            case CpAnimEventT.InputMovAllowed:
                commonData.inputMovAllowed = true;
                break;
            case CpAnimEventT.InvulEnd:
                commonData.ignoreHits = false;
                break;
            case CpAnimEventT.AirtimeEnded:
                commonData.isAffectedByGravity = true;
                break;
            case CpAnimEventT.AirtimeStarted:
                commonData.isAffectedByGravity = false;
                break;
            case CpAnimEventT.YawAllowed:
                commonData.yawAllowed = true;
                break;
            case CpAnimEventT.YawDisallowed:
                commonData.yawAllowed = false;
                break;
            default:
                Debug.LogError($"Switch defaulted with {animEvent}.", cpFlyingHead);
                break;
        }
    }

    /// <summary>
    /// Used to transition to a baic action state after an attack/special move etc.
    /// </summary>
    public static void TransitionToIdleOrFly(
        ref Cp_CommonData commonData,
        ref CpFlyingHead_Data flyingHeadData
    ) {
        var actSts = flyingHeadData.actSts;
        if (commonData.input_mov.IsZeroOrNearlyZero())
            CpUtils.TrySwitchActSt(() => actSts.idle.Enter(), ref commonData, true);
        else
            CpUtils.TrySwitchActSt(() => actSts.fly.Enter(), ref commonData, true);
    }

    public static bool TrySwitchStFromNeutralStByBufferedInput(
        ref Cp_CommonData commonData,
        ref CpFlyingHead_Data flyingHeadData
    ) {
        BufferableInput input = commonData.inputBuffer_BufferedInput;
        if (input == BufferableInput.None)
            return false;
        Func<IFsmSt_Cp> enterFunc = FindStateEnterFunc(ref commonData, ref flyingHeadData, input);
        if (
            enterFunc != null
                && InputBufferUtils.TryConsumeInput(
                    input,
                    ref commonData.inputBuffer_BufferedInput,
                    ref commonData.inputBuffer_RemainingTime
                )
        ) {
            CpUtils.TrySwitchActSt(enterFunc, ref commonData, true);
            return true;
        }
        return false;
    }
}
