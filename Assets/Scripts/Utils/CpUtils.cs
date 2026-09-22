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
        var classRefs = CpMgr.inst.classRefs[cpI];
        var unityComps = CpMgr.inst.unityComps[cpI];
        if(input == BufferableInput.BtnE)
            return () => classRefs.actSts.dodge.Enter();
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
            if(input == BufferableInput.LShldr)
                return () => classRefs.actSts.atk_FlyingAtk.Enter(
                    new HitEffects(1, KnockbackT.Weak, 5),
                    hitter.HitDealer
                );
            if(input == BufferableInput.RTrg)
                return () => classRefs.actSts.atk_Jump.Enter(
                    new HitEffects(1, KnockbackT.Weak, 1),
                    hitter.HitDealer
                );
        }
        return null;
        // Helper
        static Func<IFsmSt_Cp> GetEnterFunc(IComboNode comboStart, int cpI)
            => comboStart == null ? null : comboStart.GetEnterFunc(cpI);
    }

    /// <summary>
    /// Updates navMeshInfo if not already updated this tick and returns if the pawn is on the navmesh.
    /// </summary>
    public static bool IsOnNavMesh(int cpI) {
        if (CpMgr.GetData(cpI).navTgtInfo.hasUpdatedNavTgtInfoThisTick)
            return CpMgr.GetData(cpI).navTgtInfo.isCpOnNavmesh;
        Transform trf = CpMgr.inst.handle[cpI].transform;
        CpMgr.GetData(cpI).navTgtInfo.hasUpdatedNavTgtInfoThisTick = true;
        CpMgr.GetData(cpI).navTgtInfo.isCpOnNavmesh = NavMesh.SamplePosition(
            trf.position,
            out NavMeshHit hit,
            CpMgr.GetData(cpI).navTgtInfo.maxDistToNavMesh,
            CpMgr.inst.unityComps[cpI].navMeshAgent.areaMask
        );
        return CpMgr.GetData(cpI).navTgtInfo.isCpOnNavmesh;
    }

    /// <summary>
    /// True if switched.
    /// </summary>
    public static bool SwitchToFallingStIfNotGrounded(int cpI) {
        var classRefs = CpMgr.inst.classRefs[cpI];
        if (
            !CpMgr.GetData(cpI).isGrounded
            && classRefs.st_cur.GetType() != typeof(CpSt_Falling)
        ) {
            //Debug.Log($"{id} was not grounded so switch to falling st!");
            CpMgr.inst.SwitchActSt(() => classRefs.actSts.falling.Enter(), cpI);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Used to transition to a baic action state after an attack/special move etc.
    /// </summary>
    public static void TransitionToFallIdleOrWalk(int cpI) {
        var classRefs = CpMgr.inst.classRefs[cpI];
        SwitchToFallingStIfNotGrounded(cpI);
        if (math.all(CpMgr.GetData(cpI).input_mov != float2.zero))
            CpMgr.inst.SwitchActSt(() => classRefs.actSts.walk.Enter(), cpI);
        else
            CpMgr.inst.SwitchActSt(() => classRefs.actSts.idle.Enter(), cpI);
    }

    /// <summary>
    /// Can be used from neutral states like "walk" or "idle" to transition to new states with input.<br/>
    /// Returns true if succeeded changing state.
    /// </summary>
    public static bool TrySwitchStByBufferedInput(int cpI) {
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
            CpMgr.inst.SwitchActSt(enterFunc, cpI);
            return true;
        }
        return false;
    }

    /// <summary>
    /// NOTE: controller inputs do not directly affect movement data - instead they're read by the action
    /// state of the pawn which then sends inputs to the movement system with this method.
    /// </summary>
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
            CpMgr.inst.SwitchActSt(curComboNode.GetNextNode(input).GetEnterFunc(cpI), cpI);
            return true;
        }
        return false;
    }
}
