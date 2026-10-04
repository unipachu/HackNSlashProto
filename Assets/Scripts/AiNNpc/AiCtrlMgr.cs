using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class AiCtrlMgr : Singleton<AiCtrlMgr>{
    [Header("Settings")]
    [Tooltip("Initial capacity of entities")]
    public int initCapacity = 1;

    [Header("Custom Avoidance Movement Settings")]
    [Tooltip("Custom avoidance acts weird when a lot of ai npcs turn a corner so don't use this.")]
    public bool useCustomAvoidance = false;
    [Tooltip("Minimum seconds between path recalculations while following a moving target. "
        + "Also the retry interval after a failed path. Lower = more accurate tracking but more "
        + "pathfinding cost.")]
    public float repathInterval = 0.25f;
    [Tooltip("How far (in units) the follow target must move away from the agent's current "
        + "destination before a new path is requested. Higher = fewer repaths, but the path "
        + "lags behind a moving target.")]
    public float repathTgtMoveDist = 0.5f;
    [Tooltip("Other characters closer than this (in units) push this character away. The push "
        + "is strongest at distance 0 and fades linearly to zero at this radius. Larger values "
        + "make crowds spread out earlier but can interfere with path following.")]
    public float separationRadius = 4;
    [Tooltip("Strength of the separation push relative to the path direction. The path "
        + "direction has length 1 and the separation push is capped at length 1 before this "
        + "weight is applied. Higher = characters avoid each other more, lower = they follow "
        + "the path more strictly.")]
    public float separationWeight = 1.5f;
    [Tooltip("Maximum speed (radians per second) at which the desired movement direction may "
        + "rotate. Smooths sudden direction changes. 0 = off (direction changes instantly).")]
    public float maxTurnRate = 0;

    [HideInInspector] public AiCtrlData[] aos;

    /// <summary>
    /// Scratch buffer for NavMeshPath.GetCornersNonAlloc. Paths with more than 32 corners are truncated
    /// to the first 32.
    /// </summary>
    readonly Vector3[] cornerBuf = new Vector3[32];
    int entityCount;

    public void Init() {
        aos = new AiCtrlData[initCapacity];
        entityCount = 0;
    }

    // ----------------------------------------------------------------------------------------
    // Register and Unregister
    // ----------------------------------------------------------------------------------------

    /// <summary>
    /// Registers an AI controller and creates its per-entity runtime data.
    /// </summary>
    public void Register(
        float aggroRange,
        float atkRange,
        AiCtrlHandle newHandle,
        IBtNode newBt,
        CpHandle controlledCp
    ) {
        Debug.Assert(newHandle != null);
        int newI = entityCount;
        AiCtrlData newData = new AiCtrlData(
            aggroRange,
            atkRange,
            newBt,
            controlledCp,
            newHandle
        );
        ArrayUtils.Add(ref aos, newI, newData);
        controlledCp.Data.action_markedForPendingUnregister += newHandle.OnCpMarkedForUnregister;
        newHandle.I = newI;
        // NOTE: This is here because we use custom avoidance. If you want to use Unity's
        // NOTE C: avoidance, comment this out.
        controlledCp.navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        entityCount++;
    }

    /// <summary>
    /// Unregisters an AI controller, removes its runtime data and destroys
    /// the controller GameObject.<br/>
    /// NOTE: Since this is currently only called when cp is marked for unregisteration, we can safely
    /// unregister this without separate pendingUnregisteration field. (22.9.2026)
    /// </summary>
    public void Unregister(int i) {
        if (i < 0 || i >= entityCount) {
            Debug.LogError(
                $"Invalid AiCtrl i {i}. Ctrl count was {entityCount}."
            );
            return;
        }
        GetData(i).cp.Data.action_markedForPendingUnregister -= GetData(i).handle.OnCpMarkedForUnregister;
        int lastId = entityCount - 1;
        AiCtrlHandle swappedCtrl = i != lastId
            ? aos[lastId].handle
            : null;
        ArrayUtils.RemoveAtSwapBack(aos, entityCount, i);
        entityCount--;
        if (swappedCtrl != null)
            swappedCtrl.I = i;
        //Debug.Log($"Unregistered {typeof(AiCtrlHandle)} i: {i}.");
    }

    // ----------------------------------------------------------------------------------------
    // Tick Methods
    // ----------------------------------------------------------------------------------------

    public void Tick(float dt) {
        Tick_ResetWasPressedThisFrameInputs();
        // Note: here you could set a ai tick budget e.g. only tick half of the ai controllers during one frame.
        Tick_BehaviorTrees();
        if(useCustomAvoidance)
            Tick_AgentMovInput_CustomAvoidance_MaybeBetterCorners(dt);
        else
            Tick_AgentMovInput();
    }

    // ----------------------------------------------------------------------------------------
    // Tick Methods
    // ----------------------------------------------------------------------------------------

    /// <summary>
    /// Updates nav mesh agents' pathfinding and assigns <see cref="AiCtrlData.agentDesiredVel"/>
    /// for each AiCtrl.
    /// </summary>
    // TODO MINOR: Use repathInterval or some pathfinding budget to avoid pathfinding every frame.
    // C: Also apparently agents can use unfinished paths which is something I probably don't want,
    // C: So make sure only fully calculated paths are used.
    void Tick_AgentMovInput() {
        for (int i = 0; i < entityCount; i++) {
            CpHandle cp = aos[i].cp;
            //var cpClassRefs = CpMgr.inst.aos[cpI].classRefs;
            //Debug.Log("cpI " + cpI);
            if (aos[i].followTgt == null) {
                //Dbg.Log(
                //    $"{cpI} Set agent desired vel to 0 because tgt was null: {cpClassRefs.lockedOnTgt}",
                //    CpMgr.GetAos(cpI).enableDbgMsgs
                //);
                cp.navMeshAgent.ResetPath();
                aos[i].agentDesiredVel = float3.zero;
                continue;
            }
            // NOTE: nav mesh agent can drift away from the actual transform because nav mesh agents suck.
            cp.navMeshAgent.nextPosition = aos[i].cp.transform.position;
            // NOTE: We need to check this manually since SetDestination does not have option to set
            // NOTE C: target sample position max distance.
            if (!aos[i].followTgt.IsOnNavMesh()) {
                //Dbg.Log(
                //    $"{cpI} Set agent desired vel to 0 since tgt was not on navmesh.",
                //    CpMgr.GetAos(cpI).enableDbgMsgs
                //);
                cp.navMeshAgent.ResetPath();
                aos[i].agentDesiredVel = float3.zero;
                continue;
            }
            // NOTE: The point of this is to START path finding calculation if there is no previous path
            // NOTE C: calculation (e.g. no path status) and if the agent is not currenly calculating a path.
            // NOTE C: I think this might be incorrect way to do it (maybe) but the agent navigation seems
            // NOTE C: to work well enough for now.
            if (!cp.navMeshAgent.hasPath) {
                //Dbg.Log($"{cpI} Agent had no path. Set destination.", CpMgr.GetAos(cpI).enableDbgMsgs);
                cp.navMeshAgent.SetDestination(aos[i].followTgt.TrfToFollow.position);
                continue;
            }
            // If we are close enough to the destination, stop desiring movement.
            if (
                Vector3.SqrMagnitude(
                    cp.navMeshAgent.destination - aos[i].cp.transform.position
                ) < 0.1f // NOTE: Stopping distane is hard coded.
            ) {
                //Dbg.Log(
                //    $"{cpI} Set agent desired vel to 0 since we reached the target vicinity.",
                //    CpMgr.GetAos(cpI).enableDbgMsgs
                //);
                cp.navMeshAgent.ResetPath();
                aos[i].agentDesiredVel = float3.zero;
                continue;
            }
            // NOTE: We only use the current unfinished path if last path calculation was completed. This way
            // NOTE C: if we get sequential failed path finding attempts, the character will not move at all
            // NOTE C: (instead of jittering a little because of the partial paths).
            if (cp.navMeshAgent.pathPending) {
                //Dbg.Log($"{cpI} Path was pending.", CpMgr.GetAos(cpI).enableDbgMsgs);
                if (aos[i].prevCalculatePathSucceeded)
                    aos[i].agentDesiredVel = cp.navMeshAgent.desiredVelocity;
                else
                    aos[i].agentDesiredVel = float3.zero;
                continue;
            }
            if (cp.navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete) {
                aos[i].prevCalculatePathSucceeded = true;
                //Debug.Log($"Entity i: {cpI}");
                //Debug.Log($"prevCalculatePathSucceeded: {aos[i].prevCalculatePathSucceeded}");
                //Debug.Log($"pending: {cpUnityComps.navMeshAgent.pathPending}");
                //Debug.Log($"status: {cpUnityComps.navMeshAgent.pathStatus}");
                //Debug.Log($"has path: {cpUnityComps.navMeshAgent.hasPath}");
                //Debug.Log($"tgt: {cpClassRefs.lockedOnTgt.Trf.position}");
                //Debug.Log($"destination: {cpUnityComps.navMeshAgent.destination}");
                //Debug.Log($"path end: {cpUnityComps.navMeshAgent.pathEndPosition}");
                //Debug.Log($"desired vel: {cpUnityComps.navMeshAgent.desiredVelocity}");
                //Debug.Log($"steering tgt: {cpUnityComps.navMeshAgent.steeringTarget}");
                // NOTE: We use desired velocity instead of steering target, because steering target doesn't
                // NOTE C: use avoidance.
                aos[i].agentDesiredVel = cp.navMeshAgent.desiredVelocity;
            }
            else {
                //Dbg.Log($"{i} Did not find path. Setting desired vel to 0.", data.enableDebugMsgs[i]);
                aos[i].prevCalculatePathSucceeded = false;
                aos[i].agentDesiredVel = float3.zero;
            }
            cp.navMeshAgent.SetDestination(aos[i].followTgt.TrfToFollow.position);
        }
    }

    /// <summary>
    /// Tried generating custom avoidance with claude since nav mesh agent + character controller movement do
    /// not work well with NavMeshAgent avoidance. This custom avoidance also had some problems especially
    /// when an enemy behind other enemies turns a corner which causes the enemy's rotation to oscillate. I
    /// analysed Hades and a video about Dark Souls' pathfinding and figured out that they probably use no
    /// avoidance at all so this is not used.
    /// </summary>
    void Tick_AgentMovInput_CustomAvoidance_MaybeBetterCorners(float dt) {
        float repathTgtMoveDistSq = repathTgtMoveDist * repathTgtMoveDist;
        for (int i = 0; i < entityCount; i++) {
            CpHandle cp = aos[i].cp;
            Debug.Assert(
                cp.navMeshAgent.obstacleAvoidanceType == ObstacleAvoidanceType.NoObstacleAvoidance,
                "Cp nav mesh agent uses custom avoidance so nav mesh agent avoidance should be turned off.",
                cp
            );
            NavMeshAgent agent = cp.navMeshAgent;
            Vector3 pos = cp.transform.position;
            aos[i].repathTimer -= dt;
            if (aos[i].followTgt == null) {
                agent.ResetPath();
                aos[i].agentDesiredVel = float3.zero;
                continue;
            }
            agent.nextPosition = pos;
            if (!aos[i].followTgt.IsOnNavMesh()) {
                agent.ResetPath();
                aos[i].agentDesiredVel = float3.zero;
                continue;
            }
            Vector3 tgtPos = aos[i].followTgt.TrfToFollow.position;
            if (!agent.hasPath) {
                if (!agent.pathPending) {
                    agent.SetDestination(tgtPos);
                    aos[i].repathTimer = repathInterval;
                }
                continue;
            }
            if (Vector3.SqrMagnitude(tgtPos - pos) < 0.1f) {
                agent.ResetPath();
                aos[i].agentDesiredVel = float3.zero;
                continue;
            }
            if (agent.pathPending) {
                aos[i].agentDesiredVel = aos[i].prevCalculatePathSucceeded
                    ? aos[i].agentDesiredVel
                    : float3.zero;
                continue;
            }
            if (agent.pathStatus == NavMeshPathStatus.PathComplete) {
                aos[i].prevCalculatePathSucceeded = true;
                // Direction from the agent's own steering (avoidance is off, so this is the path
                // direction). The agent decides when a corner is reached and cuts it smoothly.
                Vector3 pathDir = agent.desiredVelocity;
                pathDir.y = 0f;
                // On the last segment, aim at the live target instead of the possibly stale destination.
                if ((agent.steeringTarget - agent.pathEndPosition).sqrMagnitude < 0.25f * 0.25f) {
                    pathDir = tgtPos - pos;
                    pathDir.y = 0f;
                }
                if (pathDir.sqrMagnitude > 0.0001f)
                    pathDir.Normalize();
                else
                    pathDir = Vector3.zero;
                Vector3 dir = pathDir;
                if (pathDir != Vector3.zero) {
                    Vector3 sep = Vector3.zero;
                    for (int j = 0; j < entityCount; j++) {
                        if (j == i)
                            continue;
                        Vector3 away = pos - aos[j].cp.transform.position;
                        away.y = 0f;
                        float d = away.magnitude;
                        if (d < 0.0001f || d >= separationRadius)
                            continue;
                        sep += (away / d) * (1f - d / separationRadius);
                    }
                    // Cap the separation so it can't overpower the path direction.
                    if (sep.sqrMagnitude > 1f)
                        sep.Normalize();
                    // Remove only the part that pushes against the path direction.
                    float along = Vector3.Dot(sep, pathDir);
                    if (along < 0f)
                        sep -= pathDir * along;
                    dir = pathDir + sep * separationWeight;
                    dir.y = 0f;
                    if (dir.sqrMagnitude > 0.0001f)
                        dir.Normalize();
                    else
                        dir = pathDir;
                    // Limit how fast the direction can turn.
                    Vector3 prev = (Vector3)aos[i].agentDesiredVel;
                    prev.y = 0f;
                    if (maxTurnRate > 0f && prev.sqrMagnitude > 0.0001f) {
                        prev.Normalize();
                        dir = Vector3.RotateTowards(prev, dir, maxTurnRate * dt, 0f);
                    }
                }
                aos[i].agentDesiredVel = (float3)dir;
            }
            else {
                aos[i].prevCalculatePathSucceeded = false;
                aos[i].agentDesiredVel = float3.zero;
            }
            bool tgtMoved = (tgtPos - agent.destination).sqrMagnitude > repathTgtMoveDistSq;
            bool retryFailed = !aos[i].prevCalculatePathSucceeded;
            if ((tgtMoved || retryFailed) && aos[i].repathTimer <= 0f) {
                agent.SetDestination(tgtPos);
                aos[i].repathTimer = repathInterval;
            }
            Dbg.Log(
                $"f{Time.frameCount} pos {pos} tgt {tgtPos} dest {agent.destination} "
                    + $"steerTgt {agent.steeringTarget} pathEnd {agent.pathEndPosition} status "
                    + $"{agent.pathStatus} pending {agent.pathPending} hasPath {agent.hasPath} desVel "
                    + $"{aos[i].agentDesiredVel} input_Mov {aos[i].ctrlInputData.input_Mov}",
                cp.so_cpData.enableDbgMsgs
            );
        }
    }

    void Tick_BehaviorTrees() {
        for ( int i = 0; i < entityCount; i++) {
            switch (aos[i].bt.Eval()) {
                case BtResult.Success:
                    aos[i].bt.Reset();
                    break;
                case BtResult.Failure:
                    aos[i].bt.Reset();
                    break;
                case BtResult.Running:
                    Dbg.Log($"Bt {i} running.");
                    break;
                default:
                    Debug.LogError($"Switch defaulted");
                    break;
            }
        }
    }

    /// <summary>
    /// NOTE: Reset "WasPressedThisFrame" inputs.
    /// </summary>
    void Tick_ResetWasPressedThisFrameInputs() {
        for (int i = 0; i < entityCount; i++) {
            aos[i].ctrlInputData.input_Atk_Light = false;
            aos[i].ctrlInputData.input_Atk_Heavy = false;
            aos[i].ctrlInputData.input_Atk_Ult = false;
            aos[i].ctrlInputData.input_Dodge = false;
        }
    }

    // ----------------------------------------------------------------------------------------
    // Other Methods
    // ----------------------------------------------------------------------------------------

    /// <summary>
    /// Gets ref to corresponding <see cref="AiCtrlData"/>.
    /// </summary>
    public static ref AiCtrlData GetData(int aiCtrlI) 
        => ref inst.aos[aiCtrlI];

    /// <summary>
    /// Gets ref to corresponding <see cref="AiCtrlData"/>.
    /// </summary>
    public static ref AiCtrlData GetData(AiCtrlHandle aiCtrlHandle)
        => ref inst.aos[aiCtrlHandle.I];

    public static bool HasFollowTgt(int aiCtrlI) {
        //Debug.Log($"{aiCtrlI} followTgt: {GetData(aiCtrlI).followTgt}");
        //if(GetData(aiCtrlI).followTgt != null) {
        //    Debug.Log($"{aiCtrlI} followTgt should not be null: {GetData(aiCtrlI).followTgt}");
        //    Dbg.Log($"{aiCtrlI} followTgt.TrfToFollow: "
        //        + $"{GetData(aiCtrlI).followTgt.TrfToFollow}");
        //}
        // NOTE: When you call Destroy for a Unity object, a interface reference to that object will return
        // C: null, BUT interface reference might not be marked null yet so it is considered not null.
        // C: Therefore you need the cast first:
        return GetData(aiCtrlI).followTgt is UnityEngine.Object unityObj && unityObj != null;
    }

    public static bool IsWithinDistToFollowTgt(int aiCtrlI, float maxDist) {
        Debug.Assert(
            GetData(aiCtrlI).followTgt != null,
            $"{nameof(AiCtrlData.followTgt)} at index {aiCtrlI} was null!"
        );
        float dist = Vector3.Distance(
            GetData(aiCtrlI).cp.transform.position,
            GetData(aiCtrlI).followTgt.TrfToFollow.position
        );
        //Dbg.Log($"Dist to tgt: {dist}. MaxDist: {maxDist}", inst.cp[cpI], inst.aosData[cpI].enableDbgMsgs);
        return dist < maxDist;
    }

    /// <summary>
    /// Tries to find any eligible follow tgt and set it as cur follow tgt.
    /// </summary>
    public static bool TryFindFollowTgt(int aiCtrlI) {
        if (HasFollowTgt(aiCtrlI))
            return true;
        GetData(aiCtrlI).followTgt = CpMgr.TryFindEnemy(GetData(aiCtrlI).cp.I);
        if(GetData(aiCtrlI).followTgt == null)
            return false;
        return true;
    }

    /// <summary>
    /// Tries to lock onto the target the <paramref name="aiCtrl"/> is currently following.<br/>
    /// NOTE: Expects the <paramref name="aiCtrl"/> to already have a <see cref="AiCtrlData.followTgt"/>!<br/>
    /// NOTE 2: The <see cref="Cp_NonUnityObjClassRefs.lockOnTgt"/> is owned by <see cref="CpMgr"/> instead
    /// of <see cref="AiCtrlMgr"/> since in the future we might want to implement lock on funcitonality for
    /// the player as well. (20.9.2026)
    /// </summary>
    public static bool TryLockOnToFollowTgt(int aiCtrl) {
        if(GetData(aiCtrl).followTgt is ILockOnTargetable lockOnTgt) {
            CpMgr.GetData(GetData(aiCtrl).cp.I).classRefs.lockOnTgt = lockOnTgt;
            return true;
        }
        return false;
    }
}
