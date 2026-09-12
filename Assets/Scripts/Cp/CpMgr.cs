using System;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Capsule pawn (i.e. player or ai controlled character that uses capsule collision for movement) manager.
/// </summary>
public class CpMgr : Singleton<CpMgr> {
    [Header("Settings")]
    // TODO: Make private after creating ai controller factory. Maybe call it "InitCpCapacity"

    [Tooltip("Initial capacity of arrays. They allocate more space if needed (but do not deallocate even" +
        "if pawns are unregistered.)")]
    public int initCapacity = 1;

    [HideInInspector] public AnimEventPlrData[] animEventPlrData;
    [HideInInspector] public Cp_BrainData[] brainData;
    [HideInInspector] public Cp_NonUnityCompClassRefs[] classRefs;
    [HideInInspector] public CpRegisterer[] cp;
    [HideInInspector] public NativeList<Cp_AosData> aosData;
    [HideInInspector] public Cp_UnityComps[] unityComps;

    /// <summary>
    /// Used to set the used length of the arrays (since they do not reallocate when elements are removed).
    /// </summary>
    int cpCount;

    public int CpCount => cpCount;

    public void Init() {
        animEventPlrData = new AnimEventPlrData[initCapacity];
        brainData = new Cp_BrainData[initCapacity];
        classRefs = new Cp_NonUnityCompClassRefs[initCapacity];
        cp = new CpRegisterer[initCapacity];
        aosData = GeneralUtils.AllocList<Cp_AosData>(initCapacity);
        //Debug.Log($"soa length in init: {aosData.Length}");
        unityComps = new Cp_UnityComps[initCapacity];
    }

    void OnDestroy() {
        aosData.Dispose();
    }

    public static ref Cp_AosData GetAos(int cpId)
        => ref inst.aosData.ElementAt(cpId);

    // ------------------------------------------------------------
    // Fixed Tick Methods
    // ------------------------------------------------------------

    public void FixedTick() {
        UpdateGroundCheck();
        FixedTick_Fsm();
    }

    void FixedTick_Fsm() {
        for (int i = 0; i < cp.Length; i++) {
            if (cp[i] == null)
                continue;
            Debug.Assert(classRefs[i].st_cur != null, $"cur st was null for {i}.");
            classRefs[i].st_cur.PhysicsTick();
        }
    }

    void UpdateGroundCheck() {
        for (int i = 0; i < cp.Length; i++) {
            if (cp[i] == null)
                continue;
            GetAos(i).isGrounded = CcMov.IsGrounded(
                unityComps[i].cc,
                out bool hitSomething,
                out RaycastHit groundCastResult
            );
            GetAos(i).groundCastHitSomething = hitSomething;
            GetAos(i).groundCastNrm = groundCastResult.normal;
        }
    }

    // ------------------------------------------------------------
    // Tick Methods
    // ------------------------------------------------------------

    public void Tick(float dt) {
        // Navigation target info is calculated only once per frame (if any request it).
        for (int i = 0; i < cp.Length; i++)
            aosData.ElementAt(i).navTgtInfo.hasUpdatedNavTgtInfoThisTick = false;
        Tick_FromNonNative(dt);
        Tick_Input();
        Tick_InputBuffer(dt);
        Tick_Sensing();
        Tick_AgentMovInput();
        Tick_Fsm();
        Tick_Mov(dt);
    }

    // TODO: Update in Tick_FromNonNative
    // TODO C: Or maybe in Tick_Sensing.
    void Tick_AgentMovInput() {
        for (int i = 0; i < cp.Length; i++) {
            if (cp[i] == null || unityComps[i].navMeshAgent == null) // TODO: Remove nav mesh check and only tick this in ai controller.
                continue;
            if (brainData[i].lockedOnTgt.Trf == null) {
                //Dbg.Log(
                //    $"{i} Set agent desired vel to 0 because tgt was null: {brainData[i].lockedOnTgt.Trf}",
                //    aosData[i].enableDebugMsgs
                //);
                unityComps[i].navMeshAgent.ResetPath();
                brainData[i].agentDesiredVel = float3.zero;
                continue;
            }
            // NOTE: nav mesh agent can drift away from the actual transform because nav mesh agents suck.
            unityComps[i].navMeshAgent.nextPosition = unityComps[i].rootTrf.position;
                // NOTE: We need to check this manually since SetDestination does not have option to set
                // NOTE C: target sample position max distance.
                if (!CpUtils.IsOnNavMesh(brainData[i].lockedOnTgt.Id)) {
                //Dbg.Log($"{i} Set agent desired vel to 0 since tgt was not on navmesh.",
                //    aosData[i].enableDebugMsgs);
                unityComps[i].navMeshAgent.ResetPath();
                brainData[i].agentDesiredVel = float3.zero;
                continue;
            }
            // TODO: The point of this is to START path finding calculation if there is no previous path
            // TODO C: calculation (e.g. no path status) and if the agent is not currenly calculating a path.
            // TODO C: I think this might be incorrect way to do it but the agent navigation seems to work
            // TODO C: well enough for now.
            if (!unityComps[i].navMeshAgent.hasPath) {
                //Dbg.Log($"{i} Agent had no path. Set destination.", data.enableDebugMsgs[i]);
                unityComps[i].navMeshAgent.SetDestination(brainData[i].lockedOnTgt.Trf.position);
                continue;
            }
            // If we are close enough to the destination, stop desiring movement.
            if(
                Vector3.SqrMagnitude(
                    unityComps[i].navMeshAgent.destination - unityComps[i].rootTrf.position
                ) < 0.1f // NOTE: Stopping distane is hard coded.
            ) {
                //Dbg.Log($"{i} Set agent desired vel to 0 since we reached the target vicinity.",
                //data.enableDebugMsgs[i]);
                unityComps[i].navMeshAgent.ResetPath();
                brainData[i].agentDesiredVel = float3.zero;
                continue;
            }
            // NOTE: We only use the current unfinished path if last path calculation was completed. This way
            // NOTE C: if we get sequential failed path finding attempts, the character will not move at all
            // NOTE C: (instead of jittering a little because of the partial paths).
            if (unityComps[i].navMeshAgent.pathPending) {
                //Dbg.Log($"{i} Path was pending.", data.enableDebugMsgs[i]);
                if (brainData[i].prevCalculatePathSucceeded)
                    brainData[i].agentDesiredVel = unityComps[i].navMeshAgent.desiredVelocity;
                else
                    brainData[i].agentDesiredVel = float3.zero;
                continue;
            }
            if (unityComps[i].navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete) {
                brainData[i].prevCalculatePathSucceeded = true;
                //Debug.Log($"Entity id: {i}");
                //Debug.Log($"Tgt on nav mesh: {tgtOnNavMesh}");
                //Debug.Log($"prevCalculatePathSucceeded: {brainData[i].prevCalculatePathSucceeded}");
                //Debug.Log($"pending: {unityComps[i].navMeshAgent.pathPending}");
                //Debug.Log($"status: {unityComps[i].navMeshAgent.pathStatus}");
                //Debug.Log($"has path: {unityComps[i].navMeshAgent.hasPath}");
                //Debug.Log($"tgt: {unityComps[i].tgt.position}");
                //Debug.Log($"destination: {unityComps[i].navMeshAgent.destination}");
                //Debug.Log($"path end: {unityComps[i].navMeshAgent.pathEndPosition}");
                //Debug.Log($"desired vel: {unityComps[i].navMeshAgent.desiredVelocity}");
                //Debug.Log($"steering tgt: {unityComps[i].navMeshAgent.steeringTarget}");
                // NOTE: We use desired velocity instead of steering target, because steering target doesn't
                // NOTE C: use avoidance.
                brainData[i].agentDesiredVel = unityComps[i].navMeshAgent.desiredVelocity;
            }
            else {
                //Dbg.Log($"{i} Did not find path. Setting desired vel to 0.", data.enableDebugMsgs[i]);
                brainData[i].prevCalculatePathSucceeded = false;
                brainData[i].agentDesiredVel = float3.zero;
            }
            unityComps[i].navMeshAgent.SetDestination(brainData[i].lockedOnTgt.Trf.position);
        }
    }

    /// <summary>
    /// Update data from non native sources, e.g. from Monobehavior components.
    /// </summary>
    void Tick_FromNonNative(float dt) {
        for (int i = 0; i < cp.Length; i++) {
            if (cp[i] == null)
                continue;
            GetAos(i).trf_pos = unityComps[i].rootTrf.position;
            GetAos(i).trf_rot = unityComps[i].rootTrf.rotation;
            GetAos(i).trf_lossyScl = unityComps[i].rootTrf.lossyScale;
            GetAos(i).lastCcVel = unityComps[i].cc.velocity;
            GetAos(i).curStDur += dt;
        }
    }

    void Tick_Fsm() {
        for (int i = 0; i < cp.Length; i++) {
            if (cp[i] == null)
                continue;
            classRefs[i].st_cur.Tick();
        }
    }

    void Tick_Input() {
        for (int i = 0; i < cp.Length; i++) {
            if (cp[i] == null || unityComps[i].cpCtrl == null)
                continue;
            GetAos(i).input_atk_Light = unityComps[i].cpCtrl.TryConsume_Atk_Light();
            GetAos(i).input_atk_Heavy = unityComps[i].cpCtrl.TryConsume_Atk_Heavy();
            GetAos(i).input_atk_Ult = unityComps[i].cpCtrl.TryConsume_Atk_Ult();
            GetAos(i).input_dodge = unityComps[i].cpCtrl.TryConsume_Dodge();
            if (unityComps[i].cpCtrl.Input_Mov.sqrMagnitude > PlrConfigs.inst.movInputSqrDeadzone) {
                GetAos(i).input_mov = unityComps[i].cpCtrl.Input_Mov;
                GetAos(i).input_mov_LastNonZero = GetAos(i).input_mov;
            } else {
                GetAos(i).input_mov = Vector2.zero;
            }
            //Debug.Log($"{i} mov input mag: {math.length(data.input_mov[i])}.");
        }
    }

    void Tick_InputBuffer(float dt) {
        for (int i = 0; i < cp.Length; i++) {
            if (cp[i] == null)
                continue;
            if (GetAos(i).input_atk_Light)
                CpInputBuffer.BufferInput(
                    ref GetAos(i).inputBuffer_BufferedInput,
                    ref GetAos(i).inputBuffer_RemainingTime,
                    BufferableInput.RShldr,
                    GlobalData.inst.inputBuffer_Dur
                );
            else if (GetAos(i).input_atk_Heavy)
                CpInputBuffer.BufferInput(
                    ref GetAos(i).inputBuffer_BufferedInput,
                    ref GetAos(i).inputBuffer_RemainingTime,
                    BufferableInput.RTrg,
                    GlobalData.inst.inputBuffer_Dur
                );
            else if (GetAos(i).input_atk_Ult)
                CpInputBuffer.BufferInput(
                    ref GetAos(i).inputBuffer_BufferedInput,
                    ref GetAos(i).inputBuffer_RemainingTime,
                    BufferableInput.LShldr,
                    GlobalData.inst.inputBuffer_Dur
                );
            else if (GetAos(i).input_dodge)
                CpInputBuffer.BufferInput(
                    ref GetAos(i).inputBuffer_BufferedInput,
                    ref GetAos(i).inputBuffer_RemainingTime, 
                    BufferableInput.BtnE,
                    GlobalData.inst.inputBuffer_Dur
                );
            // Clear input if buffer time passed.
            if (GetAos(i).inputBuffer_RemainingTime <= 0)
                continue;
            GetAos(i).inputBuffer_RemainingTime -= dt;
            //Debug.Log("remaining time: " + remainingTime);
            if (GetAos(i).inputBuffer_RemainingTime <= 0)
                CpInputBuffer.Clear(
                    ref GetAos(i).inputBuffer_BufferedInput,
                    ref GetAos(i).inputBuffer_RemainingTime
                );
        }
    }

    void Tick_Mov(float dt) {
        for (int i = 0; i < cp.Length; i++) {
            if (cp[i] == null)
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
                !float.IsNaN(GetAos(i).vel_Hor.x) && !float.IsNaN(GetAos(i).vel_Hor.y),
                $"{i} vel_hor had NaN: {GetAos(i).vel_Hor}"
            );
            //Debug.Log($"UpdateMov: data.vel_Hor before calculations: {data.vel_Hor}");
            GetAos(i).vel_Hor = Vector2.MoveTowards(
                GetAos(i).vel_Hor,
                GetAos(i).movInput_tgtHorDir * GetAos(i).movInput_tgtHorSpd,
                GetAos(i).movInput_horAcc * dt
            );
            // Skip rotation if character is already rotated towards linear movement target direction.
            if (math.lengthsq(GetAos(i).movInput_tgtHorDir) > 0.0001f) {
                GetAos(i).trf_rot = TrfMathUtils.RotateFwdToTgt(
                    GetAos(i).trf_rot,
                    GetAos(i).movInput_yawSpd,
                    GetAos(i).movInput_tgtHorDir
                );
                unityComps[i].rootTrf.rotation = GetAos(i).trf_rot;
            }
            if (GetAos(i).isAffectedByGravity)
                // NOTE: This will override previously calculated horizontal velocity if the player is
                // NOTE C: sliding down a slope. (9.9.2026)
                CcMov.ApplyGravityNSlideDownSlopes(i, dt);
            else
                // NOTE: If not using gravitational acceleration, ver velocity is reseted every tick. This
                // NOTE C: way we don't accidentally accumulate velocity when using animation root motion
                // NOTE C: for vertical movement.
                GetAos(i).vel_Ver = 0;
            // NOTE: Additional linear movement is used to apply animation root delta lin movement (9.9.2026)
            Vector3 totalMov = (Vector3)GetAos(i).movInput_additionalLinMov
                + new Vector3(GetAos(i).vel_Hor.x, GetAos(i).vel_Ver, GetAos(i).vel_Hor.y) * dt;
            //Debug.Log($"UpdateMov: totalMov: {totalMov}");
            unityComps[i].cc.Move(totalMov);
            // Save final velocity back to cp data.
            GetAos(i).vel_Hor = new float2(totalMov.x, totalMov.z) / dt;
            GetAos(i).vel_Ver = totalMov.y / dt;
            // NavMeshAgent will drift away from the capsule pawn transform if you don't set it back here.
            unityComps[i].navMeshAgent.nextPosition = unityComps[i].rootTrf.position;
        }
    }

    void Tick_Sensing() {
        for (int i = 0; i < cp.Length; i++) {
            if (cp[i] == null)
                continue;
            // TODO: Use better logic for sensing player.
            brainData[i].lockedOnTgt
                = GameObject.Find("Cp_Plr").GetComponent<LockOnTgt>();
            //Debug.Log(brainData[i].tgtPose.position);
            if (brainData[i].lockedOnTgt != null) {
                brainData[i].distToTgt = Vector3.Distance(
                    unityComps[i].rootTrf.position,
                    brainData[i].lockedOnTgt.Trf.position
                );
                brainData[i].hasTgt = true;
                brainData[i].inAggroRange
                    = Vector3.Distance(
                        unityComps[i].rootTrf.position,
                    brainData[i].lockedOnTgt.Trf.position) < brainData[i].aggroRange;
                brainData[i].inAtkRange
                    = Vector3.Distance(
                        unityComps[i].rootTrf.position,
                    brainData[i].lockedOnTgt.Trf.position
                ) < brainData[i].atkRange;
                //Dbg.Log($"in atk range: {brainData[i].inAtkRange}", aosData[i].enableDebugMsgs);
            }
            else
                brainData[i].hasTgt = false;
        }
    }

    // ------------------------------------------------------------
    // Late Tick Methods
    // ------------------------------------------------------------

    // TODO: Remember to call this from game manager.
    public void LateTick() {
        LateTick_AnimEventPlr();
        LateTick_Fsm();
    }

    void LateTick_AnimEventPlr() {
        for (int i = 0; i < cp.Length; i++) {
            if (cp[i] == null)
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
        for (int i = 0; i < cp.Length; i++) {
            if (cp[i] == null)
                continue;
            classRefs[i].st_cur.LateTick();
        }
    }

    // ------------------------------------------------------------
    // Other Methods
    // ------------------------------------------------------------

    /// <summary>
    /// Registers new capsule pawn.
    /// </summary>
    public void Register(CpRegisterer newCp, So_CpData so, Cp_UnityComps newUnityComps, So_BtRootNode newBt) {
        // NOTE: Index = new count - 1.
        //Debug.Log($"Start registering {cp}", cp);
        ArrayUtils.Add(ref animEventPlrData, cpCount, default); // This is set when switching to init act state.
        ArrayUtils.Add(ref cp, cpCount, newCp);
        // Brain data
        var newBrainData = new Cp_BrainData();
        newBrainData.agentDesiredVel = float3.zero;
        newBrainData.aggroRange = so.brain_AggroRange;
        newBrainData.atkRange = so.brain_AtkRange;
        newBrainData.distToTgt = 0;
        newBrainData.hasTgt = false;
        newBrainData.inAggroRange = false;
        newBrainData.inAtkRange = false;
        newBrainData.lockedOnTgt = null;
        ArrayUtils.Add(ref brainData, cpCount, newBrainData);
        // Structure of arrays data
        Cp_AosData newAosData = new();
        newAosData.act_BasicImpact_YawSpd = so.impact_YawSpd;
        newAosData.curStDur = 0;
        newAosData.groundCastHitSomething = false;
        newAosData.groundCastNrm = float3.zero;
        newAosData.groundSnapVerDownSpd = so.groundSnapVerDownSpd;
        newAosData.hp_Cur = so.maxHP;
        newAosData.hp_Max = so.maxHP;
        newAosData.input_mov = float2.zero;
        newAosData.input_mov_LastNonZero = float2.zero;
        newAosData.input_mov_WhenLastSwitchedSt = float2.zero;
        newAosData.input_atk_Light = false;
        newAosData.input_atk_Heavy = false;
        newAosData.input_atk_Ult = false;
        newAosData.input_dodge = false;
        newAosData.invul = false;
        newAosData.isAffectedByGravity = true;
        newAosData.isGrounded = true;
        newAosData.lastCcVel = float3.zero;
        newAosData.lastKnockbackStr = 0;
        newAosData.lastRecievedHitDir = float3.zero;
        newAosData.act_BasicWindup_MaxAngSpd = so.st_AtkHorSlash_Windup_YawSpd;
        newAosData.act_AtkJump_DownSpeedAfterJumpFinished = so.st_AtkJump_DownSpeedAfterJumpFinished;
        newAosData.act_Dodge_YawSpd = so.st_Dodge_YawAngSpd;
        newAosData.act_Falling_LandingStFallDistThreshold = so.st_Falling_LandingStFallDistThreshold;
        newAosData.act_Falling_HorAcc = so.st_Falling_HorAcc;
        newAosData.act_Falling_TgtHorSpd = so.st_Falling_TgtHorSpd;
        newAosData.trf_pos = float3.zero;
        newAosData.trf_rot = quaternion.identity;
        newAosData.trf_lossyScl = new float3(1);
        newAosData.vel_Hor = float2.zero;
        newAosData.vel_Ver = 0;
        newAosData.walkLinAcc = so.walkHorAcc;
        newAosData.walkMaxLinSpd = so.walkTgtHorSpd;
        newAosData.walkYawSpd = so.walkYawSpd;
        newAosData.act_Dodge_HorMovSpdMult = 1.5f; // NOTE: hard coded.
        newAosData.enableDebugMsgs = so.enableDebugMsgs;
        newAosData.act_AtkFlying_TgtHorSpd = so.st_AtkFlying_TgtHorSpeed;
        // NOTE: We set default maxDistToNavMesh to 0.2! (10.9.2026)
        newAosData.navTgtInfo = new(false, false, 0.2f); 
        aosData.Add(newAosData);
        ArrayUtils.Add(ref unityComps, cpCount, newUnityComps);
        ArrayUtils.Add(ref classRefs, cpCount, new Cp_NonUnityCompClassRefs(cpCount));
        // TODO: Should have a reference to a generic controller which could be player or ai. (6.9.2026)
        if (newBt != null)
            BtMgr.inst.Register(cpCount, newBt);
        //Debug.Log($"Switching {freeI} to initial act st!", this);
        newCp.Id = cpCount;
        SwitchToInitActSt(cpCount);
        cpCount++;
    }

    /// <summary>
    /// Call this before destroying a cp.
    /// </summary>
    /// <param name="cpId"></param>
    public void Unregister(int cpId) {
        if (cpId >= cpCount) {
            Debug.LogError($"{cpId} was greaterequal to {cpCount}!");
            return;
        }
        int lastId = cpCount - 1;
        CpRegisterer swappedCp = cpId != lastId ? cp[lastId] : null;
        ArrayUtils.RemoveAtSwapBack(animEventPlrData, cpCount, cpId);
        ArrayUtils.RemoveAtSwapBack(brainData, cpCount, cpId);
        ArrayUtils.RemoveAtSwapBack(classRefs, cpCount, cpId);
        ArrayUtils.RemoveAtSwapBack(cp, cpCount, cpId);
        aosData.RemoveAtSwapBack(cpId);
        ArrayUtils.RemoveAtSwapBack(unityComps, cpCount, cpId);
        cpCount--;
        if (swappedCp != null)
            // Last Cp was swapped to cpId, so update Id.
            swappedCp.Id = cpId;
    }

    public void SwitchToInitActSt(int cpId) {
        Debug.Log($"{cpId} switching to init state", this);
        // NOTE: This is currently always enters to idle state. (6.9.2026)
        SwitchActSt(() => classRefs[cpId].actSts.idle.Enter(), cpId);
        //Debug.Log($"{id} state initialized to : {initSt}", this);
    }

    /// <summary>
    /// NOTE: Never directly call Fsm.Switch state since that will bypass calling
    /// <see cref="OnStateSwitched"/>. (10.9.2026)
    /// </summary>
    public void SwitchActSt(Func<IFsmSt_Cp> enterFunc, int cpId){
        Fsm.SwitchSt(
            enterFunc,
            ref classRefs[cpId].st_cur,
            ref classRefs[cpId].st_prev,
            ref GetAos(cpId).isSwitchingSt
            // CpMgr.GetSoa(cpId).enableDebugMsgs
        );
        OnStateSwitched(cpId, classRefs[cpId].st_cur);
    }

    /// <summary>
    /// NOTE: Never directly call <see cref="Fsm.TrySwitchState"/> since that will bypass calling
    /// <see cref="OnStateSwitched"/>, so call this instead!. (10.9.2026)
    /// NOTE 2: <paramref name="enterFunc"/> return type needs to be generic (instead of IFsmSt_Cp), otherwise
    /// information of the new state type is lost. (12.9.2026)
    /// </summary>
    public bool TrySwitchActSt<TNewState>(Func<TNewState> enterFunc, int cpId) where TNewState : IFsmSt_Cp {
        if(
            Fsm.TrySwitchState(
                enterFunc,
                ref classRefs[cpId].st_cur,
                ref classRefs[cpId].st_prev,
                ref GetAos(cpId).isSwitchingSt
                // CpMgr.GetSoa(cpId).enableDebugMsgs
            )
        ) {
            OnStateSwitched(cpId, classRefs[cpId].st_cur);
            return true;
        }
        return false;
    }

    /// <summary>
    /// This should always be called when cp act state is switched!
    /// </summary>
    public void OnStateSwitched(int cpId, IFsmSt newSt) {
        GetAos(cpId).curStDur = 0;
        if (unityComps[cpId].cpCtrl == null)
            GetAos(cpId).input_mov_WhenLastSwitchedSt
                = GetAos(cpId).input_mov;
        else {
            if (unityComps[cpId].cpCtrl.Input_Mov.sqrMagnitude > PlrConfigs.inst.movInputSqrDeadzone)
                GetAos(cpId).input_mov_WhenLastSwitchedSt
                    = GetAos(cpId).input_mov;
            else
                GetAos(cpId).input_mov_WhenLastSwitchedSt = float2.zero;
        }
    }
}