using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Capsule pawn general util methods. Consider organizing these better!
/// </summary>
public static class CpUtils{
    /// <summary>
    /// Finds next state to transition to based on input and held items. Returns null if no applicable
    /// state found.<br/>
    /// NOTE: Use this when transitioning from neutral states like walk or idle. For combo chain transitions,
    /// use: <see cref="TryComboTransition"/>. (6.9.2026)
    /// </summary>
    public static Func<IFsmSt_Cp> CpHumd_FindStateEnterFunc(
        ref Cp_CommonData commonData,
        ref CpHumd_Data humdData,
        BufferableInput input
    ) {
        var humdClassRefs = humdData.classRefs;
        if(input == BufferableInput.BtnE) {
            if(humdData.cooldownTimer_Dodge == 0)
                return () => humdClassRefs.actSts.dodge.Enter();
            return null;
        }
        if (humdClassRefs.rHandItem is IHandItem_Comboer comboer) {
            Func<IFsmSt_Cp> enter = input switch {
                BufferableInput.RShldr => GetEnterFunc(comboer.RShldrComboStart, humdData.handle),
                BufferableInput.RTrg => GetEnterFunc(comboer.RTrgComboStart, humdData.handle),
                BufferableInput.LShldr => GetEnterFunc(comboer.LShldrComboStart, humdData.handle),
                _ => GeneralUtils.LogErrorForInput<BufferableInput, Func<IFsmSt_Cp>>(input)
            };
            if (enter != null)
                return enter;
        }
        // NOTE: Casting is the best option here. We could do a ECS-style "GetComponent", but for this
        // NOTE C: architecture, this is easier and not meaningfully less performant O(1).
        if (humdClassRefs.rHandItem is IHandItem_Hitter hitter) {
            // TODO: Do not hard code hit effects!
            if(input == BufferableInput.LShldr) {
                // TODO: ehh, this method is supposed to be generic for all CPs but now it uses PlrMgr...
                if(PlrMgr.inst.TryConsumeUltMeter())
                    return () => humdClassRefs.actSts.atk_FlyingAtk.Enter(
                        new HitEffects(3, HitT.Blunt, KnockbackT.Strong, 5),
                        hitter.HitDealer
                    );
                return null;
            }
            // TODO: Do not hard code hit effects!
            if(input == BufferableInput.RTrg)
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
    /// Updates navMeshInfo if not already updated this tick and returns if the pawn is on the navmesh.
    /// </summary>
    public static bool IsOnNavMesh(ref Cp_CommonData commonData) {
        if (commonData.navTgtInfo.hasUpdatedNavTgtInfoThisTick)
            return commonData.navTgtInfo.isCpOnNavmesh;
        Transform trf = commonData.handle.Go.transform;
        commonData.navTgtInfo.hasUpdatedNavTgtInfoThisTick = true;
        commonData.navTgtInfo.isCpOnNavmesh = NavMesh.SamplePosition(
            trf.position,
            out NavMeshHit hit,
            commonData.navTgtInfo.maxDistToNavMesh,
            commonData.handle.NavMeshAgent.areaMask
        );
        return commonData.navTgtInfo.isCpOnNavmesh;
    }

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
            CpHumdMgr.TrySwitchActSt(() => classRefs.actSts.falling.Enter(), ref commonData, true);
            return true;
        }
        return false;
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
        if (math.all(commonData.input_mov != float2.zero))
            CpHumdMgr.TrySwitchActSt(() => classRefs.actSts.walk.Enter(), ref commonData, true);
        else
            CpHumdMgr.TrySwitchActSt(() => classRefs.actSts.idle.Enter(), ref commonData, true);
    }

    /// <summary>
    /// Can be used from neutral states like "walk" or "idle" to transition to new states with input.<br/>
    /// Returns true if succeeded changing state.
    /// </summary>
    public static bool CpHumd_TrySwitchStFromNeutralStByBufferedInput(
        ref Cp_CommonData commonData,
        ref CpHumd_Data humdData
    ) {
        BufferableInput input = commonData.inputBuffer_BufferedInput;
        if(input == BufferableInput.None)
            return false;
        Func<IFsmSt_Cp> enterFunc = CpHumd_FindStateEnterFunc(ref commonData, ref humdData, input);
        if (
            enterFunc != null
                && InputBufferUtils.TryConsumeInput(
                    input,
                    ref commonData.inputBuffer_BufferedInput,
                    ref commonData.inputBuffer_RemainingTime
                )
        ) {
            CpHumdMgr.TrySwitchActSt(enterFunc, ref commonData, true);
            return true;
        }
        return false;
    }

    /// <summary>
    /// NOTE: controller inputs do not directly affect movement data - instead they're read by the action
    /// state of the pawn which then sends inputs to the movement system with this method.
    /// </summary>
    // You could combine tgtHorDir and tgtHorSpd to tgtHorVel
    public static void UpdateMovInputData(
        ref Cp_CommonData commonData,
        Vector2 tgtHorDir,
        Vector3 additionalLinMov, // NOTE: Currently only linear movement is used.
        float tgtHorSpd,
        float yawSpd,
        float horAcc
    ) {
        commonData.movInput_tgtHorDir = tgtHorDir;
        commonData.movInput_additionalLinMov = additionalLinMov;
        commonData.movInput_tgtHorSpd = tgtHorSpd;
        commonData.movInput_yawSpd = yawSpd;
        commonData.movInput_horAcc = horAcc;
    }

    /// <summary>
    /// Transitions to any existing next combo node that require input if such input was buffered.
    /// Immediately returns true if successfully switched state.
    /// </summary>
    public static bool TryAnyComboInputTransition(CpHumdHandle cpHumanoid, IComboNode_CpHumanoid curComboNode)
        => TryComboTransition(cpHumanoid, BufferableInput.RShldr, curComboNode)
            || TryComboTransition(cpHumanoid, BufferableInput.RTrg, curComboNode)
            || TryComboTransition(cpHumanoid, BufferableInput.BtnE, curComboNode)
            || TryComboTransition(cpHumanoid, BufferableInput.LShldr, curComboNode);

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
            CpHumdMgr.TrySwitchActSt(
                curComboNode.GetNextNode(input).GetEnterFunc(cpHumanoid),
                ref cpHumanoid.CommonData,
                true
            );
            return true;
        }
        return false;
    }
}
