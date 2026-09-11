using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Capsule pawn general util methods. Consider organizing these better!
/// </summary>
public static class CpUtils{
    /// <summary>
    /// Can be used from neutral states like "walk" or "idle" to transition to new states with input.<br/>
    /// Returns true if succeeded changing state.
    /// </summary>
    // TODO: Rename?
    public static bool BaseTrySwitchStByBufferedInput(int cpId) {
        if (TrySwitchSt(cpId, BufferableInput.BtnE))
            return true;
        if (TrySwitchSt(cpId, BufferableInput.RShldr))
            return true;
        if (TrySwitchSt(cpId, BufferableInput.RTrg))
            return true;
        if (TrySwitchSt(cpId, BufferableInput.LShldr))
            return true;
        return false;
    }

    /// <summary>
    /// Finds next state to transition to based on input and held items. Returns null if no applicable
    /// state found.<br/>
    /// NOTE: Use this when transitioning from neutral states like walk or idle. For combo chain transitions,
    /// use: <see cref="TryComboTransition"/>. (6.9.2026)
    /// </summary>
    public static Func<IFsmSt_Cp> FindStateEnterFunc(BufferableInput input, int cpId) {
        var classRefs = CpMgr.inst.classRefs[cpId];
        var unityComps = CpMgr.inst.unityComps[cpId];
        if(input == BufferableInput.BtnE)
            return () => classRefs.actSts.dodge.Enter();
        // TODO: Make projectile attack a combo attakc.
        if(
            unityComps.rHandItem is IHandItem_ProjectileSpawner projectileSpawner
                && (input == BufferableInput.RTrg || input == BufferableInput.RShldr)
        ) {
            return () => classRefs.actSts.atk_ShootHomingProj.Enter(
                projectileSpawner.ProjHitEffects,
                projectileSpawner.HomingProjData,
                projectileSpawner.ProjSpawnPose,
                CpMgr.inst.brainData[cpId].lockedOnTgt.Trf
            );
        }
        if (unityComps.rHandItem is IHandItem_Comboer comboer) {
            Func<IFsmSt_Cp> enter = input switch {
                BufferableInput.RShldr => GetEnterFunc(comboer.RShldrComboStart, cpId),
                BufferableInput.RTrg => GetEnterFunc(comboer.RTrgComboStart, cpId),
                BufferableInput.LShldr => GetEnterFunc(comboer.LShldrComboStart, cpId),
                _ => GeneralUtils.LogErrorForInput<BufferableInput, Func<IFsmSt_Cp>>(input)
            };
            if (enter != null)
                return enter;
        }
        // TODO: You should make these "combo" moves, this is just a temp solution.
        if (unityComps.rHandItem is IHandItem_Hitter hitter) {
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
        static Func<IFsmSt_Cp> GetEnterFunc(IComboNode comboStart, int cpId)
            => comboStart == null ? null : comboStart.GetEnterFunc(cpId);
    }

    /// <summary>
    /// Updates navMeshInfo if not already updated this tick and returns if the pawn is on the navmesh.
    /// </summary>
    public static bool IsOnNavMesh(int cpId) {
        if (CpMgr.GetAos(cpId).navTgtInfo.hasUpdatedNavTgtInfoThisTick)
            return CpMgr.GetAos(cpId).navTgtInfo.isCpOnNavmesh;
        Transform trf = CpMgr.inst.unityComps[cpId].rootTrf;
        CpMgr.GetAos(cpId).navTgtInfo.hasUpdatedNavTgtInfoThisTick = true;
        CpMgr.GetAos(cpId).navTgtInfo.isCpOnNavmesh = NavMesh.SamplePosition(
            trf.position,
            out NavMeshHit hit,
            CpMgr.GetAos(cpId).navTgtInfo.maxDistToNavMesh,
            CpMgr.inst.unityComps[cpId].navMeshAgent.areaMask
        );
        return CpMgr.GetAos(cpId).navTgtInfo.isCpOnNavmesh;
    }

    /// <summary>
    /// Used to transition to a baic action state after an attack/special move etc.
    /// </summary>
    // TODO: rename?
    public static void TransitionToFallIdleOrWalk(int cpId) {
        var classRefs = CpMgr.inst.classRefs[cpId];
        SwitchToFallingStIfNotGrounded(cpId);
        if (math.all(CpMgr.GetAos(cpId).input_mov != float2.zero))
            CpMgr.inst.SwitchActSt(() => classRefs.actSts.walk.Enter(), cpId);
        else
            CpMgr.inst.SwitchActSt(() => classRefs.actSts.idle.Enter(), cpId);
    }

    /// <summary>
    /// Tries to switch state based on input and equipped items (and possibly other state).
    /// Returns true if state transition successful.
    /// </summary>
    public static bool TrySwitchSt(int cpId, BufferableInput input) {
        var classRefs = CpMgr.inst.classRefs[cpId];
        Func<IFsmSt_Cp> enterFunc = FindStateEnterFunc(input, cpId);
        if (
            enterFunc != null
                && CpInputBuffer.TryConsumeInput(
                    input,
                    ref CpMgr.GetAos(cpId).inputBuffer_BufferedInput,
                    ref CpMgr.GetAos(cpId).inputBuffer_RemainingTime
                )
        ) {
            CpMgr.inst.SwitchActSt(enterFunc, cpId);
            return true;
        }
        return false;
    }

    /// <summary>
    /// True if switched.
    /// </summary>
    public static bool SwitchToFallingStIfNotGrounded(int cpId) {
        var classRefs = CpMgr.inst.classRefs[cpId];
        if (
            !CpMgr.GetAos(cpId).isGrounded
            && classRefs.st_cur.GetType() != typeof(CpSt_Falling)
        ) {
            //Debug.Log($"{id} was not grounded so switch to falling st!");
            CpMgr.inst.SwitchActSt(() => classRefs.actSts.falling.Enter(), cpId);
            return true;
        }
        return false;
    }

    /// <summary>
    /// NOTE: controller inputs do not directly affect movement data - instead they're read by the action
    /// state of the pawn which then sends inputs to the movement system with this method.
    /// </summary>
    public static void UpdateMovInputData(
        int cpId,
        float2 tgtHorDir,
        float3 additionalLinMov,
        float tgtHorSpd,
        float yawSpd,
        float horAcc
    ) {
        CpMgr.GetAos(cpId).movInput_tgtHorDir = tgtHorDir;
        CpMgr.GetAos(cpId).movInput_additionalLinMov = additionalLinMov;
        CpMgr.GetAos(cpId).movInput_tgtHorSpd = tgtHorSpd;
        CpMgr.GetAos(cpId).movInput_yawSpd = yawSpd;
        CpMgr.GetAos(cpId).movInput_horAcc = horAcc;
    }

    /// <summary>
    /// Transitions to any existing next combo node that require input if such input was buffered.
    /// Immediately returns true if successfully switched state.
    /// </summary>
    public static bool TryAnyComboInputTransition(int cpId, IComboNode curComboNode)
    => TryComboTransition(BufferableInput.RShldr, curComboNode, cpId)
        || TryComboTransition(BufferableInput.RTrg, curComboNode, cpId)
        || TryComboTransition(BufferableInput.BtnE, curComboNode, cpId)
        || TryComboTransition(BufferableInput.LShldr, curComboNode, cpId);

    /// <summary>
    /// Returns true if successfully transitioned to the next action state of the combo.
    /// </summary>
    static bool TryComboTransition(BufferableInput input, IComboNode curComboNode, int cpId) {
        var data = CpMgr.inst.aosData;
        var classRefs = CpMgr.inst.classRefs[cpId];
        if (
            // TODO: This check if faster than trying to get the next node func. However for simplicity you
            // TODO C: could just consume the input, get the func and then check if it's null. You only gain
            // TODO C: perf only when the button actually doesn't change the state which is cheap anyway.
            curComboNode.GetNextNode(input) != null
                && CpInputBuffer.TryConsumeInput(
                    input,
                    ref CpMgr.GetAos(cpId).inputBuffer_BufferedInput,
                    ref CpMgr.GetAos(cpId).inputBuffer_RemainingTime
                )
        ) {
            CpMgr.inst.SwitchActSt(curComboNode.GetNextNode(input).GetEnterFunc(cpId), cpId);
            return true;
        }
        return false;
    }
}
