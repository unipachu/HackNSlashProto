using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Capsule pawn (i.e. player or ai controlled character that uses capsule collision for movement) manager.
/// </summary>
public class CpMgr : Singleton<CpMgr> {
    [Header("Settings")]
    // TODO: Make private after creating ai controller factory.
    public int maxCps = 1;

    [HideInInspector] public AnimEventPlrData[] animEventPlrData;
    [HideInInspector] public Cp_AosData[] aosData;
    [HideInInspector] public Cp_BrainData[] brainData;
    [HideInInspector] public Cp_NonUnityCompClassRefs[] classRefs;
    [HideInInspector] public CpRegisterer[] cp;
    [HideInInspector] public NativeList<Cp_SoaData> soaData;
    [HideInInspector] public Cp_UnityComps[] unityComps;

    public void Init() {
        cp = new CpRegisterer[maxCps];
        animEventPlrData = new AnimEventPlrData[maxCps];
        aosData = new Cp_AosData[maxCps];
        brainData = new Cp_BrainData[maxCps];
        classRefs = new Cp_NonUnityCompClassRefs[maxCps];
        soaData = GeneralUtils.AllocList<Cp_SoaData>(maxCps);
        Debug.Log($"soa length in init: {soaData.Length}");
        unityComps = new Cp_UnityComps[maxCps];
    }

    void OnDestroy() {
        soaData.Dispose();
    }

    public static ref Cp_SoaData GetSoa(int cpId)
        => ref inst.soaData.ElementAt(cpId);

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
            GetSoa(i).isGrounded = CcMov.IsGrounded(
                unityComps[i].cc,
                out bool hitSomething,
                out RaycastHit groundCastResult
            );
            GetSoa(i).groundCastHitSomething = hitSomething;
            GetSoa(i).groundCastNrm = groundCastResult.normal;
        }
    }

    // ------------------------------------------------------------
    // Tick Methods
    // ------------------------------------------------------------

    public void Tick(float dt) {
        // Navigation target info is calculated only once per frame (if any request it).
        for (int i = 0; i < cp.Length; i++)
            aosData[i].navTgtInfo.hasUpdatedNavTgtInfoThisTick = false;
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
                if (!CpUtils.IsOnNavMesh(brainData[i].lockedOnTgt.CpId)) {
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
            GetSoa(i).trf_pos = unityComps[i].rootTrf.position;
            GetSoa(i).trf_rot = unityComps[i].rootTrf.rotation;
            GetSoa(i).trf_lossyScl = unityComps[i].rootTrf.lossyScale;
            GetSoa(i).lastCcVel = unityComps[i].cc.velocity;
            GetSoa(i).curStDur += dt;
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
            GetSoa(i).input_atk_Light = unityComps[i].cpCtrl.TryConsume_Atk_Light();
            GetSoa(i).input_atk_Heavy = unityComps[i].cpCtrl.TryConsume_Atk_Heavy();
            GetSoa(i).input_atk_Ult = unityComps[i].cpCtrl.TryConsume_Atk_Ult();
            GetSoa(i).input_dodge = unityComps[i].cpCtrl.TryConsume_Dodge();
            if (unityComps[i].cpCtrl.Input_Mov.sqrMagnitude > PlrConfigs.inst.movInputSqrDeadzone) {
                GetSoa(i).input_mov = unityComps[i].cpCtrl.Input_Mov;
                GetSoa(i).input_mov_LastNonZero = GetSoa(i).input_mov;
            } else {
                GetSoa(i).input_mov = Vector2.zero;
            }
            //Debug.Log($"{i} mov input mag: {math.length(data.input_mov[i])}.");
        }
    }

    void Tick_InputBuffer(float dt) {
        for (int i = 0; i < cp.Length; i++) {
            if (cp[i] == null)
                continue;
            if (GetSoa(i).input_atk_Light)
                CpInputBuffer.BufferInput(
                    ref GetSoa(i).inputBuffer_BufferedInput,
                    ref GetSoa(i).inputBuffer_RemainingTime,
                    BufferableInput.RShldr,
                    GlobalData.inst.inputBuffer_Dur
                );
            else if (GetSoa(i).input_atk_Heavy)
                CpInputBuffer.BufferInput(
                    ref GetSoa(i).inputBuffer_BufferedInput,
                    ref GetSoa(i).inputBuffer_RemainingTime,
                    BufferableInput.RTrg,
                    GlobalData.inst.inputBuffer_Dur
                );
            else if (GetSoa(i).input_atk_Ult)
                CpInputBuffer.BufferInput(
                    ref GetSoa(i).inputBuffer_BufferedInput,
                    ref GetSoa(i).inputBuffer_RemainingTime,
                    BufferableInput.LShldr,
                    GlobalData.inst.inputBuffer_Dur
                );
            else if (GetSoa(i).input_dodge)
                CpInputBuffer.BufferInput(
                    ref GetSoa(i).inputBuffer_BufferedInput,
                    ref GetSoa(i).inputBuffer_RemainingTime, 
                    BufferableInput.BtnE,
                    GlobalData.inst.inputBuffer_Dur
                );
            // Clear input if buffer time passed.
            if (GetSoa(i).inputBuffer_RemainingTime <= 0)
                continue;
            GetSoa(i).inputBuffer_RemainingTime -= dt;
            //Debug.Log("remaining time: " + remainingTime);
            if (GetSoa(i).inputBuffer_RemainingTime <= 0)
                CpInputBuffer.Clear(
                    ref GetSoa(i).inputBuffer_BufferedInput,
                    ref GetSoa(i).inputBuffer_RemainingTime
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
                !float.IsNaN(GetSoa(i).vel_Hor.x) && !float.IsNaN(GetSoa(i).vel_Hor.y),
                $"{i} vel_hor had NaN: {GetSoa(i).vel_Hor}"
            );
            //Debug.Log($"UpdateMov: data.vel_Hor before calculations: {data.vel_Hor}");
            GetSoa(i).vel_Hor = Vector2.MoveTowards(
                GetSoa(i).vel_Hor,
                GetSoa(i).movInput_tgtHorDir * GetSoa(i).movInput_tgtHorSpd,
                GetSoa(i).movInput_horAcc * dt
            );
            // Skip rotation if character is already rotated towards linear movement target direction.
            if (math.lengthsq(GetSoa(i).movInput_tgtHorDir) > 0.0001f) {
                GetSoa(i).trf_rot = TrfMathUtils.RotateFwdToTgt(
                    GetSoa(i).trf_rot,
                    GetSoa(i).movInput_yawSpd,
                    GetSoa(i).movInput_tgtHorDir
                );
                unityComps[i].rootTrf.rotation = GetSoa(i).trf_rot;
            }
            if (GetSoa(i).isAffectedByGravity)
                // NOTE: This will override previously calculated horizontal velocity if the player is
                // NOTE C: sliding down a slope. (9.9.2026)
                CcMov.ApplyGravityNSlideDownSlopes(i, dt);
            else
                // NOTE: If not using gravitational acceleration, ver velocity is reseted every tick. This
                // NOTE C: way we don't accidentally accumulate velocity when using animation root motion
                // NOTE C: for vertical movement.
                GetSoa(i).vel_Ver = 0;
            // NOTE: Additional linear movement is used to apply animation root delta lin movement (9.9.2026)
            Vector3 totalMov = (Vector3)GetSoa(i).movInput_additionalLinMov
                + new Vector3(GetSoa(i).vel_Hor.x, GetSoa(i).vel_Ver, GetSoa(i).vel_Hor.y) * dt;
            //Debug.Log($"UpdateMov: totalMov: {totalMov}");
            unityComps[i].cc.Move(totalMov);
            // Save final velocity back to cp data.
            GetSoa(i).vel_Hor = new float2(totalMov.x, totalMov.z) / dt;
            GetSoa(i).vel_Ver = totalMov.y / dt;
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
    /// Registers new capsule pawn. Returns the index of the registered data, or -1 on failure.
    /// </summary>
    public int Register(CpRegisterer cp, So_CpData so, Cp_UnityComps unityComps, So_BtRootNode bt) {
        //Debug.Log($"Start registering {cp}", cp);
        int freeI = -1;
        for (int i = 0; i < this.cp.Length; i++) {
            if (this.cp[i] == null) {
                freeI = i;
                break;
            }
        }
        if (freeI == -1) {
            Debug.LogError($"Capsule Pawn Entities at capacity ({maxCps})");
            return -1;
        }
        this.cp[freeI] = cp; 
        // Brain data
        brainData[freeI].agentDesiredVel = float3.zero;
        brainData[freeI].aggroRange = so.brain_AggroRange;
        brainData[freeI].atkRange = so.brain_AtkRange;
        brainData[freeI].distToTgt = 0;
        brainData[freeI].hasTgt = false;
        brainData[freeI].inAggroRange = false;
        brainData[freeI].inAtkRange = false;
        brainData[freeI].lockedOnTgt = null;
        // Structure of arrays data
        Cp_SoaData newData = new();
        newData.actStSt_Impact_YawSpd = so.impact_YawSpd;
        newData.curStDur = 0;
        newData.groundCastHitSomething = false;
        newData.groundCastNrm = float3.zero;
        newData.groundSnapVerDownSpd = so.groundSnapVerDownSpd;
        newData.hp_Cur = so.maxHP;
        newData.hp_Max = so.maxHP;
        newData.input_mov = float2.zero;
        newData.input_mov_LastNonZero = float2.zero;
        newData.input_mov_WhenLastSwitchedSt = float2.zero;
        newData.input_atk_Light = false;
        newData.input_atk_Heavy = false;
        newData.input_atk_Ult = false;
        newData.input_dodge = false;
        newData.invul = false;
        newData.isAffectedByGravity = true;
        newData.isGrounded = true;
        newData.lastCcVel = float3.zero;
        newData.lastKnockbackStr = 0;
        newData.lastRecievedHitDir = float3.zero;
        newData.st_AtkHorSlash_Windup_MaxAngSpd = so.st_AtkHorSlash_Windup_YawSpd;
        newData.st_AtkJump_DownSpeedAfterJumpFinished = so.st_AtkJump_DownSpeedAfterJumpFinished;
        newData.st_Dodge_YawSpd = so.st_Dodge_YawAngSpd;
        newData.st_Falling_LandingStFallDistThreshold = so.st_Falling_LandingStFallDistThreshold;
        newData.st_Falling_HorAcc = so.st_Falling_HorAcc;
        newData.st_Falling_TgtHorSpd = so.st_Falling_TgtHorSpd;
        newData.trf_pos = float3.zero;
        newData.trf_rot = quaternion.identity;
        newData.trf_lossyScl = new float3(1);
        newData.vel_Hor = float2.zero;
        newData.vel_Ver = 0;
        newData.occupied = true;
        newData.walkLinAcc = so.walkHorAcc;
        newData.walkMaxLinSpd = so.walkTgtHorSpd;
        newData.walkYawSpd = so.walkYawSpd;
        soaData.Add(newData);
        // Array of structs data.
        this.aosData[freeI] = new();
        aosData[freeI].dodgeHorMovSpdMult = 1.5f; // NOTE: hard coded.
        aosData[freeI].enableDebugMsgs = so.enableDebugMsgs;
        aosData[freeI].st_AtkFlying_TgtHorSpd = so.st_AtkFlying_TgtHorSpeed;
        // NOTE: We set default maxDistToNavMesh to 0.2! (10.9.2026)
        aosData[freeI].navTgtInfo = new(false, false, 0.2f); 
        this.unityComps[freeI] = unityComps;
        this.classRefs[freeI] = new Cp_NonUnityCompClassRefs(freeI);
        // TODO: Should have a reference to a generic controller which could be player or ai. (6.9.2026)
        if (bt != null)
            BtMgr.inst.Register(freeI, bt);
        //Debug.Log($"Switching {freeI} to initial act st!", this);
        SwitchToInitActSt(freeI);
        return freeI;
    }

    // TODO: If you use cp ref to mark entity as unoccupied, then
    public void Unregister(int cpId) {
        if (cp[cpId] == null) {
            Debug.LogError($"Capsule pawn with id {cpId} has not been registered!");
            return;
        }
        GetSoa(cpId).occupied = false;
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
            ref aosData[cpId].isSwitchingSt
            //aosData[cpId].enableDebugMsgs
        );
        OnStateSwitched(cpId, classRefs[cpId].st_cur);
    }

    /// <summary>
    /// NOTE: Never directly call Fsm.Switch state since that will bypass calling
    /// <see cref="OnStateSwitched"/>. (10.9.2026)
    /// </summary>
    public bool TrySwitchActSt(Func<IFsmSt_Cp> enterFunc, int cpId) {
        if(
            Fsm.TrySwitchState(
                enterFunc,
                ref classRefs[cpId].st_cur,
                ref classRefs[cpId].st_prev,
                ref aosData[cpId].isSwitchingSt
                //aosData[cpId].enableDebugMsgs
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
        GetSoa(cpId).curStDur = 0;
        if (unityComps[cpId].cpCtrl == null)
            GetSoa(cpId).input_mov_WhenLastSwitchedSt
                = GetSoa(cpId).input_mov;
        else {
            if (unityComps[cpId].cpCtrl.Input_Mov.sqrMagnitude > PlrConfigs.inst.movInputSqrDeadzone)
                GetSoa(cpId).input_mov_WhenLastSwitchedSt
                    = GetSoa(cpId).input_mov;
            else
                GetSoa(cpId).input_mov_WhenLastSwitchedSt = float2.zero;
        }
    }
}