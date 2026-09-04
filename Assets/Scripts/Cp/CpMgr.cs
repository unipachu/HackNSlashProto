using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Capsule pawn (i.e. player or ai controlled character that uses capsule collision for movement) manager.
/// </summary>
public class CpMgr : Singleton<CpMgr> {
    [Header("Settings")]
    public int maxCps = 1;
    // TODO: So for these?
    public float movInputSqrDeadzone = 0.2f;
    public float inputBuffer_Dur = 0.3f;

    [HideInInspector] public Cp_BaseData data;
    [HideInInspector] public Cp_BrainData brainData;
    // All game object components the capsule pawns use.
    [HideInInspector] public Cp_UnityComps[] unityComps;
    [HideInInspector] public AnimEventPlrData[] animEventPlrData;

    public void Init() {
        data = Cp_BaseData.Create(maxCps);
        brainData = Cp_BrainData.Create(maxCps);
        unityComps = new Cp_UnityComps[maxCps];
        animEventPlrData = new AnimEventPlrData[maxCps];
    }

    void OnDestroy() {
        data.Dispose();
        brainData.Dispose();
    }

    // ------------------------------------------------------------
    // Fixed Tick Methods
    // ------------------------------------------------------------

    public void FixedTick() {
        UpdateGroundCheck(
            data.groundCastHitSomething,
            data.groundCastNrm,
            data.isGrounded,
            data.occupied,
            unityComps
        );
        FixedTick_Fsm();
    }

    void FixedTick_Fsm() {
        for (int i = 0; i < unityComps.Length; i++) {
            if (!data.occupied[i])
                continue;
            switch (data.actSt[i]) {
                default:
                    //Debug.LogError($"Switch defaulted with {data.actSt[i]}.", this);
                    break;
            }
        }
    }

    static void UpdateGroundCheck(
        NativeArray<bool> groundCastHitSomething,
        NativeArray<float3> groundCastNrm,
        NativeArray<bool> isGrounded,
        NativeArray<bool> occupied,
        Cp_UnityComps[] unityComp
    ) {
        for (int i = 0; i < unityComp.Length; i++) {
            if (!occupied[i])
                continue;
            // TODO MINOR: Maybe there's a way not not use a local variable?
            bool hitSomething = false;
            isGrounded[i] = CcMov.IsGrounded(
                unityComp[i].cc,
                out hitSomething,
                out RaycastHit groundCastResult
            );
            groundCastHitSomething[i] = hitSomething;
            groundCastNrm[i] = groundCastResult.normal;
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
        Tick_Mov();
    }

    // TODO: Update in Tick_FromNonNative
    // TODO C: Or maybe in Tick_Sensing.
    void Tick_AgentMovInput() {
        for (int i = 0; i < data.occupied.Length; i++) {
            //Dbg.Log($"{i} tgt: {unityComps[i].tgt}", data.enableDebugMsgs[i]);
            if (!data.occupied[i] || unityComps[i].navMeshAgent == null)
                continue;
            if (unityComps[i].tgt == null) {
                //Dbg.Log(
                //    $"{i} Set agent desired vel to 0 because tgt was null: {unityComps[i].tgt}",
                //    data.enableDebugMsgs[i]
                //);
                unityComps[i].navMeshAgent.ResetPath();
                brainData.agentDesiredVel[i] = float3.zero;
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
                brainData.agentDesiredVel[i] = float3.zero;
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
                brainData.agentDesiredVel[i] = float3.zero;
                continue;
            }
            // NOTE: We only use the current unfinished path if last path calculation was completed. This way if we
            // NOTE C: get sequential failed path finding attempts, the character will not move at all (instead of
            // NOTE C: jittering a little because of the partial paths).
            if (unityComps[i].navMeshAgent.pathPending) {
                //Dbg.Log($"{i} Path was pending.", data.enableDebugMsgs[i]);
                if (brainData.prevCalculatePathSucceeded[i])
                    brainData.agentDesiredVel[i] = unityComps[i].navMeshAgent.desiredVelocity;
                else
                    brainData.agentDesiredVel[i] = float3.zero;
                continue;
            }
            if (unityComps[i].navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete) {
                brainData.prevCalculatePathSucceeded[i] = true;
                //Debug.Log($"Entity id: {i}");
                //Debug.Log($"Tgt on nav mesh: {tgtOnNavMesh}");
                //Debug.Log($"prevCalculatePathSucceeded: {brainData.prevCalculatePathSucceeded[i]}");
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
                brainData.agentDesiredVel[i] = unityComps[i].navMeshAgent.desiredVelocity;
            }
            else {
                //Dbg.Log($"{i} Did not find path. Setting desired vel to 0.", data.enableDebugMsgs[i]);
                brainData.prevCalculatePathSucceeded[i] = false;
                brainData.agentDesiredVel[i] = float3.zero;
            }
            unityComps[i].navMeshAgent.SetDestination(unityComps[i].tgt.position);
        }
    }

    /// <summary>
    /// Update data from non native sources, e.g. from Monobehavior components.
    /// </summary>
    void Tick_FromNonNative(float dt) {
        for (int i = 0; i < unityComps.Length; i++) {
            if (!data.occupied[i])
                continue;
            data.trf_pos[i] = unityComps[i].trf.position;
            data.trf_rot[i] = unityComps[i].trf.rotation;
            data.trf_lossyScl[i] = unityComps[i].trf.lossyScale;
            data.lastCcVel[i] = unityComps[i].cc.velocity;
            data.curStDur[i] += dt;
        }
    }

    void Tick_Fsm() {
        for (int i = 0; i < unityComps.Length; i++) {
            if (!data.occupied[i])
                continue;
            switch (data.actSt[i]) {
                case CpActSt.Atk_FlyingAtk:
                    CpSt_Atk_FlyingAtk.Tick(i, data, unityComps, ref animEventPlrData[i]);
                    break;
                case CpActSt.Atk_ShootHomingProj:
                    CpSt_Atk_ShootHomingProj.Tick(i, data, unityComps);
                    break;
                case CpActSt.Atk_HorSlash1:
                    CpSt_Atk_HorSlash1.Tick(i, data, unityComps);
                    break;
                case CpActSt.Atk_HorSlash2:
                    CpSt_Atk_HorSlash2.Tick(i, data, unityComps);
                    break;
                case CpActSt.Atk_HorSlash3:
                    CpSt_Atk_HorSlash3.Tick(i, data, unityComps);
                    break;
                case CpActSt.Atk_Jump:
                    CpSt_Atk_Jump.Tick(i, data, unityComps);
                    break;
                case CpActSt.Dodge:
                    CpSt_Dodge.Tick(i, data, unityComps);
                    break;
                case CpActSt.Falling:
                    CpSt_Falling.Tick(i, data, unityComps);
                    break;
                case CpActSt.FallLanding:
                    CpSt_FallLanding.Tick(i, data, unityComps);
                    break;
                case CpActSt.Idle:
                    CpSt_Idle.Tick(i, data, unityComps);
                    break;
                case CpActSt.Knockback_Weak:
                    CpSt_Knockback_Weak.Tick(i, data, unityComps);
                    break;
                case CpActSt.Walk:
                    CpSt_Walk.Tick(i, data, unityComps);
                    break;
                default:
                    Debug.LogError($"Switch defaulted with {data.actSt[i]}", this);
                    break;
            }
        }
    }

    void Tick_Input() {
        for (int i = 0; i < unityComps.Length; i++) {
            if (!data.occupied[i] || unityComps[i].cpCtrl == null)
                continue;
            data.input_atk_Light[i] = unityComps[i].cpCtrl.TryConsume_Atk_Light();
            data.input_atk_Heavy[i] = unityComps[i].cpCtrl.TryConsume_Atk_Heavy();
            data.input_atk_Ult[i] = unityComps[i].cpCtrl.TryConsume_Atk_Ult();
            data.input_dodge[i] = unityComps[i].cpCtrl.TryConsume_Dodge();
            if (unityComps[i].cpCtrl.Input_Mov.sqrMagnitude > movInputSqrDeadzone) {
                data.input_mov[i] = unityComps[i].cpCtrl.Input_Mov;
                data.input_mov_LastNonZero[i] = data.input_mov[i];
            }
            else {
                data.input_mov[i] = Vector2.zero;
            }
            //Debug.Log($"{i} mov input mag: {math.length(data.input_mov[i])}.");
        }
    }

    void Tick_InputBuffer(float dt) {
        for (int i = 0; i < unityComps.Length; i++) {
            if (!data.occupied[i])
                continue;
            if (data.input_atk_Light[i])
                CpInputBuffer.BufferInput(
                    i,
                    BufferableInput.Atk_Light,
                    data.inputBuffer_BufferedInput,
                    data.inputBuffer_RemainingTime,
                    inputBuffer_Dur
                );
            else if (data.input_atk_Heavy[i])
                CpInputBuffer.BufferInput(
                    i,
                    BufferableInput.Atk_Heavy,
                    data.inputBuffer_BufferedInput,
                    data.inputBuffer_RemainingTime,
                    inputBuffer_Dur
                );
            else if (data.input_atk_Ult[i])
                CpInputBuffer.BufferInput(
                    i,
                    BufferableInput.Atk_Ult,
                    data.inputBuffer_BufferedInput,
                    data.inputBuffer_RemainingTime,
                    inputBuffer_Dur
                );
            else if (data.input_dodge[i])
                CpInputBuffer.BufferInput(
                    i,
                    BufferableInput.Dodge,
                    data.inputBuffer_BufferedInput,
                    data.inputBuffer_RemainingTime, 
                    inputBuffer_Dur
                );
            // Clear input if buffer time passed.
            if (data.inputBuffer_RemainingTime[i] <= 0)
                continue;
            data.inputBuffer_RemainingTime[i] -= dt;
            //Debug.Log("remaining time: " + remainingTime);
            if (data.inputBuffer_RemainingTime[i] <= 0)
                CpInputBuffer.Clear(i, data.inputBuffer_BufferedInput, data.inputBuffer_RemainingTime);
        }
    }

    void Tick_Mov() {
        for (int i = 0; i < data.occupied.Length; i++) {
            if (!data.occupied[i])
                continue;
            //Debug.Log($"UpdateMov: horMov: {horMov} | animRootMot: {animRootMot} \n"
            //    + $"| maxLinSpd: {maxLinSpd} | linAcc: {linAcc}");
            float dt = Time.deltaTime;
            //Debug.Assert(
            //    !float.IsNaN(vel_Hor[i].x)
            //      && !float.IsNaN(vel_Hor[i].y),
            //    $"vel_hor had NaN: {vel_Hor[i]}"
            //);
            //Debug.Log($"UpdateMov: data.vel_Hor before calculations: {data.vel_Hor}");
            data.vel_Hor[i] = Vector2.MoveTowards(
                data.vel_Hor[i],
                data.mov_horMov[i] * data.mov_maxLinSpd[i],
                data.mov_linAcc[i] * dt
            );
            data.vel_Yaw[i] = data.mov_yawSpd[i];
            // Skip rotation if tgt dir vector (horMov) is too small.
            if (math.lengthsq(data.mov_horMov[i]) > 0.0001f) {
                data.trf_rot[i] = TrfMathUtils.RotateFwdToTgt(data.trf_rot[i], data.vel_Yaw[i], data.mov_horMov[i]);
                // TODO: You could make a separate function that sets this later after all calculations
                // TODO C: have finished. Though should each pawn be moved one at a time? Maybe. But
                // TODO C: wait, they are! Is that ok or is some other logic tied to how the pawn
                // TODO C: should move that should be done one pawn at a time?
                unityComps[i].trf.rotation = data.trf_rot[i];
            }
            // TODO: This should be its own Tick function I think. Then you didn't need to worry about ref
            // TODO C: keywords or such. Over multiple Tick_Mov_ you accumulate impulses and forces and
            // TODO C: then apply them all with a separate method to the pawn controller!
            if (data.isAffectedByGravity[i])
                CcMov.ApplyGravityNSlideDownSlopes(i, dt);
            else
                data.vel_Ver[i] = 0;
            Vector3 totalMov = data.animDPos[i];
            totalMov.x += data.vel_Hor[i].x * dt;
            totalMov.y += data.vel_Ver[i] * dt;
            totalMov.z += data.vel_Hor[i].y * dt;
            //Debug.Log($"UpdateMov: totalMov: {totalMov}");
            unityComps[i].cc.Move(totalMov);
            // NavMeshAgent will drift away from the capsule pawn transform if you don't set it back here.
            unityComps[i].navMeshAgent.nextPosition = unityComps[i].trf.position;
        }
    }

    void Tick_Sensing() {
        for (int i = 0; i < data.occupied.Length; i++) {
            // TODO MINOR: Find out if skipping through elements like this affects cpu cache performance.
            if (!data.occupied[i])
                continue;
            if (unityComps[i].tgt != null) {
                brainData.distToTgt[i] = Vector3.Distance(
                    unityComps[i].trf.position,
                    unityComps[i].tgt.position
                );
                brainData.hasTgt[i] = true;
                brainData.inAggroRange[i]
                    = Vector3.Distance(
                        unityComps[i].trf.position,
                    unityComps[i].tgt.position) < brainData.aggroRange[i];
                brainData.inAtkRange[i]
                    = Vector3.Distance(
                        unityComps[i].trf.position,
                    unityComps[i].tgt.position
                ) < brainData.atkRange[i];
                brainData.tgtPos[i] = unityComps[i].tgt.position;
            }
            else
                brainData.hasTgt[i] = false;
        }
    }

    // ------------------------------------------------------------
    // Late Tick Methods
    // ------------------------------------------------------------

    // TODO: Remember to call this from game manager.
    public void LateTick() {
        LateTick_AnimEventPlr();
    }

    void LateTick_AnimEventPlr() {
        for (int i = 0; i < data.occupied.Length; i++) {
            if (!data.occupied[i])
                continue;
            //Debug.Log($"{animEventPlrData[i]}");
            //Debug.Log($"{unityComps[i].anim == null}");
            //Debug.Log($"{unityComps[i].animEvents == null}");
            AnimEventPlr.Tick(i, ref animEventPlrData[i], unityComps[i].anim, unityComps[i].animEventHandler.animEvent);
        }
    }

    // ------------------------------------------------------------
    // Other Methods
    // ------------------------------------------------------------

    /// <summary>
    /// Returns the index of the registered data, or -1 on failure.
    /// </summary>
    public int Register(So_CpData so, Cp_UnityComps unityComps, So_BtRootNode bt) {
        int freeI = -1;
        for (int i = 0; i < data.occupied.Length; i++) {
            if (!data.occupied[i]) {
                freeI = i;
                break;
            }
        }
        if (freeI == -1) {
            Debug.LogError($"Capsule Pawn Entities at capacity ({maxCps})");
            return -1;
        }
        brainData.agentDesiredVel[freeI] = float3.zero;
        brainData.aggroRange[freeI] = so.brain_AggroRange;
        brainData.atkRange[freeI] = so.brain_AtkRange;
        brainData.distToTgt[freeI] = 0;
        brainData.hasTgt[freeI] = false;
        brainData.inAggroRange[freeI] = false;
        brainData.inAtkRange[freeI] = false;
        brainData.tgtPos[freeI] = float3.zero;
        data.curStDur[freeI] = 0;
        data.enableDebugMsgs[freeI] = so.enableDebugMsgs;
        // TODO: Item
        //data.equip_RHandEquippable[freeI] = so.rHandItem;
        data.gravitationalAcc[freeI] = so.gravitationalAcc;
        data.groundCastHitSomething[freeI] = false;
        data.groundCastNrm[freeI] = float3.zero;
        data.groundSnapVerDownSpd[freeI] = so.groundSnapVerDownSpd;
        data.hp_Cur[freeI] = so.maxHP;
        data.hp_Max[freeI] = so.maxHP;
        data.input_mov[freeI] = float2.zero;
        data.input_mov_LastNonZero[freeI] = float2.zero;
        data.input_mov_WhenLastSwitchedSt[freeI] = float2.zero;
        data.input_atk_Light[freeI] = false;
        data.input_atk_Heavy[freeI] = false;
        data.input_atk_Ult[freeI] = false;
        data.input_dodge[freeI] = false;
        data.invul[freeI] = false;
        data.isAffectedByGravity[freeI] = true;
        data.isGrounded[freeI] = true;
        data.lastCcVel[freeI] = float3.zero;
        data.lastKnockbackStr[freeI] = 0;
        data.lastRecievedHitDir[freeI] = float3.zero;
        data.maxFallSpd[freeI] = so.maxFallSpd;
        data.st_AtkHorSlash_Impact_AngSpd[freeI] = so.st_AtkHorSlash_Impact_AngSpd;
        data.st_AtkHorSlash_Windup_MaxAngSpd[freeI] = so.st_AtkHorSlash_Windup_MaxAngSpd;
        data.st_AtkJump_DownSpeedAfterJumpFinished[freeI] = so.st_AtkJump_DownSpeedAfterJumpFinished;
        data.st_Dodge_YawSpd[freeI] = so.st_Dodge_YawAngSpd;
        data.st_Falling_LandingStFallDistThreshold[freeI] = so.st_Falling_LandingStFallDistThreshold;
        data.st_Falling_LinAcc[freeI] = so.st_Falling_LinAcc;
        data.st_Falling_MaxLinSpd[freeI] = so.st_Falling_MaxLinSpd;
        data.st_Walk_LinAcc[freeI] = so.st_Walk_LinAcc;
        data.st_Walk_MaxLinSpd[freeI] = so.st_Walk_MaxLinSpd;
        data.st_Walk_YawSpd[freeI] = so.st_Walk_MaxAngSpd;
        data.trf_pos[freeI] = float3.zero;
        data.trf_rot[freeI] = quaternion.identity;
        data.trf_lossyScl[freeI] = new float3(1);
        data.vel_Hor[freeI] = float2.zero;
        data.vel_Ver[freeI] = 0;
        data.vel_Yaw[freeI] = 0;
        data.occupied[freeI] = true;
        this.unityComps[freeI] = unityComps; 
        if (bt != null)
            BtMgr.inst.Register(freeI, bt);
        //Debug.Log($"Switching {freeI} to initial act st!", this);
        ActSt_SwitchToInitSt(freeI, so.initSt);
        return freeI;
    }

    public void Unregister(int id) {
        if (!data.occupied[id]) {
            Debug.LogError($"Capsule pawn with id {id} has not been registered!");
            return;
        }
        data.occupied[id] = false;
    }

    public bool ActSt_CanSwitchTo(CpActSt newActSt) {
        switch (newActSt) {
            case CpActSt.Atk_FlyingAtk:
                return newActSt == CpActSt.Falling ? false : true;
            case CpActSt.Atk_HorSlash1:
                return true;
            case CpActSt.Atk_HorSlash2:
                return true;
            case CpActSt.Atk_HorSlash3:
                return true;
            case CpActSt.Atk_Jump:
                if (newActSt == CpActSt.Falling)
                    return false;
                else
                    return true;
            case CpActSt.Atk_ShootHomingProj:
                return true;
            case CpActSt.Dodge:
                return true;
            case CpActSt.Falling:
                return true;
            case CpActSt.FallLanding:
                return true;
            case CpActSt.Idle:
                return true;
            case CpActSt.Knockback_Weak:
                // TODO: To avoid stun locking, after some amount of consequtive knockbacks,
                // TODO C: allow canceling knockback state.
                if (newActSt == CpActSt.Knockback_Weak)
                    return true;
                // TODO: Allow switch to death state.
                return false;
            case CpActSt.Walk:
                return true;
            default:
                Debug.LogError($"Switch defaulted with {newActSt}", this);
                return false;
        }
    }

    public void ActSt_EnterSt(int id, CpActSt newSt, CpActSt prevSt) {
        //Debug.Log($"{id} EnterSt called! Prev st: {prevSt}. New st: {newSt}.", this);
        switch (newSt) {
            case CpActSt.Atk_FlyingAtk:
                CpSt_Atk_FlyingAtk.Enter(id, data, unityComps, ref animEventPlrData[id]);
                break;
            case CpActSt.Atk_ShootHomingProj:
                CpSt_Atk_ShootHomingProj.Enter(id, data, unityComps, ref animEventPlrData[id]);
                break;
            case CpActSt.Atk_HorSlash1:
                CpSt_Atk_HorSlash1.Enter(id, data, unityComps, ref animEventPlrData[id]);
                break;
            case CpActSt.Atk_HorSlash2:
                CpSt_Atk_HorSlash2.Enter(id, data, unityComps, ref animEventPlrData[id]);
                break;
            case CpActSt.Atk_HorSlash3:
                CpSt_Atk_HorSlash3.Enter(id, data, unityComps, ref animEventPlrData[id]);
                break;
            case CpActSt.Atk_Jump:
                CpSt_Atk_Jump.Enter(id, data, unityComps, ref animEventPlrData[id]);
                break;
            case CpActSt.Dodge:
                CpSt_Dodge.Enter(id, data, unityComps, ref animEventPlrData[id]);
                break;
            case CpActSt.Falling:
                CpSt_Falling.Enter(id, data, unityComps, ref animEventPlrData[id]);
                break;
            case CpActSt.FallLanding:
                CpSt_FallLanding.Enter(id, data, unityComps, ref animEventPlrData[id]);
                break;
            case CpActSt.Idle:
                CpSt_Idle.Enter(id, data, unityComps, ref animEventPlrData[id]);
                break;
            case CpActSt.Knockback_Weak:
                CpSt_Knockback_Weak.Enter(id, data, unityComps, ref animEventPlrData[id]);
                break;
            case CpActSt.Walk:
                CpSt_Walk.Enter(id, data, unityComps, ref animEventPlrData[id]);
                break;
            default:
                Debug.LogError($"Switch defaulted with {newSt}", this);
                break;
        }
    }

    public void ActSt_ExitSt(int id, CpActSt actSt) {
        //Debug.Log($"{id} ExitSt called for: {actSt}.", this);
        switch (actSt) {
            case CpActSt.Atk_FlyingAtk:
                CpSt_Atk_FlyingAtk.Exit(id, data, unityComps);
                break;
            case CpActSt.Atk_ShootHomingProj:
                break;
            case CpActSt.Atk_HorSlash1:
                CpSt_Atk_HorSlash1.Exit(id, unityComps);
                break;
            case CpActSt.Atk_HorSlash2:
                CpSt_Atk_HorSlash2.Exit(id, unityComps);
                break;
            case CpActSt.Atk_HorSlash3:
                CpSt_Atk_HorSlash3.Exit(id, unityComps);
                break;
            case CpActSt.Atk_Jump:
                CpSt_Atk_Jump.Exit(id, data, unityComps);
                break;
            case CpActSt.Dodge:
                CpSt_Dodge.Exit(id, data);
                break;
            case CpActSt.Falling:
                break;
            case CpActSt.FallLanding:
                break;
            case CpActSt.Idle:
                break;
            case CpActSt.Knockback_Weak:
                break;
            case CpActSt.Walk:
                break;
            default:
                Debug.LogError($"Switch defaulted with {actSt}", this);
                break;
        }
    }

    public void ActSt_SwitchToInitSt(int id, CpActSt initSt) {
        Debug.Assert(!data.isSwitchingActSt[id], $"Tried changing to {initSt}, but {id} was already changing"
            + $"state!", this);
        data.isSwitchingActSt[id] = true;
        //Debug.Log($"{id} switching to init state: {initSt}", this);
        data.actSt[id] = initSt;
        ActSt_EnterSt(id, initSt, data.prevSt[id]);
        data.curStDur[id] = 0;
        if (unityComps[id].cpCtrl == null)
            data.input_mov_WhenLastSwitchedSt[id]
                = data.input_mov[id];
        else {
            if (unityComps[id].cpCtrl.Input_Mov.sqrMagnitude > movInputSqrDeadzone)
                data.input_mov_WhenLastSwitchedSt[id]
                    = data.input_mov[id];
            else
                data.input_mov_WhenLastSwitchedSt[id] = float2.zero;
        }
        data.isSwitchingActSt[id] = false;
        //Debug.Log($"{id} state initialized to : {initSt}", this);
    }

    // TODO MINOR: Rename to St"
    public void ActSt_SwitchState(int id, CpActSt newSt) {
        Debug.Assert(!data.isSwitchingActSt[id], $"Tried changing to {newSt}, but {id} was already changing"
            + $"state!", this);
        data.isSwitchingActSt[id] = true;
        //Dbg.Log($"{id} switching state from {data.actSt[id]} to: {newSt}", this, data.enableDebugMsgs[id]);
        data.prevSt[id] = data.actSt[id];
        data.actSt[id] = newSt;
        ActSt_ExitSt(id, data.prevSt[id]);
        ActSt_EnterSt(id, newSt, data.prevSt[id]);
        data.curStDur[id] = 0;
        if (unityComps[id].cpCtrl == null)
            data.input_mov_WhenLastSwitchedSt[id]
                = data.input_mov[id];
        else {
            if (unityComps[id].cpCtrl.Input_Mov.sqrMagnitude > movInputSqrDeadzone)
                data.input_mov_WhenLastSwitchedSt[id]
                    = data.input_mov[id];
            else
                data.input_mov_WhenLastSwitchedSt[id] = float2.zero;
        }
        data.isSwitchingActSt[id] = false;
    }
}