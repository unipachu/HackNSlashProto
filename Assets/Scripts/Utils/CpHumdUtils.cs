using System;
using UnityEngine;

public static class CpHumdUtils{
    public static AnimInfo FindKnockbackAnim(Cp_CommonData commonData) {
        if (CpUtils.FindKnockBackDir(commonData) == Dir2DHor.Forward)
            // TODO MAYBE: Create different animation for "strong knockback".
            return CpHumdAnimInfoFactory.Construct(CpHumanoidAnimInfoT.knockback_Weak_Fwd);
        return CpHumdAnimInfoFactory.Construct(CpHumanoidAnimInfoT.knockback_Weak_Bwd);
    }

    /// <summary>
    /// Finds next state to transition to based on input and held items. Returns null if no applicable
    /// state found.<br/>
    /// NOTE: Use this when transitioning from neutral states like walk or idle. For combo chain transitions,
    /// use: <see cref="TryComboTransition"/>. (6.9.2026)
    /// </summary>
    public static Func<IFsmSt_Cp> FindStateEnterFunc(
        ref Cp_CommonData commonData,
        ref CpHumd_Data humdData,
        BufferableInput input
    ) {
        var humdClassRefs = humdData.classRefs;
        if (input == BufferableInput.B) {
            if (humdData.cooldownTimer_Dodge == 0)
                return () => humdClassRefs.actSts.dodge.Enter();
            return null;
        }
        if (humdClassRefs.rHandItem is IHandItem_Comboer comboer) {
            Func<IFsmSt_Cp> enter = input switch {
                BufferableInput.Rb => GetEnterFunc(comboer.RShldrComboStart, humdData.handle),
                BufferableInput.Rt => GetEnterFunc(comboer.RTrgComboStart, humdData.handle),
                BufferableInput.Lb => GetEnterFunc(comboer.LShldrComboStart, humdData.handle),
                _ => GeneralUtils.LogErrorForInput<BufferableInput, Func<IFsmSt_Cp>>(input)
            };
            if (enter != null)
                return enter;
        }
        // NOTE: Casting is the best option here. We could do a ECS-style "GetComponent", but for this
        // NOTE C: architecture, this is easier and not meaningfully less performant O(1).
        if (humdClassRefs.rHandItem is IHandItem_Hitter hitter) {
            // TODO: Do not hard code hit effects!
            if (input == BufferableInput.Lb) {
                // TODO: ehh, this method is supposed to be generic for all CPs but now it uses PlrMgr...
                if (PlrMgr.inst.TryConsumeUltMeter())
                    return () => humdClassRefs.actSts.atk_FlyingAtk.Enter(
                        new HitEffects(3, HitT.Blunt, KnockbackT.Strong, 5),
                        hitter.HitDealer
                    );
                return null;
            }
            // TODO: Do not hard code hit effects!
            if (input == BufferableInput.Rt)
                return () => humdClassRefs.actSts.atk_Jump.Enter(
                    new HitEffects(1, HitT.Blunt, KnockbackT.Strong, 1),
                    hitter.HitDealer
                );
        }
        return null;
        // Helper
        static Func<IFsmSt_Cp> GetEnterFunc(IComboNode_CpHumanoid comboStart, CpHumdHandle cpHumd)
            => comboStart == null ? null : comboStart.GetEnterFunc(cpHumd);
    }

    /// <summary>
    /// Cooldown related and such conditions for state switching shared by most <see cref="IFsmSt.CanSwitchTo"/>.
    /// </summary>
    // TODO MINOR: I'm not using this for anyhitng...
    public static bool GeneralSwitchStConditions<TState>(CpHumdHandle cp) where TState : IFsmSt
        => typeof(TState) != typeof(CpSt_Dodge)
            || cp.HumdData.cooldownTimer_Dodge <= 0f;
    /// <summary>
    /// True if switched.
    /// </summary>
    public static bool SwitchToFallingStIfNotGrounded(
        ref Cp_CommonData commonData,
        ref CpHumd_Data humanoidData
    ) {
        var classRefs = humanoidData.classRefs;
        if (
            !commonData.isGrounded
            && commonData.classRefs.st_cur.GetType() != typeof(CpSt_Falling)
        ) {
            //Debug.Log($"{id} was not grounded so switch to falling st!");
            CpUtils.TrySwitchActSt(() => classRefs.actSts.falling.Enter(), ref commonData, true);
            return true;
        }
        return false;
    }

    public static void OnAnimEvent(CpHumdHandle cpHumd, CpAnimEventT animEvent) {
        //Debug.Log($"Anim event {animEvent} for {id} called!", this);
        ref var commonData = ref cpHumd.CommonData;
        ref var humdData = ref cpHumd.HumdData;
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
            case CpAnimEventT.DodgeAllowed:
                humdData.dodgeAllowed = true;
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
                commonData.vel_Ver = -cpHumd.HumdConfig.act_AtkJump_DownSpeedAfterJumpFinished;
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
                Debug.LogError($"Switch defaulted with {animEvent}.", cpHumd);
                break;
        }
    }

    /// <summary>
    /// Used to transition to a baic action state after an attack/special move etc.
    /// </summary>
    public static void TransitionToFallIdleOrWalk(
        ref Cp_CommonData commonData,
        ref CpHumd_Data humanoidData
    ) {
        var classRefs = humanoidData.classRefs;
        SwitchToFallingStIfNotGrounded(ref commonData, ref humanoidData);
        if (commonData.input_mov.IsZeroOrNearlyZero())
            CpUtils.TrySwitchActSt(() => classRefs.actSts.idle.Enter(), ref commonData, true);
        else
            CpUtils.TrySwitchActSt(() => classRefs.actSts.walk.Enter(), ref commonData, true);
    }

    /// <summary>
    /// Transitions to any existing next combo node that require input if such input was buffered.
    /// Immediately returns true if successfully switched state.
    /// </summary>
    public static bool TryAnyComboInputTransition(CpHumdHandle cpHumanoid, IComboNode_CpHumanoid curComboNode)
        => TryComboTransition(cpHumanoid, BufferableInput.Rb, curComboNode)
            || TryComboTransition(cpHumanoid, BufferableInput.Rt, curComboNode)
            || TryComboTransition(cpHumanoid, BufferableInput.B, curComboNode)
            || TryComboTransition(cpHumanoid, BufferableInput.Lb, curComboNode);

    /// <summary>
    /// Returns true if successfully transitioned to the next action state of the combo.
    /// </summary>
    static bool TryComboTransition(
        CpHumdHandle cpHumanoid,
        BufferableInput input,
        IComboNode_CpHumanoid curComboNode
    ) {
        if (
            curComboNode.GetNextNode(input) != null
                && InputBufferUtils.TryConsumeInput(
                    input,
                    ref cpHumanoid.CommonData.inputBuffer_BufferedInput,
                    ref cpHumanoid.CommonData.inputBuffer_RemainingTime
                )
        ) {
            CpUtils.TrySwitchActSt(
                curComboNode.GetNextNode(input).GetEnterFunc(cpHumanoid),
                ref cpHumanoid.CommonData,
                true
            );
            return true;
        }
        return false;
    }

    /// <summary>
    /// Can be used from neutral states like "walk" or "idle" to transition to new states with input.<br/>
    /// Returns true if succeeded changing state.
    /// </summary>
    public static bool TrySwitchStFromNeutralStByBufferedInput(
        ref Cp_CommonData commonData,
        ref CpHumd_Data humdData
    ) {
        BufferableInput input = commonData.inputBuffer_BufferedInput;
        if (input == BufferableInput.None)
            return false;
        Func<IFsmSt_Cp> enterFunc = FindStateEnterFunc(ref commonData, ref humdData, input);
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
