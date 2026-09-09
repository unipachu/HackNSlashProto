using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Capsule pawn (i.e. player or ai controlled character that uses capsule collision for movement) manager.
/// </summary>
public class CpMgr : Singleton<CpMgr> {
    [Header("Settings")]
    public int maxCps = 1;

    [HideInInspector] public AnimEventPlrData[] animEventPlrData;
    [HideInInspector] public Cp_AosData[] aosData;
    [HideInInspector] public Cp_BrainData[] brainData;
    [HideInInspector] public Cp_NonUnityCompClassRefs[] classRefs;
    [HideInInspector] public Cp_SoaData soaData;
    [HideInInspector] public Cp_UnityComps[] unityComps;

    public void Init() {
        animEventPlrData = new AnimEventPlrData[maxCps];
        aosData = new Cp_AosData[maxCps];
        brainData = new Cp_BrainData[maxCps];
        classRefs = new Cp_NonUnityCompClassRefs[maxCps];
        soaData = Cp_SoaData.Create(maxCps);
        unityComps = new Cp_UnityComps[maxCps];
    }

    void OnDestroy() {
        soaData.Dispose();
    }

    // ------------------------------------------------------------
    // Fixed Tick Methods
    // ------------------------------------------------------------

    public void FixedTick() {
        UpdateGroundCheck();
        FixedTick_Fsm();
    }

    void FixedTick_Fsm() {
        for (int i = 0; i < unityComps.Length; i++) {
            if (!soaData.occupied[i])
                continue;
            Debug.Assert(classRefs[i].st_cur != null, $"cur st was null for {i}.");
            classRefs[i].st_cur.PhysicsTick();
        }
    }

    void UpdateGroundCheck() {
        for (int i = 0; i < soaData.occupied.Length; i++) {
            if (!soaData.occupied[i])
                continue;
            soaData.isGrounded[i] = CcMov.IsGrounded(
                unityComps[i].cc,
                out bool hitSomething,
                out RaycastHit groundCastResult
            );
            soaData.groundCastHitSomething[i] = hitSomething;
            soaData.groundCastNrm[i] = groundCastResult.normal;
        }
    }

    // ------------------------------------------------------------
    // Tick Methods
    // ------------------------------------------------------------

    public void Tick(float dt) {
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
        for (int i = 0; i < soaData.occupied.Length; i++) {
            //Dbg.Log($"{i} tgt: {unityComps[i].tgt}", data.enableDebugMsgs[i]);
            if (!soaData.occupied[i] || unityComps[i].navMeshAgent == null)
                continue;
            if (unityComps[i].tgt == null) {
                //Dbg.Log(
                //    $"{i} Set agent desired vel to 0 because tgt was null: {unityComps[i].tgt}",
                //    data.enableDebugMsgs[i]
                //);
                unityComps[i].navMeshAgent.ResetPath();
                brainData[i].agentDesiredVel = float3.zero;
                continue;
            }
            // NOTE: nav mesh agent can drift away from the actual transform because nav mesh agents suck.
            unityComps[i].navMeshAgent.nextPosition = unityComps[i].trf.position;
            bool tgtOnNavMesh = NavMesh.SamplePosition(
                unityComps[i].tgt.position,
                out NavMeshHit hit,
                // TODO: Make So.
                0.2f,
                unityComps[i].navMeshAgent.areaMask
            );
            // NOTE: We need to check this manually since SetDestination does not have option to set target sample
            // NOTE C: position max distance.
            if (!tgtOnNavMesh) {
                //Dbg.Log($"{i} Set agent desired vel to 0 since tgt was not on navmesh.", data.enableDebugMsgs[i]);
                unityComps[i].navMeshAgent.ResetPath();
                brainData[i].agentDesiredVel = float3.zero;
                continue;
            }
            // TODO: The point of this is to START path finding calculation if there is no previous path calculation
            // TODO C: (e.g. no path status) and if the agent is not currenly calculating a path. I think this might
            // TODO C: be incorrect way to do it but the agent navigation seems to work well enough for now.
            if (!unityComps[i].navMeshAgent.hasPath) {
                //Dbg.Log($"{i} Agent had no path. Set destination.", data.enableDebugMsgs[i]);
                unityComps[i].navMeshAgent.SetDestination(unityComps[i].tgt.position);
                continue;
            }
            // If we are close enough to the destination, stop desiring movement.
            // TODO: Make So.
            if(Vector3.SqrMagnitude(unityComps[i].navMeshAgent.destination - unityComps[i].trf.position) < 0.1f) {
                //Dbg.Log($"{i} Set agent desired vel to 0 since we reached the target vicinity.", data.enableDebugMsgs[i]);
                unityComps[i].navMeshAgent.ResetPath();
                brainData[i].agentDesiredVel = float3.zero;
                continue;
            }
            // NOTE: We only use the current unfinished path if last path calculation was completed. This way if we
            // NOTE C: get sequential failed path finding attempts, the character will not move at all (instead of
            // NOTE C: jittering a little because of the partial paths).
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
            unityComps[i].navMeshAgent.SetDestination(unityComps[i].tgt.position);
        }
    }

    /// <summary>
    /// Update data from non native sources, e.g. from Monobehavior components.
    /// </summary>
    void Tick_FromNonNative(float dt) {
        for (int i = 0; i < soaData.occupied.Length; i++) {
            if (!soaData.occupied[i])
                continue;
            soaData.trf_pos[i] = unityComps[i].trf.position;
            soaData.trf_rot[i] = unityComps[i].trf.rotation;
            soaData.trf_lossyScl[i] = unityComps[i].trf.lossyScale;
            soaData.lastCcVel[i] = unityComps[i].cc.velocity;
            soaData.curStDur[i] += dt;
        }
    }

    void Tick_Fsm() {
        for (int i = 0; i < soaData.occupied.Length; i++) {
            if (!soaData.occupied[i])
                continue;
            classRefs[i].st_cur.Tick();
        }
    }

    void Tick_Input() {
        for (int i = 0; i < soaData.occupied.Length; i++) {
            if (!soaData.occupied[i] || unityComps[i].cpCtrl == null)
                continue;
            soaData.input_atk_Light[i] = unityComps[i].cpCtrl.TryConsume_Atk_Light();
            soaData.input_atk_Heavy[i] = unityComps[i].cpCtrl.TryConsume_Atk_Heavy();
            soaData.input_atk_Ult[i] = unityComps[i].cpCtrl.TryConsume_Atk_Ult();
            soaData.input_dodge[i] = unityComps[i].cpCtrl.TryConsume_Dodge();
            if (unityComps[i].cpCtrl.Input_Mov.sqrMagnitude > PlrConfigsManager.inst.movInputSqrDeadzone) {
                soaData.input_mov[i] = unityComps[i].cpCtrl.Input_Mov;
                soaData.input_mov_LastNonZero[i] = soaData.input_mov[i];
            } else {
                soaData.input_mov[i] = Vector2.zero;
            }
            //Debug.Log($"{i} mov input mag: {math.length(data.input_mov[i])}.");
        }
    }

    void Tick_InputBuffer(float dt) {
        for (int i = 0; i < soaData.occupied.Length; i++) {
            if (!soaData.occupied[i])
                continue;
            if (soaData.input_atk_Light[i])
                CpInputBuffer.BufferInput(
                    i,
                    BufferableInput.RShldr,
                    soaData.inputBuffer_BufferedInput,
                    soaData.inputBuffer_RemainingTime,
                    GlobalData.inst.inputBuffer_Dur
                );
            else if (soaData.input_atk_Heavy[i])
                CpInputBuffer.BufferInput(
                    i,
                    BufferableInput.RTrg,
                    soaData.inputBuffer_BufferedInput,
                    soaData.inputBuffer_RemainingTime,
                    GlobalData.inst.inputBuffer_Dur
                );
            else if (soaData.input_atk_Ult[i])
                CpInputBuffer.BufferInput(
                    i,
                    BufferableInput.LShldr,
                    soaData.inputBuffer_BufferedInput,
                    soaData.inputBuffer_RemainingTime,
                    GlobalData.inst.inputBuffer_Dur
                );
            else if (soaData.input_dodge[i])
                CpInputBuffer.BufferInput(
                    i,
                    BufferableInput.BtnE,
                    soaData.inputBuffer_BufferedInput,
                    soaData.inputBuffer_RemainingTime, 
                    GlobalData.inst.inputBuffer_Dur
                );
            // Clear input if buffer time passed.
            if (soaData.inputBuffer_RemainingTime[i] <= 0)
                continue;
            soaData.inputBuffer_RemainingTime[i] -= dt;
            //Debug.Log("remaining time: " + remainingTime);
            if (soaData.inputBuffer_RemainingTime[i] <= 0)
                CpInputBuffer.Clear(i, soaData.inputBuffer_BufferedInput, soaData.inputBuffer_RemainingTime);
        }
    }

    void Tick_Mov(float dt) {
        for (int i = 0; i < soaData.occupied.Length; i++) {
            if (!soaData.occupied[i])
                continue;
            //Debug.Log($"UpdateMov: horMov: {horMov} | animRootMot: {animRootMot} \n"
            //    + $"| maxLinSpd: {maxLinSpd} | linAcc: {linAcc}");
            Debug.Assert(
                !float.IsNaN(soaData.vel_Hor[i].x) && !float.IsNaN(soaData.vel_Hor[i].y),
                $"{i} vel_hor had NaN: {soaData.vel_Hor[i]}"
            );
            //Debug.Log($"UpdateMov: data.vel_Hor before calculations: {data.vel_Hor}");
            soaData.vel_Hor[i] = Vector2.MoveTowards(
                soaData.vel_Hor[i],
                soaData.movInput_tgtHorDir[i] * soaData.movInput_tgtHorSpd[i],
                soaData.movInput_horAcc[i] * dt
            );
            // Skip rotation if character is already rotated towards linear movement target direction.
            if (math.lengthsq(soaData.movInput_tgtHorDir[i]) > 0.0001f) {
                soaData.trf_rot[i] = TrfMathUtils.RotateFwdToTgt(
                    soaData.trf_rot[i],
                    soaData.movInput_yawSpd[i],
                    soaData.movInput_tgtHorDir[i]
                );
                unityComps[i].trf.rotation = soaData.trf_rot[i];
            }
            if (soaData.isAffectedByGravity[i])
                // NOTE: This will override previously calculated horizontal velocity if the player is
                // NOTE C: sliding down a slope. (9.9.2026)
                CcMov.ApplyGravityNSlideDownSlopes(i, dt);
            else
                // NOTE: If not using gravitational acceleration, ver velocity is reseted every tick. This
                // NOTE C: way we don't accidentally accumulate velocity when using animation root motion
                // NOTE C: for vertical movement.
                soaData.vel_Ver[i] = 0;
            // NOTE: Additional linear movement is used to apply animation root delta lin movement (9.9.2026)
            Vector3 totalMov = (Vector3)soaData.movInput_additionalLinMov[i]
                + new Vector3(soaData.vel_Hor[i].x, soaData.vel_Ver[i], soaData.vel_Hor[i].y) * dt;
            //Debug.Log($"UpdateMov: totalMov: {totalMov}");
            unityComps[i].cc.Move(totalMov);
            // Save final velocity back to cp data.
            soaData.vel_Hor[i] = new float2(totalMov.x, totalMov.z) / dt;
            soaData.vel_Ver[i] = totalMov.y / dt;
            // NavMeshAgent will drift away from the capsule pawn transform if you don't set it back here.
            unityComps[i].navMeshAgent.nextPosition = unityComps[i].trf.position;
        }
    }

    void Tick_Sensing() {
        for (int i = 0; i < soaData.occupied.Length; i++) {
            // TODO MINOR: Find out if skipping through elements like this affects cpu cache performance.
            if (!soaData.occupied[i])
                continue;
            if (unityComps[i].tgt != null) {
                brainData[i].distToTgt = Vector3.Distance(
                    unityComps[i].trf.position,
                    unityComps[i].tgt.position
                );
                brainData[i].hasTgt = true;
                brainData[i].inAggroRange
                    = Vector3.Distance(
                        unityComps[i].trf.position,
                    unityComps[i].tgt.position) < brainData[i].aggroRange;
                brainData[i].inAtkRange
                    = Vector3.Distance(
                        unityComps[i].trf.position,
                    unityComps[i].tgt.position
                ) < brainData[i].atkRange;
                brainData[i].tgtPos = unityComps[i].tgt.position;
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
        for (int i = 0; i < soaData.occupied.Length; i++) {
            if (!soaData.occupied[i])
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
        for (int i = 0; i < soaData.occupied.Length; i++) {
            if (!soaData.occupied[i])
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
    public int Register(So_CpData so, Cp_UnityComps unityComps, So_BtRootNode bt) {
        int freeI = -1;
        for (int i = 0; i < soaData.occupied.Length; i++) {
            if (!soaData.occupied[i]) {
                freeI = i;
                break;
            }
        }
        if (freeI == -1) {
            Debug.LogError($"Capsule Pawn Entities at capacity ({maxCps})");
            return -1;
        }
        // Brain data
        brainData[freeI].agentDesiredVel = float3.zero;
        brainData[freeI].aggroRange = so.brain_AggroRange;
        brainData[freeI].atkRange = so.brain_AtkRange;
        brainData[freeI].distToTgt = 0;
        brainData[freeI].hasTgt = false;
        brainData[freeI].inAggroRange = false;
        brainData[freeI].inAtkRange = false;
        brainData[freeI].tgtPos = float3.zero;
        // Structure of arrays data
        soaData.actStSt_Impact_YawSpd[freeI] = so.impact_YawSpd;
        soaData.curStDur[freeI] = 0;
        soaData.groundCastHitSomething[freeI] = false;
        soaData.groundCastNrm[freeI] = float3.zero;
        soaData.groundSnapVerDownSpd[freeI] = so.groundSnapVerDownSpd;
        soaData.hp_Cur[freeI] = so.maxHP;
        soaData.hp_Max[freeI] = so.maxHP;
        soaData.input_mov[freeI] = float2.zero;
        soaData.input_mov_LastNonZero[freeI] = float2.zero;
        soaData.input_mov_WhenLastSwitchedSt[freeI] = float2.zero;
        soaData.input_atk_Light[freeI] = false;
        soaData.input_atk_Heavy[freeI] = false;
        soaData.input_atk_Ult[freeI] = false;
        soaData.input_dodge[freeI] = false;
        soaData.invul[freeI] = false;
        soaData.isAffectedByGravity[freeI] = true;
        soaData.isGrounded[freeI] = true;
        soaData.lastCcVel[freeI] = float3.zero;
        soaData.lastKnockbackStr[freeI] = 0;
        soaData.lastRecievedHitDir[freeI] = float3.zero;
        soaData.st_AtkHorSlash_Windup_MaxAngSpd[freeI] = so.st_AtkHorSlash_Windup_YawSpd;
        soaData.st_AtkJump_DownSpeedAfterJumpFinished[freeI] = so.st_AtkJump_DownSpeedAfterJumpFinished;
        soaData.st_Dodge_YawSpd[freeI] = so.st_Dodge_YawAngSpd;
        soaData.st_Falling_LandingStFallDistThreshold[freeI] = so.st_Falling_LandingStFallDistThreshold;
        soaData.st_Falling_HorAcc[freeI] = so.st_Falling_HorAcc;
        soaData.st_Falling_TgtHorSpd[freeI] = so.st_Falling_TgtHorSpd;
        soaData.trf_pos[freeI] = float3.zero;
        soaData.trf_rot[freeI] = quaternion.identity;
        soaData.trf_lossyScl[freeI] = new float3(1);
        soaData.vel_Hor[freeI] = float2.zero;
        soaData.vel_Ver[freeI] = 0;
        soaData.occupied[freeI] = true;
        soaData.walkLinAcc[freeI] = so.walkHorAcc;
        soaData.walkMaxLinSpd[freeI] = so.walkTgtHorSpd;
        soaData.walkYawSpd[freeI] = so.walkYawSpd;
        // Array of structs data.
        this.aosData[freeI] = new();
        aosData[freeI].enableDebugMsgs = so.enableDebugMsgs;
        aosData[freeI].st_AtkFlying_TgtHorSpd = so.st_AtkFlying_TgtHorSpeed;
        this.unityComps[freeI] = unityComps;
        this.classRefs[freeI] = new Cp_NonUnityCompClassRefs(freeI);
        // TODO: Should have a reference to a generic controller which could be player or ai. (6.9.2026)
        if (bt != null)
            BtMgr.inst.Register(freeI, bt);
        //Debug.Log($"Switching {freeI} to initial act st!", this);
        SwitchToInitActSt(freeI);
        return freeI;
    }

    public void Unregister(int cpId) {
        if (!soaData.occupied[cpId]) {
            Debug.LogError($"Capsule pawn with id {cpId} has not been registered!");
            return;
        }
        soaData.occupied[cpId] = false;
    }

    // NOTE: This is currently always enters to idle state. (6.9.2026)
    public void SwitchToInitActSt(int cpId) {
        Debug.Log($"{cpId} switching to init state", this);
        SwitchActSt(() => classRefs[cpId].actSts.idle.Enter(), cpId);
        //Debug.Log($"{id} state initialized to : {initSt}", this);
    }

    public void SwitchActSt(Func<IFsmSt_Cp> enterFunc, int cpId){
        Fsm.SwitchSt(
            enterFunc,
            ref classRefs[cpId].st_cur,
            ref classRefs[cpId].st_prev,
            ref aosData[cpId].isSwitchingSt
            //aosData[cpId].enableDebugMsgs
        );
    }

    public bool TrySwitchActSt(Func<IFsmSt_Cp> enterFunc, int cpId) {
        return Fsm.TrySwitchState(
            enterFunc,
            ref classRefs[cpId].st_cur,
            ref classRefs[cpId].st_prev,
            ref aosData[cpId].isSwitchingSt
            //aosData[cpId].enableDebugMsgs
        );
    }

    // TODO: Create per cp action for "state switched", then pass that to SwitchSt and subscribe this to it. Or. Idk. Could just invoke this directly with the SwitchSt function? Maybe like the Enter state methods?
    public void OnStateSwitched(int cpId, IFsmSt newSt) {
        soaData.curStDur[cpId] = 0;
        if (unityComps[cpId].cpCtrl == null)
            soaData.input_mov_WhenLastSwitchedSt[cpId]
                = soaData.input_mov[cpId];
        else {
            if (unityComps[cpId].cpCtrl.Input_Mov.sqrMagnitude > PlrConfigsManager.inst.movInputSqrDeadzone)
                soaData.input_mov_WhenLastSwitchedSt[cpId]
                    = soaData.input_mov[cpId];
            else
                soaData.input_mov_WhenLastSwitchedSt[cpId] = float2.zero;
        }
    }
}