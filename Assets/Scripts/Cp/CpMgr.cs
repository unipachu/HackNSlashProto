using System;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Capsule pawn (i.e. player or ai controlled character that uses capsule collision for movement) manager.
/// </summary>
public class CpMgr : Singleton<CpMgr> {
    [Tooltip("Initial capacity of arrays. They allocate more space if needed (but do not deallocate even" +
        "if pawns are unregistered.)")]
    [SerializeField] int initCapacity = 1;

    [HideInInspector] public Cp_CommonData[] commonData;
    [HideInInspector] public CpHumd_Data[] humdData;

    /// <summary>
    /// Used to set the used length of the arrays (since they do not reallocate when elements are removed).
    /// </summary>
    int entityCount;

    public void Init() {
        humdData = new CpHumd_Data[initCapacity];
        commonData = new Cp_CommonData[initCapacity];
        //Debug.Log($"{nameof(aos)} length in init: {aos.Length}");
    }

    // ------------------------------------------------------------
    // Register and Unregister
    // ------------------------------------------------------------

    /// <summary>
    /// Registers new capsule pawn.
    /// NOTE: Initialize the game object beforehand and pass it in as a <paramref name="newCp"/>.
    /// </summary>
    public void Register(CpHandle newCp) {
        CpHumd_Data newAosData = new();
        Cp_CommonData newCommonData = new();
        // NOTE: If these are not set to false, the nav mesh agent component will try to move the capsule
        // NOTE C: pawn trf. NavMeshAgent will still move its own position and rotation which can cause
        // NOTE C: problems if you don't set the drifting navmesh position back to the transform position
        // NOTE C: and rotation every time you move the capsule pawn.
        newCp.navMeshAgent.updatePosition = false;
        newCp.navMeshAgent.updateRotation = false;
        newCommonData.classRefs = new Cp_NonUnityObjClassRefs(null); ;
        newCommonData.curStDur = 0;
        newCommonData.groundCastHitSomething = false;
        newCommonData.groundCastNrm = float3.zero;
        newCommonData.handle = newCp;
        newCommonData.hp_Cur = newCp.so_cpCommonData.hp_Max;
        newCommonData.input_mov = float2.zero;
        newCommonData.input_mov_LastNonZero = float2.zero;
        newCommonData.input_mov_WhenLastSwitchedSt = float2.zero;
        newCommonData.ignoreHits = false;
        newCommonData.isAffectedByGravity = true;
        newCommonData.isGrounded = true;
        newCommonData.lastKnockbackStr = 0;
        newCommonData.lastRecievedHitDir = float3.zero;
        newCommonData.vel_Hor = float2.zero;
        newCommonData.vel_Ver = 0;
        // NOTE: We set default maxDistToNavMesh to 0.2! (10.9.2026) TODO: Put this into global variables.
        newCommonData.navTgtInfo = new(false, false, 0.2f);
        ArrayUtils.Add(ref commonData, entityCount, newCommonData);
        newAosData.handle = newCp;
        IHandItem rHandItem = HandItemFactory.InstantiateHandItem(newCp.so_cpHumdConfig.rHandItem);
        rHandItem.Trf.SetPositionAndRotation(
            newCp.rHand.position,
            newCp.rHand.rotation
        );
        rHandItem.Trf.parent = newCp.rHand;
        rHandItem.hitSomething += newCp.OnHitSomething;
        newAosData.classRefs = new CpHumanoid_NonUnityObjClassRefs(newCp, rHandItem);
        ArrayUtils.Add(ref humdData, entityCount, newAosData);
        //Debug.Log($"Switching {freeI} to initial act st!", this);
        newCp.I = entityCount;
        CpRegister.inst.cps.Add(newCp);
        SwitchToInitActSt(entityCount);
        entityCount++;
    }

    /// <summary>
    /// Unregisters cp entity and destroys the handle.
    /// WARNING: NEVER CALL THIS DIRECTLY FROM ANYWHERE EXCEPT
    /// <see cref="LateTick_UnregisterNDestroyPending"/>!
    /// </summary>
    void UnregisterNDestroy(int cpI) {
        if (cpI >= entityCount) {
            Debug.LogError($"{cpI} was greaterequal to {entityCount}!");
            return;
        }
        CpRegister.inst.cps.Remove(humdData[cpI].handle);
        humdData[cpI].classRefs.rHandItem.hitSomething -= humdData[cpI].handle.OnHitSomething;
        GameObject.Destroy(humdData[cpI].handle.gameObject);
        int lastI = entityCount - 1;
        CpHandle swappedCp = cpI != lastI ? humdData[lastI].handle : null;
        ArrayUtils.RemoveAtSwapBack(humdData, entityCount, cpI);
        ArrayUtils.RemoveAtSwapBack(commonData, entityCount, cpI);
        entityCount--;
        if (swappedCp != null)
            // Last Cp was swapped to cpI, so update Id.
            swappedCp.I = cpI;
        //Debug.Log($"Unregistered (and destroyed) {typeof(CpHandle)} id: {cpI}.\n" +
        //    $"New entity count: {entityCount}");
    }

    // ------------------------------------------------------------
    // Fixed Tick Methods
    // ------------------------------------------------------------

    public void FixedTick() {
        UpdateGroundCheck(commonData, entityCount);
        FixedTick_Fsm(commonData, entityCount);
    }

    public static void FixedTick_Fsm(Cp_CommonData[] commonData, int usedLength) {
        for (int i = 0; i < usedLength; i++) {
            if (commonData[i].pendingUnregister)
                continue;
            Debug.Assert(commonData[i].classRefs.st_cur != null, $"cur st was null for {i}.");
            commonData[i].classRefs.st_cur.PhysicsTick();
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

    // ------------------------------------------------------------
    // Tick Methods
    // ------------------------------------------------------------

    public void Tick(float dt) {
        Tick_TickSetup(commonData, dt, entityCount);
        Tick_Cooldowns(dt);
        Tick_ReadMovInput(commonData, entityCount);
        Tick_InputBuffer(commonData, dt, entityCount);
        Tick_Fsm(commonData, entityCount);
    }

    public static void Tick_TickSetup(Cp_CommonData[] commonData, float dt, int usedLength) {
        // Navigation target info is calculated only once per frame (if any request it).
        for (int i = 0; i < usedLength; i++) {
            if (commonData[i].pendingUnregister)
                continue;
            commonData[i].navTgtInfo.hasUpdatedNavTgtInfoThisTick = false;
            commonData[i].curStDur += dt;
        }
    }

    void Tick_Cooldowns(float dt) {
        for (int i = 0; i < entityCount; i++) {
            if (commonData[i].pendingUnregister)
                continue;
            ref var cpData = ref GetHumanoidData(i);
            if(cpData.cooldownTimer_Dodge > 0 && !cpData.cooldownFreezed_Dodge)
                cpData.cooldownTimer_Dodge = Mathf.Max(0, cpData.cooldownTimer_Dodge - dt);
            //Dbg.Log(
            //    $"cp {i} {nameof(cpData.cooldownTimer_Dodge)}: {cpData.cooldownTimer_Dodge}, " 
            //        + $"{nameof(cpData.cooldownFreezed_Dodge)}: {cpData.cooldownFreezed_Dodge}",
            //    cpData.handle,
            //    cpData.handle.so_cpData.enableDbgMsgs
            //);
        }
    }

    public static void Tick_Fsm(Cp_CommonData[] commonData, int usedLength) {
        for (int i = 0; i < usedLength; i++) {
            if (commonData[i].pendingUnregister)
                continue;
            commonData[i].classRefs.st_cur.Tick();
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
                    BufferableInput.RShldr,
                    GlobalData.inst.inputBuffer_Dur
                );
            else if (commonData[i].classRefs.cpCtrl.TryConsume_Atk_Heavy())
                InputBufferUtils.BufferInput(
                    ref commonData[i].inputBuffer_BufferedInput,
                    ref commonData[i].inputBuffer_RemainingTime,
                    BufferableInput.RTrg,
                    GlobalData.inst.inputBuffer_Dur
                );
            else if (commonData[i].classRefs.cpCtrl.TryConsume_Atk_Ult())
                InputBufferUtils.BufferInput(
                    ref commonData[i].inputBuffer_BufferedInput,
                    ref commonData[i].inputBuffer_RemainingTime,
                    BufferableInput.LShldr,
                    GlobalData.inst.inputBuffer_Dur
                );
            else if (commonData[i].classRefs.cpCtrl.TryConsume_Dodge())
                InputBufferUtils.BufferInput(
                    ref commonData[i].inputBuffer_BufferedInput,
                    ref commonData[i].inputBuffer_RemainingTime,
                    BufferableInput.BtnE,
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

    // ------------------------------------------------------------
    // Late Tick Methods
    // ------------------------------------------------------------

    public void LateTick(float dt) {
        // NOTE: We move character controller right after animation update so that animation rootmotion is
        // C: applied instantly.
        LateTick_Mov(commonData, dt, entityCount);
        LateTick_AnimEventPlr();
        LateTick_Fsm(commonData, entityCount);
        LateTick_UnregisterNDestroyPending();
    }

    public void LateTick_AnimEventPlr() {
        for (int i = 0; i < entityCount; i++) {
            // NOTE: If pendingUnregister, animEventPlr is never ticked for a cp even if the Animator
            // NOTE C: itself hadn't been destroyed yet (so it's possible to have an Animator update without
            // NOTE C: the AnimEventPlr ticking for the corresponding Cp.
            if (commonData[i].pendingUnregister)
                continue;
            //Debug.Log($"{aos[i].animEventPlrData}");
            //Debug.Log($"{aos[i].unityComps.anim == null}");
            //Debug.Log($"{aos[i].unityComps.animEvents == null}");
            AnimEventPlr.Tick(
                humdData[i].handle,
                ref commonData[i].animEventPlrData,
                commonData[i].handle.Anim,
                humdData[i].handle.AnimEventHandler.animEvent
            );
        }
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
    /// NOTE: We want to unregister an entity at a safe point when we are not looping over the entities or
    /// otherwise using their Id's. You can safely mark a cp for deletion by destroying its
    /// <see cref="CpHandle"/>, it will then be unregistered here.
    /// </summary>
    void LateTick_UnregisterNDestroyPending() {
        int i = 0;
        // We swap the last element in the place of the unregistered one, so we onlu increment index if we
        // don't unregister a cp.
        while (i < entityCount) {
            if (commonData[i].pendingUnregister)
                UnregisterNDestroy(i);
            else
                i++;
        }
    }

    // ------------------------------------------------------------
    // Other Methods
    // ------------------------------------------------------------

    public static ref CpHumd_Data GetHumanoidData(int cpI)
        => ref inst.humdData[cpI];

    public static ref Cp_CommonData GetCommonData(int cpI)
        => ref inst.commonData[cpI];

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
                commonData.input_mov_WhenLastSwitchedSt = float2.zero;
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
    /// Call this if you want to make a cp listen to a controller, i.e. get possessed by a controller.
    /// </summary>
    public static void StartListeningToCtrlInput(ref Cp_CommonData commonData, ICpCtrl ctrl) {
        Debug.Assert(
            commonData.classRefs.cpCtrl == null,
            $"{commonData.handle.Go} already listening to a ctrl!"
        );
        commonData.classRefs.cpCtrl = ctrl;
    }

    public void SwitchToInitActSt(int cpI) {
        //Debug.Log($"{cpI} switching to init state", this);
        // NOTE: This is currently always enters to idle state. (6.9.2026)
        TrySwitchActSt(() => humdData[cpI].classRefs.actSts.idle.Enter(), ref commonData[cpI], true);
        //Debug.Log($"{cpI} state initialized to : {nameof(Cp_ActSts.idle)}", this);
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
        if(
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
}