using UnityEngine;

/// <summary>
/// Capsule flying head manager.
/// </summary>
public class CpFlyingHeadMgr : Singleton<CpFlyingHeadMgr> {
    [Tooltip("Initial capacity of arrays. They allocate more space if needed (but do not deallocate even" +
        "if pawns are unregistered.)")]
    [SerializeField] int initCapacity = 1;

    [HideInInspector] public Cp_CommonData[] commonData;
    [HideInInspector] public CpFlyingHead_Data[] flyingHeadData;

    /// <summary>
    /// Used to set the used length of the arrays (since they do not reallocate when elements are removed).
    /// </summary>
    int entityCount;

    public void Init() {
        commonData = new Cp_CommonData[initCapacity];
        flyingHeadData = new CpFlyingHead_Data[initCapacity];
    }

    // ------------------------------------------------------------
    // Register and Unregister
    // ------------------------------------------------------------

    /// <summary>
    /// Registers new flying head.
    /// NOTE: Initialize the game object beforehand and pass it in as <paramref name="newCp"/>.
    /// </summary>
    public void Register(CpFlyingHeadHandle newCp) {
        Cp_CommonData newCommonData = new();
        CpFlyingHead_Data newFlyingHeadData = new();
        // NOTE: If these are not set to false, the nav mesh agent component will try to move the
        // NOTE C: pawn trf. NavMeshAgent will still move its own position and rotation which can cause
        // NOTE C: problems if you don't set the drifting navmesh position back to the transform position
        // NOTE C: and rotation every time you move the capsule pawn.
        newCp.navMeshAgent.updatePosition = false;
        newCp.navMeshAgent.updateRotation = false;
        newCommonData.classRefs = new Cp_NonUnityObjClassRefs(null);
        newCommonData.curStDur = 0;
        newCommonData.groundCastHitSomething = false;
        newCommonData.groundCastNrm = Unity.Mathematics.float3.zero;
        newCommonData.handle = newCp;
        newCommonData.hp_Cur = newCp.CpCommonConfig.hp_Max;
        newCommonData.input_mov = Unity.Mathematics.float2.zero;
        newCommonData.input_mov_LastNonZero = Unity.Mathematics.float2.zero;
        newCommonData.input_mov_WhenLastSwitchedSt = Unity.Mathematics.float2.zero;
        newCommonData.ignoreHits = false;
        newCommonData.isAffectedByGravity = true;
        newCommonData.isGrounded = true;
        newCommonData.lastKnockbackStr = 0;
        newCommonData.lastRecievedHitDir = Unity.Mathematics.float3.zero;
        newCommonData.vel_Hor = Unity.Mathematics.float2.zero;
        newCommonData.vel_Ver = 0;
        // NOTE: We set default maxDistToNavMesh to 0.2! (10.9.2026) TODO: Put this into global variables.
        newCommonData.navTgtInfo = new(false, false, 0.2f);
        newFlyingHeadData.handle = newCp;
        newFlyingHeadData.actSts = new(newCp);
        ArrayUtils.Add(ref commonData, entityCount, newCommonData);
        ArrayUtils.Add(ref flyingHeadData, entityCount, newFlyingHeadData);
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
        CpRegister.inst.cps.Remove(flyingHeadData[cpI].handle);
        GameObject.Destroy(flyingHeadData[cpI].handle.gameObject);
        int lastI = entityCount - 1;
        CpFlyingHeadHandle swappedCp = cpI != lastI ? flyingHeadData[lastI].handle : null;
        ArrayUtils.RemoveAtSwapBack(commonData, entityCount, cpI);
        ArrayUtils.RemoveAtSwapBack(flyingHeadData, entityCount, cpI);
        entityCount--;
        if (swappedCp != null)
            // Last Cp was swapped to cpI, so update Id.
            swappedCp.I = cpI;
    }

    // ------------------------------------------------------------
    // Fixed Tick Methods
    // ------------------------------------------------------------

    public void FixedTick() {
        CpUtils.UpdateGroundCheck(commonData, entityCount);
        CpUtils.FixedTick_Fsm(commonData, entityCount);
    }

    // ------------------------------------------------------------
    // Tick Methods
    // ------------------------------------------------------------

    public void Tick(float dt) {
        CpUtils.Tick_TickSetup(commonData, dt, entityCount);
        CpUtils.Tick_ReadMovInput(commonData, entityCount);
        CpUtils.Tick_InputBuffer(commonData, dt, entityCount);
        CpUtils.Tick_Fsm(commonData, entityCount);
    }

    // ------------------------------------------------------------
    // Late Tick Methods
    // ------------------------------------------------------------

    public void LateTick(float dt) {
        // NOTE: We move character controller right after animation update so that animation rootmotion is
        // C: applied instantly.
        CpUtils.LateTick_Mov(commonData, dt, entityCount);
        LateTick_AnimEventPlr();
        CpUtils.LateTick_Fsm(commonData, entityCount);
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
                flyingHeadData[i].handle,
                ref commonData[i].animEventPlrData,
                commonData[i].handle.Anim,
                CpFlyingHeadUtils.OnAnimEvent,
                flyingHeadData[i].handle.CpCommonConfig.enableDbgMsgs
            );
        }
    }

    /// <summary>
    /// NOTE: We want to unregister an entity at a safe point when we are not looping over the entities or
    /// otherwise using their Id's. You can safely mark a cp for deletion by destroying its
    /// <see cref="CpFlyingHeadHandle"/>, it will then be unregistered here.
    /// </summary>
    void LateTick_UnregisterNDestroyPending() {
        int i = 0;
        // We swap the last element in the place of the unregistered one, so we only increment index if we
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

    public static ref Cp_CommonData GetCommonData(int cpI)
        => ref inst.commonData[cpI];

    public static ref CpFlyingHead_Data GetFlyingHeadData(int cpI)
        => ref inst.flyingHeadData[cpI];

    public void SwitchToInitActSt(int cpI) {
        // NOTE: Change this if FlyingHead has a different initial state.
        CpUtils.TrySwitchActSt(
            () => flyingHeadData[cpI].actSts.idle.Enter(),
            ref commonData[cpI],
            true
        );
    }
}