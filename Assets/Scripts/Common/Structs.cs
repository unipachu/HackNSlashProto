using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Used to configure data of the ai controller.
/// </summary>
[Serializable]
public struct AiCtrlConfigData {
    public float aggroRange;
    public float atkRange;

    public AiCtrlConfigData(float aggroRange, float atkRange) {
        this.aggroRange = aggroRange;
        this.atkRange = atkRange;
    }
}

public struct AiCtrlData {
    public float3 agentDesiredVel;
    public float aggroRange;
    public float atkRange;
    public IBtNode bt;
    public CpHandle cp;
    public CtrlInputData ctrlInputData;
    public IFollowTgt followTgt;
    public AiCtrlHandle handle;
    public bool prevCalculatePathSucceeded;

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
        ctrlInputData = default;
        followTgt = null;
        this.handle = handle;
        prevCalculatePathSucceeded = false;
    }
}

/// <summary>
/// Grouped Animator state info used by <see cref="AnimEventPlr"/>.
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

[Obsolete()]
public struct BtNodeDataOld {
    public int childCount;
    /// <summary>
    /// Id for optional data the node might use.
    /// </summary>
    public int dataId;
    public int firstChild;
    public int nextSibling;
    public FixedString32Bytes nodeName;
    public int parent;
    public BtNodeT t;
}

[Obsolete]
public struct  BtNodeConfig {
    public string dbgName;
    public BtNodeT t;
    public List<BtNodeConfig> children;
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

[Serializable]
public struct ComboNodeConfig {
    public ComboNodeConfigT t;
    public CpAnimInfoT animInfo;
    public byte node_BtnE;
    public byte node_LShldr;
    public byte node_NoInput;
    public byte node_RShldr;
    public byte node_RTrg;
}

public struct ComboNode_Transitions {
    public IComboNode node_BtnE;
    public IComboNode node_LShldr;
    public IComboNode node_NoInput;
    public IComboNode node_RShldr;
    public IComboNode node_RTrg;
}

/// <summary>
/// All action states available for capsule pawn.
/// </summary>
public struct Cp_ActSts {
    public CpSt_Death death;
    public CpSt_Atk_BasicImpact atk_BasicImpact;
    public CpSt_Atk_BasicBranch atk_BasicBranch;
    public CpSt_Atk_BasicRecovery atk_BasicRecovery;
    public CpSt_Atk_FlyingAtk atk_FlyingAtk;
    public CpSt_Atk_Jump atk_Jump;
    public CpSt_Atk_ShootHomingProj atk_ShootHomingProj;
    public CpSt_Dodge dodge;
    public CpSt_Falling falling;
    public CpSt_FallLanding fallLanding;
    public CpSt_Idle idle;
    public CpSt_Knockback knockback;
    public CpSt_Walk walk;

    public Cp_ActSts(CpHandle cp) {
        death = new(cp);
        atk_BasicImpact = new(cp);
        atk_BasicBranch = new(cp);
        atk_BasicRecovery = new(cp);
        atk_FlyingAtk = new(cp);
        atk_Jump = new(cp);
        atk_ShootHomingProj = new(cp);
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
public struct Cp_AosData {
    public AtkPhase act_AtkPhase;
    // NOTE: "act_" means action state specific data (11.9.2026)
    public float act_AtkFlying_TgtHorSpd;
    public float act_AtkJump_DownSpeedAfterJumpFinished;
    public float act_Dodge_HorMovSpdMult;
    public float act_Dodge_YawSpd;
    public float act_Falling_HorAcc;
    public float act_Falling_LandingStFallDistThreshold;
    public float act_Falling_StartHgt;
    public float act_Falling_TgtHorSpd;
    public float act_BasicImpact_YawSpd;
    public float act_BasicRecovery_MotInterpTimer;
    public float act_BasicWindup_MaxAngSpd;
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
    /// <see cref="Cp_AosData.pendingUnregister"/>, which is tiresome compared to subscribing to this action.
    /// </summary>
    public Action action_markedForPendingUnregister;
    /// <summary>
    /// Parameters are (curHp, maxHp) (NOT the changed amount)!
    /// </summary>
    public Action<int, int> action_maxHpChanged;
    /// <summary>
    /// Invoked when local player ends the lock on to this.
    /// </summary>
    public Action action_plrLockedOnEnded;
    /// <summary>
    /// Invoked when local player locks onto this.
    /// </summary>
    public Action action_plrLockedOnStarted;
    public float3 animDPos;
    public quaternion animDRot;
    public bool bufferedInputStSwitchAllowed;
    public bool comboAllowed;
    public float curStDur;
    public string displayName;
    public bool dodgeAllowed;
    public bool enableDbgMsgs;
    public bool groundCastHitSomething;
    public float3 groundCastNrm;
    public float groundSnapVerDownSpd;
    public int hp_Cur;
    public int hp_Max;
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
    public bool inputRotAllowed;
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
    /// Teams are used to prohibit friendly fire and for the ai to choose targets only from other teams.
    /// </summary>
    public PawnTeam team;
    /// <summary>
    /// Current horisontal (XZ) velocity.
    /// </summary>
    public float2 vel_Hor;
    /// <summary>
    /// Current vertical (Y) velocity.
    /// </summary>
    public float vel_Ver;
    public float walkLinAcc;
    public float walkMaxLinSpd;
    public float walkYawSpd;
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

/// <summary>
/// Unity Object references for capsule pawn.
/// </summary>
[Serializable]
public struct Cp_UnityObjs {
    public Animator anim;
    public CpAnimEventHandler animEventHandler;
    public CharacterController cc;
    public CpHitReciever hitRecieverHandler;
    public NavMeshAgent navMeshAgent;
    public Transform rHand;
    public Transform lockOnTrf;
    public Transform wldHpBarPos;
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

public struct HitData {
    public HitEffects effects;
    public PawnTeam team;
    public Vector3 wldDir;

    public HitData(HitEffects effects, PawnTeam team, Vector3 wldDir) {
        this.effects = effects;
        this.team = team;
        this.wldDir = wldDir;
    }
}

[Serializable]
public struct HitEffects {
    public int dmg;
    public KnockbackT knockbackT;
    /// <summary>
    /// This is multiplied by the knockback animation root motion.
    /// </summary>
    public float knockbackStr;

    public HitEffects(int dmg, KnockbackT knockbackT, float knockbackStr) {
        this.dmg = dmg;
        this.knockbackT = knockbackT;
        this.knockbackStr = knockbackStr;
    }
}

public struct HitResult {
    public bool wasInvul;
    public bool wasBlocked;

    public HitResult(bool wasInvul, bool wasBlocked) {
        this.wasInvul = wasInvul;
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
