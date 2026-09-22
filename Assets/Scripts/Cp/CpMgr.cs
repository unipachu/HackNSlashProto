using System;
using Unity.AppUI.UI;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Capsule pawn (i.e. player or ai controlled character that uses capsule collision for movement) manager.
/// </summary>
public class CpMgr : Singleton<CpMgr> {
    [Tooltip("Initial capacity of arrays. They allocate more space if needed (but do not deallocate even" +
        "if pawns are unregistered.)")]
    [SerializeField] int initCapacity = 1;

    [HideInInspector] public AnimEventPlrData[] animEventPlrData;
    [HideInInspector] public Cp_NonUnityObjClassRefs[] classRefs;
    [HideInInspector] public CpHandle[] handle;
    [HideInInspector] public Cp_AosData[] aosData;
    [HideInInspector] public Cp_UnityObjs[] unityComps;

    /// <summary>
    /// Used to set the used length of the arrays (since they do not reallocate when elements are removed).
    /// </summary>
    int entityCount;

    public void Init() {
        animEventPlrData = new AnimEventPlrData[initCapacity];
        classRefs = new Cp_NonUnityObjClassRefs[initCapacity];
        handle = new CpHandle[initCapacity];
        aosData = new Cp_AosData[initCapacity];
        //Debug.Log($"soa length in init: {aosData.Length}");
        unityComps = new Cp_UnityObjs[initCapacity];
    }

    // ------------------------------------------------------------
    // Register and Unregister
    // ------------------------------------------------------------

    /// <summary>
    /// Registers new capsule pawn.
    /// NOTE: Initialize the game object beforehand and pass it in as a <paramref name="newCp"/>.
    /// </summary>
    public void Register(CpHandle newCp) {
        // If these are not set to false, the nav mesh agent component will try to move the capsule pawn trf.
        // NOTE: NavMeshAgent will still move its own position and rotation which can cause problems if you don't
        // NOTE C: set the drifting navmesh position back to the transform position and rotation every time you
        // NOTE C: move the capsule pawn.
        newCp.unityObjs.navMeshAgent.updatePosition = false;
        newCp.unityObjs.navMeshAgent.updateRotation = false;
        // NOTE: Index = new count - 1.
        //Debug.Log($"Start registering {cp}", cp);
        // This is set when switching to init act state.
        ArrayUtils.Add(ref animEventPlrData, entityCount, default);
        ArrayUtils.Add(ref handle, entityCount, newCp);
        // Structure of arrays data
        Cp_AosData newAosData = new();
        newAosData.act_BasicImpact_YawSpd = newCp.so_cpData.impact_YawSpd;
        newAosData.act_BasicWindup_MaxAngSpd = newCp.so_cpData.st_AtkHorSlash_Windup_YawSpd;
        newAosData.act_AtkJump_DownSpeedAfterJumpFinished
            = newCp.so_cpData.st_AtkJump_DownSpeedAfterJumpFinished;
        newAosData.act_Dodge_YawSpd = newCp.so_cpData.st_Dodge_YawAngSpd;
        newAosData.act_Dodge_HorMovSpdMult = 1.5f; // NOTE: hard coded.
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
        // NOTE: We set default maxDistToNavMesh to 0.2! (10.9.2026)
        newAosData.navTgtInfo = new(false, false, 0.2f);
        ArrayUtils.Add(ref aosData, entityCount, newAosData);
        ArrayUtils.Add(ref unityComps, entityCount, newCp.unityObjs);
        IHandItem rHandItem = HandItemFactory.InstantiateHandItem(newCp.so_cpData.rHandItem);
        rHandItem.Trf.SetPositionAndRotation(
            newCp.unityObjs.rHand.position,
            newCp.unityObjs.rHand.rotation
        );
        rHandItem.Trf.parent = newCp.unityObjs.rHand;
        Cp_NonUnityObjClassRefs newClassRefs = new Cp_NonUnityObjClassRefs(newCp, null, rHandItem);
        ArrayUtils.Add(ref classRefs, entityCount, newClassRefs);
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
        GameObject.Destroy(handle[cpI].gameObject);
        int lastId = entityCount - 1;
        CpHandle swappedCp = cpI != lastId ? handle[lastId] : null;
        ArrayUtils.RemoveAtSwapBack(animEventPlrData, entityCount, cpI);
        ArrayUtils.RemoveAtSwapBack(classRefs, entityCount, cpI);
        ArrayUtils.RemoveAtSwapBack(handle, entityCount, cpI);
        ArrayUtils.RemoveAtSwapBack(aosData, entityCount, cpI);
        ArrayUtils.RemoveAtSwapBack(unityComps, entityCount, cpI);
        entityCount--;
        if (swappedCp != null)
            // Last Cp was swapped to cpI, so update Id.
            swappedCp.I = cpI;
        Debug.Log($"Unregistered (and destroyed) {typeof(CpHandle)} id: {cpI}.");
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
            if (aosData[i].pendingUnregister)
                continue;
            Debug.Assert(classRefs[i].st_cur != null, $"cur st was null for {i}.");
            classRefs[i].st_cur.PhysicsTick();
        }
    }

    void UpdateGroundCheck() {
        for (int i = 0; i < entityCount; i++) {
            if (aosData[i].pendingUnregister)
                continue;
            GetData(i).isGrounded = CcMov.IsGrounded(
                unityComps[i].cc,
                out bool hitSomething,
                out RaycastHit groundCastResult
            );
            GetData(i).groundCastHitSomething = hitSomething;
            GetData(i).groundCastNrm = groundCastResult.normal;
        }
    }

    // ------------------------------------------------------------
    // Tick Methods
    // ------------------------------------------------------------

    public void Tick(float dt) {
        // Navigation target info is calculated only once per frame (if any request it).
        for (int i = 0; i < entityCount; i++) {
            if (aosData[i].pendingUnregister)
                continue;
            aosData[i].navTgtInfo.hasUpdatedNavTgtInfoThisTick = false;
            GetData(i).curStDur += dt;
        }
        Tick_ReadMovInput();
        Tick_InputBuffer(dt);
        Tick_Fsm();
        Tick_Mov(dt);
    }

    void Tick_Fsm() {
        for (int i = 0; i < entityCount; i++) {
            if (aosData[i].pendingUnregister)
                continue;
            classRefs[i].st_cur.Tick();
        }
    }

    void Tick_ReadMovInput() {
        for (int i = 0; i < entityCount; i++) {
            if (aosData[i].pendingUnregister)
                continue;
            if (classRefs[i].cpCtrl == null)
                continue;
            //GetAos(i).input_atk_Light = classRefs[i].cpCtrl.TryConsume_Atk_Light();
            //GetAos(i).input_atk_Heavy = classRefs[i].cpCtrl.TryConsume_Atk_Heavy();
            //GetAos(i).input_atk_Ult = classRefs[i].cpCtrl.TryConsume_Atk_Ult();
            //GetAos(i).input_dodge = classRefs[i].cpCtrl.TryConsume_Dodge();
            if (classRefs[i].cpCtrl.Input_Mov.sqrMagnitude > PlrConfigs.inst.movInputSqrDeadzone) {
                GetData(i).input_mov = classRefs[i].cpCtrl.Input_Mov;
                GetData(i).input_mov_LastNonZero = classRefs[i].cpCtrl.Input_Mov;
            } else
                GetData(i).input_mov = Vector2.zero;
            //Dbg.Log($"light attack input: {GetAos(i).input_atk_Light}", cp[i], aosData[i].enableDbgMsgs);
            //Debug.Log($"{i} mov input mag: {math.length(data.input_mov[i])}.");
        }
    }

    void Tick_InputBuffer(float dt) {
        for (int i = 0; i < entityCount; i++) {
            if (aosData[i].pendingUnregister)
                continue;
            if (classRefs[i].cpCtrl == null)
                continue;
            if (classRefs[i].cpCtrl.TryConsume_Atk_Light())
                InputBufferUtils.BufferInput(
                    ref GetData(i).inputBuffer_BufferedInput,
                    ref GetData(i).inputBuffer_RemainingTime,
                    BufferableInput.RShldr,
                    GlobalData.inst.inputBuffer_Dur
                );
            else if (classRefs[i].cpCtrl.TryConsume_Atk_Heavy())
                InputBufferUtils.BufferInput(
                    ref GetData(i).inputBuffer_BufferedInput,
                    ref GetData(i).inputBuffer_RemainingTime,
                    BufferableInput.RTrg,
                    GlobalData.inst.inputBuffer_Dur
                );
            else if (classRefs[i].cpCtrl.TryConsume_Atk_Ult())
                InputBufferUtils.BufferInput(
                    ref GetData(i).inputBuffer_BufferedInput,
                    ref GetData(i).inputBuffer_RemainingTime,
                    BufferableInput.LShldr,
                    GlobalData.inst.inputBuffer_Dur
                );
            else if (classRefs[i].cpCtrl.TryConsume_Dodge())
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
            if (aosData[i].pendingUnregister)
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
                handle[i].transform.rotation = TrfMathUtils.RotateFwdToTgt(
                    inst.handle[i].transform.rotation,
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
            unityComps[i].cc.Move(totalMov);
            // Save final velocity back to cp data.
            GetData(i).vel_Hor = new float2(totalMov.x, totalMov.z) / dt;
            GetData(i).vel_Ver = totalMov.y / dt;
            // NavMeshAgent will drift away from the capsule pawn transform if you don't set it back here.
            unityComps[i].navMeshAgent.nextPosition = handle[i].transform.position;
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
            if (aosData[i].pendingUnregister)
                continue;
            //Debug.Log($"{animEventPlrData[i]}");
            //Debug.Log($"{unityComps[i].anim == null}");
            //Debug.Log($"{unityComps[i].animEvents == null}");
            AnimEventPlr.Tick(
                i,
                ref animEventPlrData[i],
                unityComps[i].anim,
                unityComps[i].animEventHandler.animEvent
            );
        }
    }

    void LateTick_Fsm() {
        for (int i = 0; i < entityCount; i++) {
            if (aosData[i].pendingUnregister)
                continue;
            classRefs[i].st_cur.LateTick();
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
            if (aosData[i].pendingUnregister)
                UnregisterNDestroy(i);
            else
                i++;
        }
    }

    // ------------------------------------------------------------
    // Other Methods
    // ------------------------------------------------------------

    public static ref Cp_AosData GetData(int cpI)
        => ref inst.aosData[cpI];

    /// <summary>
    /// This should always be called when cp act state is switched!
    /// </summary>
    public void OnStateSwitched(int cpI, IFsmSt newSt) {
        GetData(cpI).curStDur = 0;
        if (classRefs[cpI].cpCtrl == null)
            GetData(cpI).input_mov_WhenLastSwitchedSt
                = GetData(cpI).input_mov;
        else {
            if (classRefs[cpI].cpCtrl.Input_Mov.sqrMagnitude > PlrConfigs.inst.movInputSqrDeadzone)
                GetData(cpI).input_mov_WhenLastSwitchedSt
                    = GetData(cpI).input_mov;
            else
                GetData(cpI).input_mov_WhenLastSwitchedSt = float2.zero;
        }
    }

    /// <summary>
    /// Marks the entity for being unregistered and destroyed and invokes <see cref="Cp_AosData"/>.<br/>
    /// NOTE: DO NOT DESTROY A <see cref="CpHandle"/> DIRECTLY OR ASSIGN
    /// <see cref="Cp_AosData.pendingUnregister"/>, INSTEAD CALL THIS! 
    /// = true!!!
    /// </summary>
    public void MarkForPendingUnregister(int cpI) {
        aosData[cpI].pendingUnregister = true;
        aosData[cpI].action_markedForPendingUnregister?.Invoke();
    }

    /// <summary>
    /// Call this if you want to make a cp listen to a controller, i.e. get possessed by a controller.
    /// </summary>
    public static void StartListeningToCtrlInput(int cpI, ICpCtrlInputter ctrl) {
        Debug.Assert(inst.classRefs[cpI].cpCtrl == null, $"{cpI} already listening to a ctrl!");
        inst.classRefs[cpI].cpCtrl = ctrl;
    }

    /// <summary>
    /// NOTE: Never directly call Fsm.Switch state since that will bypass calling
    /// <see cref="OnStateSwitched"/>. (10.9.2026)
    /// </summary>
    public void SwitchActSt(Func<IFsmSt_Cp> enterFunc, int cpI){
        Fsm.SwitchSt(
            enterFunc,
            ref classRefs[cpI].st_cur,
            ref classRefs[cpI].st_prev,
            ref GetData(cpI).isSwitchingSt
            //CpMgr.GetData(cpI).enableDbgMsgs
        );
        OnStateSwitched(cpI, classRefs[cpI].st_cur);
    }

    public void SwitchToInitActSt(int cpI) {
        Debug.Log($"{cpI} switching to init state", this);
        // NOTE: This is currently always enters to idle state. (6.9.2026)
        SwitchActSt(() => classRefs[cpI].actSts.idle.Enter(), cpI);
        //Debug.Log($"{id} state initialized to : {initSt}", this);
    }

    /// <summary>
    /// Tries to find any <see cref="CpHandle"/> considered an "enemy" to <paramref name="cpI"/>. Returns
    /// null if none found.
    /// </summary>
    public static CpHandle TryFindEnemy(int cpI) {
        for (int i = 0; i < inst.entityCount; i++) {
            if (i == cpI)
                continue;
            PawnTeam candTeam = CpMgr.inst.aosData[i].team;
            if (candTeam == PawnTeam.FriendToAll)
                continue;
            if (candTeam == PawnTeam.EnemyToAll || candTeam != CpMgr.inst.aosData[cpI].team) {
                //Debug.Log("Found tgt: " + i);
                return inst.handle[i];
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
                ref classRefs[cpI].st_cur,
                ref classRefs[cpI].st_prev,
                ref GetData(cpI).isSwitchingSt
                // CpMgr.GetSoa(cpI).enableDebugMsgs
            )
        ) {
            OnStateSwitched(cpI, classRefs[cpI].st_cur);
            return true;
        }
        return false;
    }
}