using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class AiCtrlMgr : Singleton<AiCtrlMgr>{
    [Header("Settings")]
    [Tooltip("Initial capacity of entities")]
    public int initCapacity = 1;

    [HideInInspector] public AiCtrlData[] aos;

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
        AiCtrlHandle newHandle,
        IBtNode newBt,
        CpHandle controlledCp,
        AiCtrlConfigData configData
    ) {
        Debug.Assert(newHandle != null);
        int newI = entityCount;
        AiCtrlData newData = new AiCtrlData(
            configData.aggroRange,
            configData.atkRange,
            newBt,
            controlledCp,
            newHandle
        );
        ArrayUtils.Add(ref aos, newI, newData);
        controlledCp.Data.action_markedForPendingUnregister += newHandle.OnCpMarkedForUnregister;
        newHandle.I = newI;
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

    public void Tick() {
        Tick_ResetWasPressedThisFrameInputs();
        // Note: here you could set a ai tick budget e.g. only tick half of the ai controllers during one frame.
        Tick_BehaviorTrees();
        Tick_AgentMovInput();
    }

    // ----------------------------------------------------------------------------------------
    // Tick Methods
    // ----------------------------------------------------------------------------------------

    /// <summary>
    /// Updates nav mesh agents' pathfinding and assigns <see cref="AiCtrlData.agentDesiredVel"/>
    /// for each AiCtrl.
    /// </summary>
    void Tick_AgentMovInput() {
        for (int i = 0; i < entityCount; i++) {
            int cpI = aos[i].cp.I;
            var cpUnityComps = CpMgr.inst.aos[cpI].unityComps;
            //var cpClassRefs = CpMgr.inst.aos[cpI].classRefs;
            //Debug.Log("cpI " + cpI);
            if (aos[i].followTgt == null) {
                //Dbg.Log(
                //    $"{cpI} Set agent desired vel to 0 because tgt was null: {cpClassRefs.lockedOnTgt}",
                //    CpMgr.GetAos(cpI).enableDbgMsgs
                //);
                cpUnityComps.navMeshAgent.ResetPath();
                aos[i].agentDesiredVel = float3.zero;
                continue;
            }
            // NOTE: nav mesh agent can drift away from the actual transform because nav mesh agents suck.
            cpUnityComps.navMeshAgent.nextPosition = aos[i].cp.transform.position;
            // NOTE: We need to check this manually since SetDestination does not have option to set
            // NOTE C: target sample position max distance.
            if (!aos[i].followTgt.IsOnNavMesh()) {
                //Dbg.Log(
                //    $"{cpI} Set agent desired vel to 0 since tgt was not on navmesh.",
                //    CpMgr.GetAos(cpI).enableDbgMsgs
                //);
                cpUnityComps.navMeshAgent.ResetPath();
                aos[i].agentDesiredVel = float3.zero;
                continue;
            }
            // NOTE: The point of this is to START path finding calculation if there is no previous path
            // NOTE C: calculation (e.g. no path status) and if the agent is not currenly calculating a path.
            // NOTE C: I think this might be incorrect way to do it (maybe) but the agent navigation seems
            // NOTE C: to work well enough for now.
            if (!cpUnityComps.navMeshAgent.hasPath) {
                //Dbg.Log($"{cpI} Agent had no path. Set destination.", CpMgr.GetAos(cpI).enableDbgMsgs);
                cpUnityComps.navMeshAgent.SetDestination(aos[i].followTgt.TrfToFollow.position);
                continue;
            }
            // If we are close enough to the destination, stop desiring movement.
            if (
                Vector3.SqrMagnitude(
                    cpUnityComps.navMeshAgent.destination - aos[i].cp.transform.position
                ) < 0.1f // NOTE: Stopping distane is hard coded.
            ) {
                //Dbg.Log(
                //    $"{cpI} Set agent desired vel to 0 since we reached the target vicinity.",
                //    CpMgr.GetAos(cpI).enableDbgMsgs
                //);
                cpUnityComps.navMeshAgent.ResetPath();
                aos[i].agentDesiredVel = float3.zero;
                continue;
            }
            // NOTE: We only use the current unfinished path if last path calculation was completed. This way
            // NOTE C: if we get sequential failed path finding attempts, the character will not move at all
            // NOTE C: (instead of jittering a little because of the partial paths).
            if (cpUnityComps.navMeshAgent.pathPending) {
                //Dbg.Log($"{cpI} Path was pending.", CpMgr.GetAos(cpI).enableDbgMsgs);
                if (aos[i].prevCalculatePathSucceeded)
                    aos[i].agentDesiredVel = cpUnityComps.navMeshAgent.desiredVelocity;
                else
                    aos[i].agentDesiredVel = float3.zero;
                continue;
            }
            if (cpUnityComps.navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete) {
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
                aos[i].agentDesiredVel = cpUnityComps.navMeshAgent.desiredVelocity;
            }
            else {
                //Dbg.Log($"{i} Did not find path. Setting desired vel to 0.", data.enableDebugMsgs[i]);
                aos[i].prevCalculatePathSucceeded = false;
                aos[i].agentDesiredVel = float3.zero;
            }
            cpUnityComps.navMeshAgent.SetDestination(aos[i].followTgt.TrfToFollow.position);
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

    public static bool HasFollowTgt(int aiCtrlI)
        => GetData(aiCtrlI).followTgt != null;

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
