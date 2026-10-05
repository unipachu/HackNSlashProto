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
    public static Func<IFsmSt_Cp> FindStateEnterFunc(BufferableInput input, int cpI) {
        var classRefs = CpMgr.inst.aos[cpI].classRefs;
        if(input == BufferableInput.BtnE) {
            if(CpMgr.inst.aos[cpI].cooldownTimer_Dodge == 0)
                return () => classRefs.actSts.dodge.Enter();
            return null;
        }
        if (classRefs.rHandItem is IHandItem_Comboer comboer) {
            Func<IFsmSt_Cp> enter = input switch {
                BufferableInput.RShldr => GetEnterFunc(comboer.RShldrComboStart, cpI),
                BufferableInput.RTrg => GetEnterFunc(comboer.RTrgComboStart, cpI),
                BufferableInput.LShldr => GetEnterFunc(comboer.LShldrComboStart, cpI),
                _ => GeneralUtils.LogErrorForInput<BufferableInput, Func<IFsmSt_Cp>>(input)
            };
            if (enter != null)
                return enter;
        }
        // NOTE: Casting is the best option here. We could do a ECS-style "GetComponent", but for this
        // NOTE C: architecture, this is easier and not meaningfully less performant O(1).
        if (classRefs.rHandItem is IHandItem_Hitter hitter) {
            // TODO: Do not hard code hit effects!
            if(input == BufferableInput.LShldr)
                return () => classRefs.actSts.atk_FlyingAtk.Enter(
                    new HitEffects(3, HitT.Blunt, KnockbackT.Weak, 5),
                    hitter.HitDealer
                );
            // TODO: Do not hard code hit effects!
            if(input == BufferableInput.RTrg)
                return () => classRefs.actSts.atk_Jump.Enter(
                    new HitEffects(1, HitT.Blunt, KnockbackT.Weak, 1),
                    hitter.HitDealer
                );
        }
        return null;
        // Helper
        static Func<IFsmSt_Cp> GetEnterFunc(IComboNode comboStart, int cpI)
            => comboStart == null ? null : comboStart.GetEnterFunc(cpI);
    }

    /// <summary>
    /// Cooldown related and such conditions for state switching shared by most <see cref="IFsmSt.CanSwitchTo"/>.
    /// </summary>
    // TODO MINOR: I'm not using this for anyhitng...
    public static bool GeneralSwitchStConditions<TState>(CpHandle cp) where TState : IFsmSt
        => typeof(TState) != typeof(CpSt_Dodge)
            || cp.Data.cooldownTimer_Dodge <= 0f;

    /// <summary>
    /// Updates navMeshInfo if not already updated this tick and returns if the pawn is on the navmesh.
    /// </summary>
    public static bool IsOnNavMesh(int cpI) {
        var cpData = CpMgr.GetData(cpI);
        if (cpData.navTgtInfo.hasUpdatedNavTgtInfoThisTick)
            return cpData.navTgtInfo.isCpOnNavmesh;
        Transform trf = cpData.handle.transform;
        cpData.navTgtInfo.hasUpdatedNavTgtInfoThisTick = true;
        cpData.navTgtInfo.isCpOnNavmesh = NavMesh.SamplePosition(
            trf.position,
            out NavMeshHit hit,
            cpData.navTgtInfo.maxDistToNavMesh,
            cpData.handle.navMeshAgent.areaMask
        );
        return cpData.navTgtInfo.isCpOnNavmesh;
    }

    /// <summary>
    /// True if switched.
    /// </summary>
    public static bool SwitchToFallingStIfNotGrounded(int cpI) {
        var classRefs = CpMgr.inst.aos[cpI].classRefs;
        if (
            !CpMgr.GetData(cpI).isGrounded
            && classRefs.st_cur.GetType() != typeof(CpSt_Falling)
        ) {
            //Debug.Log($"{id} was not grounded so switch to falling st!");
            CpMgr.inst.TrySwitchActSt(() => classRefs.actSts.falling.Enter(), cpI, true);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Used to transition to a baic action state after an attack/special move etc.
    /// </summary>
    public static void TransitionToFallIdleOrWalk(int cpI) {
        var classRefs = CpMgr.inst.aos[cpI].classRefs;
        SwitchToFallingStIfNotGrounded(cpI);
        if (math.all(CpMgr.GetData(cpI).input_mov != float2.zero))
            CpMgr.inst.TrySwitchActSt(() => classRefs.actSts.walk.Enter(), cpI, true);
        else
            CpMgr.inst.TrySwitchActSt(() => classRefs.actSts.idle.Enter(), cpI, true);
    }

    /// <summary>
    /// Can be used from neutral states like "walk" or "idle" to transition to new states with input.<br/>
    /// Returns true if succeeded changing state.
    /// </summary>
    public static bool TrySwitchStFromNeutralStByBufferedInput(int cpI) {
        BufferableInput input = CpMgr.GetData(cpI).inputBuffer_BufferedInput;
        if(input == BufferableInput.None)
            return false;
        Func<IFsmSt_Cp> enterFunc = FindStateEnterFunc(input, cpI);
        if (
            enterFunc != null
                && InputBufferUtils.TryConsumeInput(
                    input,
                    ref CpMgr.GetData(cpI).inputBuffer_BufferedInput,
                    ref CpMgr.GetData(cpI).inputBuffer_RemainingTime
                )
        ) {
            CpMgr.inst.TrySwitchActSt(enterFunc, cpI, true);
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
        int cpI,
        float2 tgtHorDir,
        float3 additionalLinMov,
        float tgtHorSpd,
        float yawSpd,
        float horAcc
    ) {
        CpMgr.GetData(cpI).movInput_tgtHorDir = tgtHorDir;
        CpMgr.GetData(cpI).movInput_additionalLinMov = additionalLinMov;
        CpMgr.GetData(cpI).movInput_tgtHorSpd = tgtHorSpd;
        CpMgr.GetData(cpI).movInput_yawSpd = yawSpd;
        CpMgr.GetData(cpI).movInput_horAcc = horAcc;
    }

    /// <summary>
    /// Transitions to any existing next combo node that require input if such input was buffered.
    /// Immediately returns true if successfully switched state.
    /// </summary>
    public static bool TryAnyComboInputTransition(int cpI, IComboNode curComboNode)
    => TryComboTransition(BufferableInput.RShldr, curComboNode, cpI)
        || TryComboTransition(BufferableInput.RTrg, curComboNode, cpI)
        || TryComboTransition(BufferableInput.BtnE, curComboNode, cpI)
        || TryComboTransition(BufferableInput.LShldr, curComboNode, cpI);

    /// <summary>
    /// Returns true if successfully transitioned to the next action state of the combo.
    /// </summary>
    static bool TryComboTransition(BufferableInput input, IComboNode curComboNode, int cpI) {
        if (
            curComboNode.GetNextNode(input) != null
                && InputBufferUtils.TryConsumeInput(
                    input,
                    ref CpMgr.GetData(cpI).inputBuffer_BufferedInput,
                    ref CpMgr.GetData(cpI).inputBuffer_RemainingTime
                )
        ) {
            CpMgr.inst.TrySwitchActSt(curComboNode.GetNextNode(input).GetEnterFunc(cpI), cpI, true);
            return true;
        }
        return false;
    }
}
