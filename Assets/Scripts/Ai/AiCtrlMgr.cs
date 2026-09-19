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
        int newId = entityCount;
        AiCtrlData newData = new AiCtrlData(
            configData.aggroRange,
            configData.atkRange,
            newBt,
            controlledCp,
            newHandle
        );
        ArrayUtils.Add(ref aos, newId, newData);
        newHandle.Id = newId;
        entityCount++;
    }

    /// <summary>
    /// Unregisters an AI controller, removes its runtime data and destroys
    /// the controller GameObject.
    /// </summary>
    public void Unregister(int id) {
        if (id < 0 || id >= entityCount) {
            Debug.LogError(
                $"Invalid AiCtrl id {id}. Ctrl count was {entityCount}."
            );
            return;
        }
        int lastId = entityCount - 1;
        AiCtrlHandle swappedCtrl = id != lastId
            ? aos[lastId].handle
            : null;
        ArrayUtils.RemoveAtSwapBack(aos, entityCount, id);
        entityCount--;
        if (swappedCtrl != null)
            swappedCtrl.Id = id;
    }

    // ----------------------------------------------------------------------------------------
    // Tick Methods
    // ----------------------------------------------------------------------------------------

    public void Tick() {
        Tick_ResetWasPressedThisFrameInputs();
        Tick_AgentMovInput();
        Tick_BehaviorTrees();
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
            int cpId = aos[i].cp.Id;
            var cpUnityComps = CpMgr.inst.unityComps[cpId];
            var cpClassRefs = CpMgr.inst.classRefs[cpId];
            //Debug.Log("cpId " + cpId);
            if (cpClassRefs.lockedOnTgt == null) {
                //Dbg.Log(
                //    $"{cpId} Set agent desired vel to 0 because tgt was null: {cpClassRefs.lockedOnTgt}",
                //    CpMgr.GetAos(cpId).enableDbgMsgs
                //);
                cpUnityComps.navMeshAgent.ResetPath();
                aos[i].agentDesiredVel = float3.zero;
                continue;
            }
            // NOTE: nav mesh agent can drift away from the actual transform because nav mesh agents suck.
            cpUnityComps.navMeshAgent.nextPosition = aos[i].cp.transform.position;
            // NOTE: We need to check this manually since SetDestination does not have option to set
            // NOTE C: target sample position max distance.
            if (!CpUtils.IsOnNavMesh(cpClassRefs.lockedOnTgt.Id)) {
                //Dbg.Log(
                //    $"{cpId} Set agent desired vel to 0 since tgt was not on navmesh.",
                //    CpMgr.GetAos(cpId).enableDbgMsgs
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
                //Dbg.Log($"{cpId} Agent had no path. Set destination.", CpMgr.GetAos(cpId).enableDbgMsgs);
                cpUnityComps.navMeshAgent.SetDestination(cpClassRefs.lockedOnTgt.LockOnTrf.position);
                continue;
            }
            // If we are close enough to the destination, stop desiring movement.
            if (
                Vector3.SqrMagnitude(
                    cpUnityComps.navMeshAgent.destination - aos[i].cp.transform.position
                ) < 0.1f // NOTE: Stopping distane is hard coded.
            ) {
                //Dbg.Log(
                //    $"{cpId} Set agent desired vel to 0 since we reached the target vicinity.",
                //    CpMgr.GetAos(cpId).enableDbgMsgs
                //);
                cpUnityComps.navMeshAgent.ResetPath();
                aos[i].agentDesiredVel = float3.zero;
                continue;
            }
            // NOTE: We only use the current unfinished path if last path calculation was completed. This way
            // NOTE C: if we get sequential failed path finding attempts, the character will not move at all
            // NOTE C: (instead of jittering a little because of the partial paths).
            if (cpUnityComps.navMeshAgent.pathPending) {
                //Dbg.Log($"{cpId} Path was pending.", CpMgr.GetAos(cpId).enableDbgMsgs);
                if (aos[i].prevCalculatePathSucceeded)
                    aos[i].agentDesiredVel = cpUnityComps.navMeshAgent.desiredVelocity;
                else
                    aos[i].agentDesiredVel = float3.zero;
                continue;
            }
            if (cpUnityComps.navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete) {
                aos[i].prevCalculatePathSucceeded = true;
                //Debug.Log($"Entity id: {cpId}");
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
            cpUnityComps.navMeshAgent.SetDestination(cpClassRefs.lockedOnTgt.LockOnTrf.position);
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

    // ----------------------------------------------------------------------------------------
    // Other Methods
    // ----------------------------------------------------------------------------------------

    /// <summary>
    /// Gets ref to corresponding <see cref="AiCtrlData"/>.
    /// </summary>
    public static ref AiCtrlData GetData(int aiCtrlId) 
        => ref inst.aos[aiCtrlId];

    /// <summary>
    /// Gets ref to corresponding <see cref="AiCtrlData"/>.
    /// </summary>
    public static ref AiCtrlData GetData(AiCtrlHandle aiCtrl) 
        => ref inst.aos[aiCtrl.Id];
}
