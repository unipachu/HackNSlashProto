using System;
using Unity.Mathematics;
using UnityEngine;

public struct AiCtrlData {
    public float3 agentDesiredVel;
    public float aggroRange;
    public float atkRange;
    public IBtNode bt;
    public CpHandle cp;
    public Vector3 cpPrevPos;
    public CtrlInputData ctrlInputData;
    public IFollowTgt followTgt;
    public AiCtrlHandle handle;
    public bool prevCalculatePathSucceeded;
    public float repathTimer;

    public AiCtrlData(
        float aggroRange,
        float atkRange, 
        IBtNode bt,
        CpHandle cp,
        AiCtrlHandle handle
    ) {
        agentDesiredVel = Vector3.zero;
        this.aggroRange = aggroRange;
        this.atkRange = atkRange;
        this.bt = bt;
        this.cp = cp;
        this.cpPrevPos = cp.transform.position;
        ctrlInputData = default;
        followTgt = null;
        this.handle = handle;
        prevCalculatePathSucceeded = false;
        this.repathTimer = 0;
    }
}

/// <summary>
/// Grouped Animator state and animation event info used by <see cref="AnimEventPlr"/>.
/// </summary>
public struct AnimInfo {
    public int shortNameHash;
    public int animLayer;
    /// <summary>
    /// Does the animation loop?
    /// </summary>
    public bool looping;
    public int lastFrame;
    // NOTE: Struct includes an array reference and thus this cannot be used with native containers! (11.9.2026)
    public AnimEvent[] sortedAnimEvents;

    /// <param name="sortedEvents">
    /// NOTE: Must be sorted ascending by normalized time, otherwise they are not necessarily
    /// called in the right order if multiple event trigger during one tick!
    /// </param>
    public AnimInfo(
        int shortNameHash,
        int animLayer,
        bool looping,
        int lastFrame,
        params (int frame, CpAnimEventT id)[] sortedEvents
    ) {
        this.shortNameHash = shortNameHash;
        this.animLayer = animLayer;
        this.looping = looping;
        this.lastFrame = lastFrame;
        // NOTE: We cannot automaticize sorting since some events might happen on the same frame and yet
        // their order matters.
        sortedAnimEvents = new AnimEvent[sortedEvents.Length];
        // Assert that the animation events are in ascending order based on their timing.
        for (int i = 0; i < sortedEvents.Length; i++) {
            sortedAnimEvents[i] = new AnimEvent(sortedEvents[i].frame, lastFrame, sortedEvents[i].id);
            if (i != 0)
                Debug.Assert(
                    sortedEvents[i - 1].frame <= sortedEvents[i].frame,
                    $"Animation event '{sortedEvents[i].id}' at frame {sortedEvents[i].frame} should happen "
                        + $"after the previous anim event: '{sortedEvents[i - 1].id}' at frame "
                        + $"{sortedEvents[i - 1].frame}"
                );
        }
    }
}

/// <summary>
/// Animation event decoupled from the Animator.
/// </summary>
[Serializable]
public struct AnimEvent {
    /// <summary>
    /// Normalized time of the event during one animation loop.
    /// </summary>
    public float nrmT;
    /// <summary>
    /// Unique id for the animation event.
    /// </summary>
    public CpAnimEventT id;

    /// <param name="frame">
    /// Frame of the animation event.<br/>
    /// NOTE: Unity's first animation frame has an index of 0.
    /// </param>
    /// <param name="lastFrame">
    /// Index of the last frame of the animation. In the Animation window, this is the frame
    /// on the timeline where the animation bar changes to dark grey.
    /// </param>
    /// <param name="id">Unique name for the action event, used to check against a switch case.</param>
    public AnimEvent(int frame, int lastFrame, CpAnimEventT id) {
        nrmT = frame / (float)lastFrame;
        this.id = id;
    }
}

/// <summary>
/// Data used for one animation's animation events.
/// </summary>
public struct AnimEventPlrData {
    public AnimInfo animInfo;
    /// <summary>
    /// Last frame's normalized time from the animator.<br/>
    /// NOTE: This will go over 1 if animation loops.<br/>
    /// NOTE 2: This resets to 0 when <see cref="AnimEventPlr.CrossfadeNInitAnimEventPlr"/> is called and thus is
    /// safe to use after a state switch even if <see cref="Animator"/> has not been updated
    /// for the new state yet.
    /// </summary>
    public float prevTotalNrmT;
    /// <summary>
    /// Normalized position within current loop (0-1)
    /// </summary>
    public float cursor;
    /// <summary>
    /// Authoritative loop counter. Doesn't reset even when animation state is rebased.
    /// </summary>
    public int loopCount;
    /// <summary>
    /// Counter used to start a looping animation from the beginning after
    /// <see cref="loopRebaseThreshold"/> is reached to avoid animation time precision problems.
    /// </summary>
    public int loopsSinceRebase;
    /// <summary>
    /// Has the animation finished (for non-looping only)?
    /// </summary>
    public bool finished;
    /// <summary>
    /// When starting new animation with offset start time, should we still fire the event from
    /// earlier in the animation?
    /// </summary>
    public bool fireEventsBeforeStartOffset;
    /// <summary>
    /// Is this the first tick of the animation?
    /// </summary>
    public bool firstTick;
}

[Serializable]
public struct CapsuleShape {
    public Vector3 pt0;
    public Vector3 pt1;
    public float r;

    public CapsuleShape(Vector3 pt0, Vector3 pt1, float r) {
        this.pt0 = pt0;
        this.pt1 = pt1;
        this.r = r;
    }
}

public struct ComboNode_Transitions {
    public IComboNode node_BtnE;
    public IComboNode node_LShldr;
    public IComboNode node_NoInput;
    public IComboNode node_RShldr;
    public IComboNode node_RTrg;

    public ComboNode_Transitions(
        IComboNode node_BtnE,
        IComboNode node_LShldr,
        IComboNode node_NoInput,
        IComboNode node_RShldr,
        IComboNode node_RTrg
    ) {
        this.node_BtnE = node_BtnE;
        this.node_LShldr = node_LShldr;
        this.node_NoInput = node_NoInput;
        this.node_RShldr = node_RShldr;
        this.node_RTrg = node_RTrg;
    }
}

/// <summary>
/// All action states available for capsule pawn.
/// </summary>
public struct Cp_ActSts {
    public CpSt_Death death;
    public CpSt_Atk_FlyingAtk atk_FlyingAtk;
    public CpSt_Atk_Jump atk_Jump;
    public CpSt_ComboBranch_BasicImpact comboBranch_BasicImpact;
    public CpSt_ComboBranch_LaserAimNShootHomingProj comboBranch_LaserAimNShootHomingProj;
    public CpSt_ComboBranch_RotateToLastNonZeroInputDir comboBranch_RotateToLastNonZeroInputDir;
    public CpSt_ComboBranch_RotateToWhenLastSwitchedStInputDir comboBranch_RotateToWhenLastSwitchedStInputDir;
    public CpSt_ComboEnd_BasicRecovery comboEnd_BasicRecovery;
    // TODO: Maybe name to just BasicShootProj and allow state to use different projectiles.
    public CpSt_ComboBranch_ShootHomingProj comboBranch_ShootHomingProj;
    public CpSt_Dodge dodge;
    public CpSt_Falling falling;
    public CpSt_FallLanding fallLanding;
    public CpSt_Idle idle;
    public CpSt_Knockback knockback;
    public CpSt_Walk walk;

    public Cp_ActSts(CpHandle cp) {
        death = new(cp);
        comboBranch_BasicImpact = new(cp);
        comboBranch_RotateToLastNonZeroInputDir = new(cp);
        comboBranch_RotateToWhenLastSwitchedStInputDir = new(cp);
        comboEnd_BasicRecovery = new(cp);
        atk_FlyingAtk = new(cp);
        atk_Jump = new(cp);
        comboBranch_LaserAimNShootHomingProj = new(cp);
        comboBranch_ShootHomingProj = new(cp);
        dodge = new(cp);
        falling = new(cp);
        fallLanding = new(cp);
        idle = new(cp);
        knockback = new(cp);
        walk = new(cp);
    }
}

/// <summary>
/// NOTE: When looping over SoA data, YOU MUST NOT REMOVE OR ADD NEW ENTITIES TO NOT CAUSE ERRORS WITH THE LOOP!! (10.9.2026) 
/// </summary>
public struct Cp_Data {
    // NOTE: "act_" means action state specific data (11.9.2026)
    public AtkPhase act_AtkPhase;
    public float act_Falling_StartHgt;
    /// <summary>
    /// Parameters are (curHp, maxHp) (NOT the changed amount)!
    /// </summary>
    public Action<int, int> action_curHpChanged;
    /// <summary>
    /// Should be called when cp enters death/dying state (not when in unregisters). Param is cp in dying state.
    /// </summary>
    public Action<CpHandle> action_died;
    /// <summary>
    /// Param is the damage taken.
    /// </summary>
    public Action<int> action_dmgTaken;
    /// <summary>
    /// Objects having reference to this <see cref="CpHandle"/> should listen to this Action and nullify the
    /// reference when this is invoked. Other way would be to check both <see cref="CpHandle"/> == null and
    /// <see cref="Cp_Data.pendingUnregister"/>, which is tiresome compared to subscribing to this action.
    /// </summary>
    public Action action_markedForPendingUnregister;
    /// <summary>
    /// Parameters are (curHp, maxHp) (NOT the changed amount)!
    /// </summary>
    public Action<int, int> action_maxHpChanged;
    /// <summary>
    /// Invoked when local player ends the lock on to this.
    /// </summary>
    public Action action_PlrLockedOnEnded;
    /// <summary>
    /// Invoked when local player locks onto this.
    /// </summary>
    public Action action_PlrLockedOnStarted;
    public float3 animDPos;
    public quaternion animDRot;
    public AnimEventPlrData animEventPlrData;
    public bool bufferedInputStSwitchAllowed;
    public Cp_NonUnityObjClassRefs classRefs;
    public bool comboAllowed;
    public float curStDur;
    public bool dodgeAllowed;
    public bool groundCastHitSomething;
    public float3 groundCastNrm;
    public CpHandle handle;
    public int hp_Cur;
    public float2 input_mov;
    /// <summary>
    /// Last nonzero movement input (in world space).
    /// </summary>
    public float2 input_mov_LastNonZero;
    /// <summary>
    /// Movement input during last state switch (in world space).
    /// </summary>
    public float2 input_mov_WhenLastSwitchedSt;
    public BufferableInput inputBuffer_BufferedInput;
    public float inputBuffer_RemainingTime;
    public bool yawAllowed;
    public bool inputMovAllowed;
    /// <summary>
    /// Invulnerable (character ignores hits).
    /// </summary>
    public bool ignoreHits;
    public bool isAffectedByGravity;
    public bool isGrounded;
    public bool isSwitchingSt;
    public float lastKnockbackStr;
    public float3 lastRecievedHitDir;
    public float2 movInput_tgtHorDir;
    /// <summary>
    /// Action states can set this to the delta animation, or any other value. It is then applied (ignoring
    /// acceleration) after other linear movement calculations (ignoring acceleration).
    /// </summary>
    public float3 movInput_additionalLinMov;
    public float movInput_tgtHorSpd;
    public float movInput_yawSpd;
    public float movInput_horAcc;
    public Cp_NavTgtInfo navTgtInfo;
    public bool pendingUnregister;
    /// <summary>
    /// Current horisontal (XZ) velocity.
    /// </summary>
    public float2 vel_Hor;
    /// <summary>
    /// Current vertical (Y) velocity.
    /// </summary>
    public float vel_Ver;
}

/// <summary>
/// Info so that this pawn can be used as a navigation target BY OTHER PAWNS.
/// </summary>
[Serializable]
public struct Cp_NavTgtInfo {
    /// <summary>
    /// Helper bool so that we only check ONCE A FRAME if a pawn is on a navmesh.
    /// </summary>
    public bool hasUpdatedNavTgtInfoThisTick;
    public bool isCpOnNavmesh;
    /// <summary>
    /// Max distance from navTgt to navmesh
    /// </summary>
    public float maxDistToNavMesh;

    public Cp_NavTgtInfo(
        bool hasUpdatedNavTgtInfoThisTick,
        bool isCpOnNavmesh,
        float maxDistToNavMesh
    ) {
        this.hasUpdatedNavTgtInfoThisTick = hasUpdatedNavTgtInfoThisTick;
        this.isCpOnNavmesh = isCpOnNavmesh;
        this.maxDistToNavMesh = maxDistToNavMesh;
    }
}

/// <summary>
/// Cp class dependencies that do not derive from Unity's Object class.
/// </summary>
public struct Cp_NonUnityObjClassRefs {
    public Cp_ActSts actSts;
    public ICpCtrlInputter cpCtrl;
    /// <summary>
    /// Other pawn this is pawn is locked onto.
    /// </summary>
    public ILockOnTargetable lockOnTgt;
    public IHandItem rHandItem;
    public IFsmSt_Cp st_cur;
    public IFsmSt_Cp st_prev;

    public Cp_NonUnityObjClassRefs(CpHandle cp, ICpCtrlInputter cpCtrl, IHandItem rHandItem) {
        actSts = new Cp_ActSts(cp);
        this.cpCtrl = cpCtrl;
        lockOnTgt = null;
        this.rHandItem = rHandItem;
        st_cur = null;
        st_prev = null;
    }
}

public struct CtrlInputData {
    public bool input_Atk_Light;
    public bool input_Atk_Heavy;
    public bool input_Atk_Ult;
    public bool input_Dodge;
    public Vector2 input_Look_Gamepad;
    public Vector2 input_Look_Pointer;
    public Vector2 input_Mov;
}

[Serializable]
public struct EnemyWave {
    public string name;

    [Header("Enemies")]
    public EnemyWave_EnemyEntry[] enemies;

    [Header("Next Wave Conditions")]
    [Min(0f)] public float timeUntilNextWave;
    [Tooltip("The next wave cannot start while the current enemy count is above this value. "
        + "Set to -1 to disable.")]
    public int blockNextWaveAtEnemyCount;
    [Tooltip("The next wave starts immediately when the current enemy count is at or below this value, "
        + "even if the timer has not expired. Set to -1 to disable.")]
    public int forceNextWaveAtEnemyCount;
}

[Serializable]
public struct EnemyWave_EnemyEntry {
    public So_AiCpConfig enemyConfig;
    [Min(1)] public int amount;
}

public struct HitData {
    public Vector3 hitDealerMovDir;
    public HitDirMode hitDirMode;
    public HitEffects hitEffects;
    /// <summary>
    /// The world position of the where the raycast has hit, or if using overlap shape,
    /// an approximation of a hit point.
    /// </summary>
    public Vector3 hitPt;
    /// <summary>
    /// The normal at the hit location of the raycast, or if using an overlap shape, a normal on
    /// the <see cref="IHitReceiver"/> surface at a approximate hit point, or
    /// <see cref="Vector3.zero"/> if failed to get normal at the approximate hit point!
    /// </summary>
    public Vector3 normal;
    /// <summary>
    /// Used to calculate hit direction if <see cref="HitDirMode"/> is set to
    /// <see cref="HitDirMode.FromHitSourceTrfToHitReciever"/>. E.g. this should be the root/center point of a
    /// character's body who initiates the hit with its melee weapon if we want knock back to be away from
    /// that character.
    /// </summary>
    public Transform srcTrf;
    public Team team;

    public HitData(
        Vector3 hitDealerMovDir,
        HitDirMode hitDirMode,
        HitEffects hitEffects,
        Vector3 hitPt,
        Vector3 normal,
        Transform srcTrf,
        Team team
    ) {
        this.hitDealerMovDir = hitDealerMovDir;
        this.hitDirMode = hitDirMode;
        this.hitEffects = hitEffects;
        this.hitPt = hitPt;
        this.normal = normal;
        this.srcTrf = srcTrf;
        this.team = team;
    }
}

[Serializable]
public struct HitEffects {
    public int dmg;
    public HitT hitT;
    public KnockbackT knockbackT;
    /// <summary>
    /// This is multiplied by the knockback animation root motion.
    /// </summary>
    public float knockbackStr;

    public HitEffects(int dmg, HitT hitT, KnockbackT knockbackT, float knockbackStr) {
        this.dmg = dmg;
        this.hitT = hitT;
        this.knockbackT = knockbackT;
        this.knockbackStr = knockbackStr;
    }
}

public struct HitResult {
    /// <summary>
    /// All hit recievers owned by the hit entity. Can be used to ignore them from further hits.
    /// </summary>
    public IHitReceiver[] allEntityHitReceivers;
    public bool wasBlocked;

    public HitResult(IHitReceiver[] allEntityHitReceivers, bool wasBlocked) {
        this.allEntityHitReceivers = allEntityHitReceivers;
        this.wasBlocked = wasBlocked;
    }
}

[Serializable]
public struct HomingProjData {
    public float spd;
    public float maxLifetime;
    public float homingStr;

    public HomingProjData(float spd, float maxLifetime, float homingStr) {
        this.spd = spd;
        this.maxLifetime = maxLifetime;
        this.homingStr = homingStr;
    }
}

public struct MeleeWeaponData {
    public int atk0Dmg;
    public int atk1Dmg;
    public int atk2Dmg;

    public MeleeWeaponData(
    int atk0Dmg,
    int atk1Dmg,
    int atk2Dmg
    ) {
        this.atk0Dmg = atk0Dmg;
        this.atk1Dmg = atk1Dmg;
        this.atk2Dmg = atk2Dmg;
    }
}

[Serializable]
public struct SphereShape {
    public Vector3 center;
    public float r;

    public SphereShape(Vector3 center, float r) {
        this.center = center;
        this.r = r;
    }
}

[Serializable]
public struct VectorGizmo {
    public Vector3 pt;
    public Vector3 dir;
    public float expireTime;

    public VectorGizmo(Vector3 pt, Vector3 dir, float expireTime) {
        this.pt = pt;
        this.dir = dir;
        this.expireTime = expireTime;
    }
}

public struct WldHpBarData {
    public Transform anchor;
    public float barVisibleUntil;
    public CpHandle cpHandle;
    public float dmgNumberVisibleUntil;
    public WldHpBar hpBar;
    public bool isLocked;
    public int accumulatedDmg;
    public bool pendingUnregister;
    public RectTransform rect;
    public HpBarTrailingSt trailingSt;
    public float trailingDelayStartTime;
}
