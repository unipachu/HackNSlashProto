using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Capsule pawn general util methods. Consider organizing these better!
/// </summary>
public static class CpUtils{
    /// <summary>
    /// Finds the local horizontal knockback direction for cp.
    /// </summary>
    public static Dir2DHor FindKnockBackDir(Cp_CommonData commonData) {
        Vector3 horHitDir = new Vector3(
            commonData.lastRecievedHitDir.x,
            0,
            commonData.lastRecievedHitDir.z
        );
        // If the hit dir is for some reason Vector3.zero.
        if (horHitDir.IsZeroOrNearlyZero())
            return Dir2DHor.Backward;
        horHitDir.Normalize();
        Transform trf = commonData.handle.Go.transform;
        float forwardDot = Vector3.Dot(horHitDir, trf.forward);
        float rightDot = Vector3.Dot(horHitDir, trf.right);
        if (Mathf.Abs(forwardDot) >= Mathf.Abs(rightDot))
            return forwardDot >= 0 ? Dir2DHor.Forward : Dir2DHor.Backward;
        return rightDot >= 0 ? Dir2DHor.Right : Dir2DHor.Left;
    }

    /// <summary>
    /// Call this in FixedUpdate!
    /// </summary>
    public static void FixedTick_Fsm(Cp_CommonData[] commonData, int usedLength) {
        for (int i = 0; i < usedLength; i++) {
            if (commonData[i].pendingUnregister)
                continue;
            Debug.Assert(commonData[i].classRefs.st_cur != null, $"cur st was null for {i}.");
            commonData[i].classRefs.st_cur.PhysicsTick();
        }
    }

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

    public static void LateTick_Fsm(Cp_CommonData[] commonData, int usedLength) {
        for (int i = 0; i < usedLength; i++) {
            if (commonData[i].pendingUnregister)
                continue;
            commonData[i].classRefs.st_cur.LateTick();
        }
    }

    public static void LateTick_Mov(Cp_CommonData[] commonData, float dt, int usedLength) {
        for (int i = 0; i < usedLength; i++) {
            if (commonData[i].pendingUnregister)
                continue;
            //Dbg.Log(
            //    $"tgtHorSpd: {cpData.movInput_tgtHorSpd} "
            //    + $"| additionalLinMov: {cpData.movInput_additionalLinMov} \n"
            //    + $"| tgtHorDir: {cpData.movInput_tgtHorDir} "
            //    + $"| horAcc: {cpData.movInput_horAcc} "
            //    + $"| yawSpd {cpData.movInput_yawSpd}",
            //    cpData.enableDbgMsgs
            //);
            Debug.Assert(
                !float.IsNaN(commonData[i].vel_Hor.x) && !float.IsNaN(commonData[i].vel_Hor.y),
                $"{i} vel_hor had NaN: {commonData[i].vel_Hor}"
            );
            //Debug.Log($"UpdateMov: data.vel_Hor before calculations: {data.vel_Hor}");
            commonData[i].vel_Hor = Vector2.MoveTowards(
                commonData[i].vel_Hor,
                commonData[i].movInput_tgtHorDir * commonData[i].movInput_tgtHorSpd,
                commonData[i].movInput_horAcc * dt
            );
            // Skip rotation if character is already rotated towards linear movement target direction.
            if (math.lengthsq(commonData[i].movInput_tgtHorDir) > 0.0001f) {
                commonData[i].handle.Go.transform.rotation = TrfMathUtils.RotateFwdTowardsTgt(
                    commonData[i].handle.Go.transform.rotation,
                    dt,
                    commonData[i].movInput_yawSpd,
                    commonData[i].movInput_tgtHorDir
                );
            }
            if (commonData[i].isAffectedByGravity)
                // NOTE: This will override previously calculated horizontal velocity if the player is
                // NOTE C: sliding down a slope. (9.9.2026)
                CcMov.ApplyGravityNSlideDownSlopes(ref commonData[i], dt);
            else
                // NOTE: If not using gravitational acceleration, ver velocity is reseted every tick. This
                // NOTE C: way we don't accidentally accumulate velocity when using animation root motion
                // NOTE C: for vertical movement.
                commonData[i].vel_Ver = 0;
            // NOTE: Additional linear movement is used to apply animation root delta lin movement (9.9.2026)
            Vector3 totalMov = (Vector3)commonData[i].movInput_additionalLinMov
                + new Vector3(commonData[i].vel_Hor.x, commonData[i].vel_Ver, commonData[i].vel_Hor.y) * dt;
            //Debug.Log($"UpdateMov: totalMov: {totalMov}");
            commonData[i].handle.Cc.Move(totalMov);
            // Save final velocity back to cp data.
            commonData[i].vel_Hor = new float2(totalMov.x, totalMov.z) / dt;
            commonData[i].vel_Ver = totalMov.y / dt;
            // NavMeshAgent will drift away from the capsule pawn transform if you don't set it back here.
            commonData[i].handle.NavMeshAgent.nextPosition = commonData[i].handle.Go.transform.position;
        }
    }

    /// <summary>
    /// Marks the entity for being unregistered and destroyed and invokes
    /// <see cref="Cp_CommonData.action_MarkedForPendingUnregister"/>.<br/>
    /// NOTE: DO NOT DESTROY A <see cref="ICp"/> DIRECTLY OR ASSIGN
    /// <see cref="Cp_CommonData.pendingUnregister"/>, INSTEAD CALL THIS! 
    /// = true!!!
    /// </summary>
    public static void MarkForPendingUnregister(ref Cp_CommonData commonData) {
        commonData.pendingUnregister = true;
        commonData.action_MarkedForPendingUnregister?.Invoke();
    }

    /// <summary>
    /// This should always be called when cp act state is switched!
    /// </summary>
    public static void OnStateSwitched(ref Cp_CommonData commonData) {
        commonData.curStDur = 0;
        if (commonData.classRefs.cpCtrl == null)
            commonData.input_mov_WhenLastSwitchedSt
                = commonData.input_mov;
        else {
            if (commonData.classRefs.cpCtrl.Input_LStick.sqrMagnitude > GameSettings.inst.movInputSqrDeadzone)
                commonData.input_mov_WhenLastSwitchedSt
                    = commonData.input_mov;
            else
                commonData.input_mov_WhenLastSwitchedSt = Vector2.zero;
        }
    }

    /// <summary>
    /// Call this if you want to make a cp listen to a controller, i.e. get possessed by a controller.
    /// </summary>
    public static void StartListeningToCtrlInput(ref Cp_CommonData commonData, ICpCtrl ctrl) {
        Debug.Assert(
            commonData.classRefs.cpCtrl == null,
            $"{commonData.handle.Go} already listening to a ctrl!"
        );
        commonData.classRefs.cpCtrl = ctrl;
    }

    /// <summary>
    /// Call this in Update!
    /// </summary>
    public static void Tick_Fsm(Cp_CommonData[] commonData, int usedLength) {
        for (int i = 0; i < usedLength; i++) {
            if (commonData[i].pendingUnregister)
                continue;
            commonData[i].classRefs.st_cur.Tick();
        }
    }

    public static void Tick_InputBuffer(Cp_CommonData[] commonData, float dt, int usedLength) {
        for (int i = 0; i < usedLength; i++) {
            if (commonData[i].pendingUnregister)
                continue;
            if (commonData[i].classRefs.cpCtrl == null)
                continue;
            if (commonData[i].classRefs.cpCtrl.TryConsume_Atk_Light())
                InputBufferUtils.BufferInput(
                    ref commonData[i].inputBuffer_BufferedInput,
                    ref commonData[i].inputBuffer_RemainingTime,
                    BufferableInput.Rb,
                    GlobalData.inst.inputBuffer_Dur
                );
            else if (commonData[i].classRefs.cpCtrl.TryConsume_Atk_Heavy())
                InputBufferUtils.BufferInput(
                    ref commonData[i].inputBuffer_BufferedInput,
                    ref commonData[i].inputBuffer_RemainingTime,
                    BufferableInput.Rt,
                    GlobalData.inst.inputBuffer_Dur
                );
            else if (commonData[i].classRefs.cpCtrl.TryConsume_Atk_Ult())
                InputBufferUtils.BufferInput(
                    ref commonData[i].inputBuffer_BufferedInput,
                    ref commonData[i].inputBuffer_RemainingTime,
                    BufferableInput.Lb,
                    GlobalData.inst.inputBuffer_Dur
                );
            else if (commonData[i].classRefs.cpCtrl.TryConsume_Dodge())
                InputBufferUtils.BufferInput(
                    ref commonData[i].inputBuffer_BufferedInput,
                    ref commonData[i].inputBuffer_RemainingTime,
                    BufferableInput.B,
                    GlobalData.inst.inputBuffer_Dur
                );
            if (commonData[i].inputBuffer_RemainingTime <= 0)
                continue;
            commonData[i].inputBuffer_RemainingTime -= dt;
            //Debug.Log("remaining time: " + remainingTime);
            // Clear input if buffer time passed.
            if (commonData[i].inputBuffer_RemainingTime <= 0)
                InputBufferUtils.Clear(
                    ref commonData[i].inputBuffer_BufferedInput,
                    ref commonData[i].inputBuffer_RemainingTime
                );
            continue;
        }
    }

    /// <summary>
    /// Writes movement input from controller.
    /// </summary>
    public static void Tick_ReadMovInput(Cp_CommonData[] commonData, int usedLength) {
        for (int i = 0; i < usedLength; i++) {
            if (commonData[i].pendingUnregister)
                continue;
            if (commonData[i].classRefs.cpCtrl == null) {
                commonData[i].input_mov = Vector2.zero;
                continue;
            }
            //GetAos(i).input_atk_Light = aos[i].classRefs.cpCtrl.TryConsume_Atk_Light();
            //GetAos(i).input_atk_Heavy = aos[i].classRefs.cpCtrl.TryConsume_Atk_Heavy();
            //GetAos(i).input_atk_Ult = aos[i].classRefs.cpCtrl.TryConsume_Atk_Ult();
            //GetAos(i).input_dodge = aos[i].classRefs.cpCtrl.TryConsume_Dodge();
            if (commonData[i].classRefs.cpCtrl.Input_LStick.sqrMagnitude > GameSettings.inst.movInputSqrDeadzone) {
                commonData[i].input_mov = commonData[i].classRefs.cpCtrl.Input_LStick;
                commonData[i].input_mov_LastNonZero = commonData[i].classRefs.cpCtrl.Input_LStick;
            }
            else
                commonData[i].input_mov = Vector2.zero;
            //Dbg.Log($"light attack input: {GetAos(i).input_atk_Light}", cp[i], aosData[i].enableDbgMsgs);
            //Debug.Log($"{i} mov input mag: {math.length(data.input_mov[i])}.");
        }
    }

    /// <summary>
    /// Call this in the very beginning of your cp Tick (aka Update).
    /// </summary>
    public static void Tick_TickSetup(Cp_CommonData[] commonData, float dt, int usedLength) {
        // Navigation target info is calculated only once per frame (if any request it).
        for (int i = 0; i < usedLength; i++) {
            if (commonData[i].pendingUnregister)
                continue;
            commonData[i].navTgtInfo.hasUpdatedNavTgtInfoThisTick = false;
            commonData[i].curStDur += dt;
        }
    }

    public static void UpdateGroundCheck(Cp_CommonData[] commonData, int usedLength) {
        for (int i = 0; i < usedLength; i++) {
            if (commonData[i].pendingUnregister)
                continue;
            commonData[i].isGrounded = CcMov.IsGrounded(
                commonData[i].handle.Cc,
                out bool hitSomething,
                out RaycastHit groundCastResult
            );
            commonData[i].groundCastHitSomething = hitSomething;
            commonData[i].groundCastNrm = groundCastResult.normal;
            //Debug.Log($"{i} {GetData(i).isGrounded}");
        }
    }

    /// <summary>
    /// NOTE: Never directly call <see cref="Fsm.TrySwitchState"/> since that will bypass calling
    /// <see cref="OnStateSwitched"/>, so call this instead!. (10.9.2026)
    /// NOTE 2: <paramref name="enterFunc"/> return type needs to be generic (instead of IFsmSt_Cp), otherwise
    /// information of the new state type is lost. (12.9.2026)
    /// </summary>
    /// <param name="forceStSwitch">Skip state transition checks and force state swtich?</param>
    public static bool TrySwitchActSt<TNewState>(
        Func<TNewState> enterFunc,
        ref Cp_CommonData commonData,
        bool forceStSwitch = false
    ) where TNewState : class, IFsmSt_Cp {
        if (forceStSwitch) {
            Fsm.SwitchSt(
                enterFunc,
                ref commonData.classRefs.st_cur,
                ref commonData.classRefs.st_prev,
                ref commonData.isSwitchingSt
            //GetData(cpI).handle.so_cpData.enableDbgMsgs
            );
            OnStateSwitched(ref commonData);
            return true;
        }
        if (
            Fsm.TrySwitchState(
                enterFunc,
                ref commonData.classRefs.st_cur,
                ref commonData.classRefs.st_prev,
                ref commonData.isSwitchingSt
            //GetData(cpI).handle.so_cpData.enableDbgMsgs
            )
        ) {
            OnStateSwitched(ref commonData);
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
}
