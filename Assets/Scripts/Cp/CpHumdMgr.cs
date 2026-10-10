using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Capsule pawn (i.e. player or ai controlled character that uses capsule collision for movement) manager.
/// </summary>
public class CpHumdMgr : Singleton<CpHumdMgr> {
    [Tooltip("Initial capacity of arrays. They allocate more space if needed (but do not deallocate even" +
        "if pawns are unregistered.)")]
    [SerializeField] int initCapacity = 1;

    [HideInInspector] public Cp_CommonData[] commonData;
    [HideInInspector] public CpHumd_Data[] humdData;

    /// <summary>
    /// Used to set the used length of the arrays (since they do not reallocate when elements are removed).
    /// </summary>
    int entityCount;

    public void Init() {
        humdData = new CpHumd_Data[initCapacity];
        commonData = new Cp_CommonData[initCapacity];
        //Debug.Log($"{nameof(aos)} length in init: {aos.Length}");
    }

    // ------------------------------------------------------------
    // Register and Unregister
    // ------------------------------------------------------------

    /// <summary>
    /// Registers new capsule pawn.
    /// NOTE: Initialize the game object beforehand and pass it in as a <paramref name="newCp"/>.
    /// </summary>
    public void Register(CpHumdHandle newCp) {
        CpHumd_Data newAosData = new();
        Cp_CommonData newCommonData = new();
        // NOTE: If these are not set to false, the nav mesh agent component will try to move the capsule
        // NOTE C: pawn trf. NavMeshAgent will still move its own position and rotation which can cause
        // NOTE C: problems if you don't set the drifting navmesh position back to the transform position
        // NOTE C: and rotation every time you move the capsule pawn.
        newCp.navMeshAgent.updatePosition = false;
        newCp.navMeshAgent.updateRotation = false;
        newCommonData.classRefs = new Cp_NonUnityObjClassRefs(null); ;
        newCommonData.curStDur = 0;
        newCommonData.groundCastHitSomething = false;
        newCommonData.groundCastNrm = float3.zero;
        newCommonData.handle = newCp;
        newCommonData.hp_Cur = newCp.CpCommonConfig.hp_Max;
        newCommonData.input_mov = float2.zero;
        newCommonData.input_mov_LastNonZero = float2.zero;
        newCommonData.input_mov_WhenLastSwitchedSt = float2.zero;
        newCommonData.ignoreHits = false;
        newCommonData.isAffectedByGravity = true;
        newCommonData.isGrounded = true;
        newCommonData.lastKnockbackStr = 0;
        newCommonData.lastRecievedHitDir = float3.zero;
        newCommonData.vel_Hor = float2.zero;
        newCommonData.vel_Ver = 0;
        // NOTE: We set default maxDistToNavMesh to 0.2! (10.9.2026) TODO: Put this into global variables.
        newCommonData.navTgtInfo = new(false, false, 0.2f);
        ArrayUtils.Add(ref commonData, entityCount, newCommonData);
        newAosData.handle = newCp;
        IHandItem rHandItem = HandItemFactory.InstantiateHandItem(newCp.HumdConfig.rHandItem);
        rHandItem.Trf.SetPositionAndRotation(
            newCp.rHand.position,
            newCp.rHand.rotation
        );
        rHandItem.Trf.parent = newCp.rHand;
        rHandItem.hitSomething += newCp.OnHitSomething;
        newAosData.classRefs = new CpHumd_NonUnityObjClassRefs(newCp, rHandItem);
        ArrayUtils.Add(ref humdData, entityCount, newAosData);
        //Debug.Log($"Switching {freeI} to initial act st!", this);
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
        CpRegister.inst.cps.Remove(humdData[cpI].handle);
        humdData[cpI].classRefs.rHandItem.hitSomething -= humdData[cpI].handle.OnHitSomething;
        GameObject.Destroy(humdData[cpI].handle.gameObject);
        int lastI = entityCount - 1;
        CpHumdHandle swappedCp = cpI != lastI ? humdData[lastI].handle : null;
        ArrayUtils.RemoveAtSwapBack(humdData, entityCount, cpI);
        ArrayUtils.RemoveAtSwapBack(commonData, entityCount, cpI);
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
        CpUtils.UpdateGroundCheck(commonData, entityCount);
        CpUtils.FixedTick_Fsm(commonData, entityCount);
    }


    // ------------------------------------------------------------
    // Tick Methods
    // ------------------------------------------------------------

    public void Tick(float dt) {
        CpUtils.Tick_TickSetup(commonData, dt, entityCount);
        Tick_Cooldowns(dt);
        CpUtils.Tick_ReadMovInput(commonData, entityCount);
        CpUtils.Tick_InputBuffer(commonData, dt, entityCount);
        CpUtils.Tick_Fsm(commonData, entityCount);
    }

    void Tick_Cooldowns(float dt) {
        for (int i = 0; i < entityCount; i++) {
            if (commonData[i].pendingUnregister)
                continue;
            ref var cpData = ref GetHumdData(i);
            if(cpData.cooldownTimer_Dodge > 0 && !cpData.cooldownFreezed_Dodge)
                cpData.cooldownTimer_Dodge = Mathf.Max(0, cpData.cooldownTimer_Dodge - dt);
            //Dbg.Log(
            //    $"cp {i} {nameof(cpData.cooldownTimer_Dodge)}: {cpData.cooldownTimer_Dodge}, " 
            //        + $"{nameof(cpData.cooldownFreezed_Dodge)}: {cpData.cooldownFreezed_Dodge}",
            //    cpData.handle,
            //    cpData.handle.so_cpData.enableDbgMsgs
            //);
        }
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

    // TODO MINOR: other cps probably use similar, could this be made into a generic static method?
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
                humdData[i].handle,
                ref commonData[i].animEventPlrData,
                commonData[i].handle.Anim,
                CpHumdUtils.OnAnimEvent,
                humdData[i].handle.CpCommonConfig.enableDbgMsgs
            );
        }
    }

    /// <summary>
    /// NOTE: We want to unregister an entity at a safe point when we are not looping over the entities or
    /// otherwise using their Id's. You can safely mark a cp for deletion by destroying its
    /// <see cref="CpHumdHandle"/>, it will then be unregistered here.
    /// </summary>
    void LateTick_UnregisterNDestroyPending() {
        int i = 0;
        // We swap the last element in the place of the unregistered one, so we onlu increment index if we
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

    public static ref CpHumd_Data GetHumdData(int cpHumdI)
        => ref inst.humdData[cpHumdI];

    public static ref Cp_CommonData GetCommonData(int cpHumdI)
        => ref inst.commonData[cpHumdI];

    public void SwitchToInitActSt(int cpI) {
        //Debug.Log($"{cpI} switching to init state", this);
        // NOTE: This is currently always enters to idle state. (6.9.2026)
        CpUtils.TrySwitchActSt(() => humdData[cpI].classRefs.actSts.idle.Enter(), ref commonData[cpI], true);
        //Debug.Log($"{cpI} state initialized to : {nameof(Cp_ActSts.idle)}", this);
    }
}