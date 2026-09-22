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

    [HideInInspector] public Cp_Data[] aos;

    /// <summary>
    /// Used to set the used length of the arrays (since they do not reallocate when elements are removed).
    /// </summary>
    int entityCount;

    public void Init() {
        aos = new Cp_Data[initCapacity];
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
        Cp_Data newAosData = new();
        // NOTE: If these are not set to false, the nav mesh agent component will try to move the capsule
        // NOTE C: pawn trf. NavMeshAgent will still move its own position and rotation which can cause
        // NOTE C: problems if you don't set the drifting navmesh position back to the transform position
        // NOTE C: and rotation every time you move the capsule pawn.
        newCp.unityObjs.navMeshAgent.updatePosition = false;
        newCp.unityObjs.navMeshAgent.updateRotation = false;
        newAosData.handle = newCp;
        newAosData.act_BasicImpact_YawSpd = newCp.so_cpData.impact_YawSpd;
        newAosData.act_BasicWindup_MaxAngSpd = newCp.so_cpData.st_AtkHorSlash_Windup_YawSpd;
        newAosData.act_AtkJump_DownSpeedAfterJumpFinished
            = newCp.so_cpData.st_AtkJump_DownSpeedAfterJumpFinished;
        newAosData.act_Dodge_YawSpd = newCp.so_cpData.st_Dodge_YawAngSpd;
        newAosData.act_Dodge_HorMovSpdMult = 1.5f; // TODO: hard coded.
        newAosData.act_Falling_LandingStFallDistThreshold
            = newCp.so_cpData.st_Falling_LandingStFallDistThreshold;
        newAosData.act_Falling_HorAcc = newCp.so_cpData.st_Falling_HorAcc;
        newAosData.act_Falling_TgtHorSpd = newCp.so_cpData.st_Falling_TgtHorSpd;
        newAosData.curStDur = 0;
        newAosData.displayName = "Test Name"; // TODO: Set with so.
        newAosData.groundCastHitSomething = false;
        newAosData.groundCastNrm = float3.zero;
        newAosData.groundSnapVerDownSpd = newCp.so_cpData.groundSnapVerDownSpd;
        newAosData.hp_Cur = newCp.so_cpData.maxHP;
        newAosData.hp_Max = newCp.so_cpData.maxHP;
        newAosData.input_mov = float2.zero;
        newAosData.input_mov_LastNonZero = float2.zero;
        newAosData.input_mov_WhenLastSwitchedSt = float2.zero;
        newAosData.ignoreHits = false;
        newAosData.isAffectedByGravity = true;
        newAosData.isGrounded = true;
        newAosData.lastKnockbackStr = 0;
        newAosData.lastRecievedHitDir = float3.zero;
        newAosData.team = newCp.so_cpData.team;
        newAosData.vel_Hor = float2.zero;
        newAosData.vel_Ver = 0;
        newAosData.walkLinAcc = newCp.so_cpData.walkHorAcc;
        newAosData.walkMaxLinSpd = newCp.so_cpData.walkTgtHorSpd;
        newAosData.walkYawSpd = newCp.so_cpData.walkYawSpd;
        newAosData.enableDbgMsgs = newCp.so_cpData.enableDebugMsgs;
        newAosData.act_AtkFlying_TgtHorSpd = newCp.so_cpData.st_AtkFlying_TgtHorSpeed;
        // NOTE: We set default maxDistToNavMesh to 0.2! (10.9.2026) TODO: Put this into global variables.
        newAosData.navTgtInfo = new(false, false, 0.2f);
        newAosData.unityComps = newCp.unityObjs;
        IHandItem rHandItem = HandItemFactory.InstantiateHandItem(newCp.so_cpData.rHandItem);
        rHandItem.Trf.SetPositionAndRotation(
            newCp.unityObjs.rHand.position,
            newCp.unityObjs.rHand.rotation
        );
        rHandItem.Trf.parent = newCp.unityObjs.rHand;
        Cp_NonUnityObjClassRefs newClassRefs = new Cp_NonUnityObjClassRefs(newCp, null, rHandItem);
        newAosData.classRefs = newClassRefs;
        ArrayUtils.Add(ref aos, entityCount, newAosData);
        //Debug.Log($"Switching {freeI} to initial act st!", this);
        newCp.I = entityCount;
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
        GameObject.Destroy(aos[cpI].handle.gameObject);
        int lastI = entityCount - 1;
        CpHandle swappedCp = cpI != lastI ? aos[lastI].handle : null;
        ArrayUtils.RemoveAtSwapBack(aos, entityCount, cpI);
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
        UpdateGroundCheck();
        FixedTick_Fsm();
    }

    void FixedTick_Fsm() {
        for (int i = 0; i < entityCount; i++) {
            if (aos[i].pendingUnregister)
                continue;
            Debug.Assert(aos[i].classRefs.st_cur != null, $"cur st was null for {i}.");
            aos[i].classRefs.st_cur.PhysicsTick();
        }
    }

    void UpdateGroundCheck() {
        for (int i = 0; i < entityCount; i++) {
            if (aos[i].pendingUnregister)
                continue;
            GetData(i).isGrounded = CcMov.IsGrounded(
                aos[i].unityComps.cc,
                out bool hitSomething,
                out RaycastHit groundCastResult
            );
            GetData(i).groundCastHitSomething = hitSomething;
            GetData(i).groundCastNrm = groundCastResult.normal;
            //Debug.Log($"{i} {GetData(i).isGrounded}");
        }
    }

    // ------------------------------------------------------------
    // Tick Methods
    // ------------------------------------------------------------

    public void Tick(float dt) {
        // Navigation target info is calculated only once per frame (if any request it).
        for (int i = 0; i < entityCount; i++) {
            if (aos[i].pendingUnregister)
                continue;
            aos[i].navTgtInfo.hasUpdatedNavTgtInfoThisTick = false;
            GetData(i).curStDur += dt;
        }
        Tick_ReadMovInput();
        Tick_InputBuffer(dt);
        Tick_Fsm();
        Tick_Mov(dt);
    }

    void Tick_Fsm() {
        for (int i = 0; i < entityCount; i++) {
            if (aos[i].pendingUnregister)
                continue;
            aos[i].classRefs.st_cur.Tick();
        }
    }

    void Tick_ReadMovInput() {
        for (int i = 0; i < entityCount; i++) {
            if (aos[i].pendingUnregister)
                continue;
            if (aos[i].classRefs.cpCtrl == null)
                continue;
            //GetAos(i).input_atk_Light = aos[i].classRefs.cpCtrl.TryConsume_Atk_Light();
            //GetAos(i).input_atk_Heavy = aos[i].classRefs.cpCtrl.TryConsume_Atk_Heavy();
            //GetAos(i).input_atk_Ult = aos[i].classRefs.cpCtrl.TryConsume_Atk_Ult();
            //GetAos(i).input_dodge = aos[i].classRefs.cpCtrl.TryConsume_Dodge();
            if (aos[i].classRefs.cpCtrl.Input_Mov.sqrMagnitude > GameSettings.inst.movInputSqrDeadzone) {
                GetData(i).input_mov = aos[i].classRefs.cpCtrl.Input_Mov;
                GetData(i).input_mov_LastNonZero = aos[i].classRefs.cpCtrl.Input_Mov;
            } else
                GetData(i).input_mov = Vector2.zero;
            //Dbg.Log($"light attack input: {GetAos(i).input_atk_Light}", cp[i], aosData[i].enableDbgMsgs);
            //Debug.Log($"{i} mov input mag: {math.length(data.input_mov[i])}.");
        }
    }

    void Tick_InputBuffer(float dt) {
        for (int i = 0; i < entityCount; i++) {
            if (aos[i].pendingUnregister)
                continue;
            if (aos[i].classRefs.cpCtrl == null)
                continue;
            if (aos[i].classRefs.cpCtrl.TryConsume_Atk_Light())
                InputBufferUtils.BufferInput(
                    ref GetData(i).inputBuffer_BufferedInput,
                    ref GetData(i).inputBuffer_RemainingTime,
                    BufferableInput.RShldr,
                    GlobalData.inst.inputBuffer_Dur
                );
            else if (aos[i].classRefs.cpCtrl.TryConsume_Atk_Heavy())
                InputBufferUtils.BufferInput(
                    ref GetData(i).inputBuffer_BufferedInput,
                    ref GetData(i).inputBuffer_RemainingTime,
                    BufferableInput.RTrg,
                    GlobalData.inst.inputBuffer_Dur
                );
            else if (aos[i].classRefs.cpCtrl.TryConsume_Atk_Ult())
                InputBufferUtils.BufferInput(
                    ref GetData(i).inputBuffer_BufferedInput,
                    ref GetData(i).inputBuffer_RemainingTime,
                    BufferableInput.LShldr,
                    GlobalData.inst.inputBuffer_Dur
                );
            else if (aos[i].classRefs.cpCtrl.TryConsume_Dodge())
                InputBufferUtils.BufferInput(
                    ref GetData(i).inputBuffer_BufferedInput,
                    ref GetData(i).inputBuffer_RemainingTime, 
                    BufferableInput.BtnE,
                    GlobalData.inst.inputBuffer_Dur
                );
            // Clear input if buffer time passed.
            if (GetData(i).inputBuffer_RemainingTime <= 0)
                continue;
            GetData(i).inputBuffer_RemainingTime -= dt;
            //Debug.Log("remaining time: " + remainingTime);
            if (GetData(i).inputBuffer_RemainingTime <= 0)
                InputBufferUtils.Clear(
                    ref GetData(i).inputBuffer_BufferedInput,
                    ref GetData(i).inputBuffer_RemainingTime
                );
        }
    }

    void Tick_Mov(float dt) {
        for (int i = 0; i < entityCount; i++) {
            if (aos[i].pendingUnregister)
                continue;
            //Dbg.Log(
            //    $"tgtHorSpd: {soaData.movInput_tgtHorSpd[i]} "
            //    + $"| additionalLinMov: {soaData.movInput_additionalLinMov[i]} \n"
            //    + $"| tgtHorDir: {soaData.movInput_tgtHorDir[i]} "
            //    + $"| horAcc: {soaData.movInput_horAcc[i]} "
            //    + $"| yawSpd {soaData.movInput_yawSpd[i]}",
            //    aosData[i].enableDebugMsgs
            //);
            Debug.Assert(
                !float.IsNaN(GetData(i).vel_Hor.x) && !float.IsNaN(GetData(i).vel_Hor.y),
                $"{i} vel_hor had NaN: {GetData(i).vel_Hor}"
            );
            //Debug.Log($"UpdateMov: data.vel_Hor before calculations: {data.vel_Hor}");
            GetData(i).vel_Hor = Vector2.MoveTowards(
                GetData(i).vel_Hor,
                GetData(i).movInput_tgtHorDir * GetData(i).movInput_tgtHorSpd,
                GetData(i).movInput_horAcc * dt
            );
            // Skip rotation if character is already rotated towards linear movement target direction.
            if (math.lengthsq(GetData(i).movInput_tgtHorDir) > 0.0001f) {
                aos[i].handle.transform.rotation = TrfMathUtils.RotateFwdToTgt(
                    inst.aos[i].handle.transform.rotation,
                    GetData(i).movInput_yawSpd,
                    GetData(i).movInput_tgtHorDir
                );
            }
            if (GetData(i).isAffectedByGravity)
                // NOTE: This will override previously calculated horizontal velocity if the player is
                // NOTE C: sliding down a slope. (9.9.2026)
                CcMov.ApplyGravityNSlideDownSlopes(i, dt);
            else
                // NOTE: If not using gravitational acceleration, ver velocity is reseted every tick. This
                // NOTE C: way we don't accidentally accumulate velocity when using animation root motion
                // NOTE C: for vertical movement.
                GetData(i).vel_Ver = 0;
            // NOTE: Additional linear movement is used to apply animation root delta lin movement (9.9.2026)
            Vector3 totalMov = (Vector3)GetData(i).movInput_additionalLinMov
                + new Vector3(GetData(i).vel_Hor.x, GetData(i).vel_Ver, GetData(i).vel_Hor.y) * dt;
            //Debug.Log($"UpdateMov: totalMov: {totalMov}");
            aos[i].unityComps.cc.Move(totalMov);
            // Save final velocity back to cp data.
            GetData(i).vel_Hor = new float2(totalMov.x, totalMov.z) / dt;
            GetData(i).vel_Ver = totalMov.y / dt;
            // NavMeshAgent will drift away from the capsule pawn transform if you don't set it back here.
            aos[i].unityComps.navMeshAgent.nextPosition = aos[i].handle.transform.position;
        }
    }

    // ------------------------------------------------------------
    // Late Tick Methods
    // ------------------------------------------------------------

    public void LateTick() {
        LateTick_AnimEventPlr();
        LateTick_Fsm();
        LateTick_UnregisterNDestroyPending();
    }

    void LateTick_AnimEventPlr() {
        for (int i = 0; i < entityCount; i++) {
            // NOTE: If pendingUnregister, animEventPlr is never ticked for a cp even if the Animator
            // NOTE C: itself hadn't been destroyed yet (so it's possible to have an Animator update without
            // NOTE C: the AnimEventPlr ticking for the corresponding Cp.
            if (aos[i].pendingUnregister)
                continue;
            //Debug.Log($"{aos[i].animEventPlrData}");
            //Debug.Log($"{aos[i].unityComps.anim == null}");
            //Debug.Log($"{aos[i].unityComps.animEvents == null}");
            AnimEventPlr.Tick(
                i,
                ref aos[i].animEventPlrData,
                aos[i].unityComps.anim,
                aos[i].unityComps.animEventHandler.animEvent
            );
        }
    }

    void LateTick_Fsm() {
        for (int i = 0; i < entityCount; i++) {
            if (aos[i].pendingUnregister)
                continue;
            aos[i].classRefs.st_cur.LateTick();
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
            if (aos[i].pendingUnregister)
                UnregisterNDestroy(i);
            else
                i++;
        }
    }

    // ------------------------------------------------------------
    // Other Methods
    // ------------------------------------------------------------

    public static ref Cp_Data GetData(int cpI)
        => ref inst.aos[cpI];

    /// <summary>
    /// This should always be called when cp act state is switched!
    /// </summary>
    public void OnStateSwitched(int cpI, IFsmSt newSt) {
        GetData(cpI).curStDur = 0;
        if (aos[cpI].classRefs.cpCtrl == null)
            GetData(cpI).input_mov_WhenLastSwitchedSt
                = GetData(cpI).input_mov;
        else {
            if (aos[cpI].classRefs.cpCtrl.Input_Mov.sqrMagnitude > GameSettings.inst.movInputSqrDeadzone)
                GetData(cpI).input_mov_WhenLastSwitchedSt
                    = GetData(cpI).input_mov;
            else
                GetData(cpI).input_mov_WhenLastSwitchedSt = float2.zero;
        }
    }

    /// <summary>
    /// Marks the entity for being unregistered and destroyed and invokes <see cref="Cp_Data"/>.<br/>
    /// NOTE: DO NOT DESTROY A <see cref="CpHandle"/> DIRECTLY OR ASSIGN
    /// <see cref="Cp_Data.pendingUnregister"/>, INSTEAD CALL THIS! 
    /// = true!!!
    /// </summary>
    public void MarkForPendingUnregister(int cpI) {
        aos[cpI].pendingUnregister = true;
        aos[cpI].action_markedForPendingUnregister?.Invoke();
    }

    /// <summary>
    /// Call this if you want to make a cp listen to a controller, i.e. get possessed by a controller.
    /// </summary>
    public static void StartListeningToCtrlInput(int cpI, ICpCtrlInputter ctrl) {
        Debug.Assert(inst.aos[cpI].classRefs.cpCtrl == null, $"{cpI} already listening to a ctrl!");
        inst.aos[cpI].classRefs.cpCtrl = ctrl;
    }

    /// <summary>
    /// NOTE: Never directly call Fsm.Switch state since that will bypass calling
    /// <see cref="OnStateSwitched"/>. (10.9.2026)
    /// </summary>
    public void SwitchActSt(Func<IFsmSt_Cp> enterFunc, int cpI){
        Fsm.SwitchSt(
            enterFunc,
            ref aos[cpI].classRefs.st_cur,
            ref aos[cpI].classRefs.st_prev,
            ref GetData(cpI).isSwitchingSt
            //GetData(cpI).enableDbgMsgs
        );
        OnStateSwitched(cpI, aos[cpI].classRefs.st_cur);
    }

    public void SwitchToInitActSt(int cpI) {
        //Debug.Log($"{cpI} switching to init state", this);
        // NOTE: This is currently always enters to idle state. (6.9.2026)
        SwitchActSt(() => aos[cpI].classRefs.actSts.idle.Enter(), cpI);
        //Debug.Log($"{cpI} state initialized to : {nameof(Cp_ActSts.idle)}", this);
    }

    /// <summary>
    /// Tries to find any <see cref="CpHandle"/> considered an "enemy" to <paramref name="cpI"/>. Returns
    /// null if none found.
    /// </summary>
    public static CpHandle TryFindEnemy(int cpI) {
        for (int i = 0; i < inst.entityCount; i++) {
            if (i == cpI)
                continue;
            PawnTeam candTeam = CpMgr.inst.aos[i].team;
            if (candTeam == PawnTeam.FriendToAll)
                continue;
            if (candTeam == PawnTeam.EnemyToAll || candTeam != CpMgr.inst.aos[cpI].team) {
                //Debug.Log("Found tgt: " + i);
                return inst.aos[i].handle;
            }
        }
        return null;
    }

    /// <summary>
    /// NOTE: Never directly call <see cref="Fsm.TrySwitchState"/> since that will bypass calling
    /// <see cref="OnStateSwitched"/>, so call this instead!. (10.9.2026)
    /// NOTE 2: <paramref name="enterFunc"/> return type needs to be generic (instead of IFsmSt_Cp), otherwise
    /// information of the new state type is lost. (12.9.2026)
    /// </summary>
    public bool TrySwitchActSt<TNewState>(Func<TNewState> enterFunc, int cpI) where TNewState : IFsmSt_Cp {
        if(
            Fsm.TrySwitchState(
                enterFunc,
                ref aos[cpI].classRefs.st_cur,
                ref aos[cpI].classRefs.st_prev,
                ref GetData(cpI).isSwitchingSt
                //GetData(cpI).enableDbgMsgs
            )
        ) {
            OnStateSwitched(cpI, aos[cpI].classRefs.st_cur);
            return true;
        }
        return false;
    }
}