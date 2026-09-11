using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

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

[Serializable]
public struct HitEffects {
    public int dmg;
    public KnockbackT knockbackT;
    /// <summary>
    /// 1 equals knocback movement of 1 unit.
    /// </summary>
    public float knockbackStr;

    public HitEffects(int dmg, KnockbackT knockbackT, float knockbackStr) {
        this.dmg = dmg;
        this.knockbackT = knockbackT;
        this.knockbackStr = knockbackStr;
    }
}

public struct BtNodeData {
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

[Serializable]
public struct ComboNodeConfig {
    public ComboNodeT t;
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
    public CpSt_Atk_BasicBranch atk_BasicWindup;
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

    public Cp_ActSts(int cpId) {
        death = new(cpId);
        atk_BasicImpact = new(cpId);
        atk_BasicWindup = new(cpId);
        atk_BasicRecovery = new(cpId);
        atk_FlyingAtk = new(cpId);
        atk_Jump = new(cpId);
        atk_ShootHomingProj = new(cpId);
        dodge = new(cpId);
        falling = new(cpId);
        fallLanding = new(cpId);
        idle = new(cpId);
        knockback = new(cpId);
        walk = new(cpId);
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
    public float3 animDPos;
    public quaternion animDRot;
    public bool bufferedInputStSwitchAllowed;
    public bool comboAllowed;
    public float curStDur;
    public bool dodgeAllowed;
    public bool enableDebugMsgs;
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
    public bool input_atk_Light;
    public bool input_atk_Heavy;
    public bool input_atk_Ult;
    public bool input_dodge;
    public BufferableInput inputBuffer_BufferedInput;
    public float inputBuffer_RemainingTime;
    public bool inputRotAllowed;
    public bool invul;
    public bool isAffectedByGravity;
    public bool isGrounded;
    public bool isSwitchingSt;
    public float3 lastCcVel;
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
    public float3 trf_lossyScl;
    public float3 trf_pos;
    public quaternion trf_rot;
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

public struct Cp_BrainData {
    public float3 agentDesiredVel;
    public float aggroRange;
    public float atkRange;
    public float distToTgt;
    public bool hasTgt;
    public bool inAggroRange;
    public bool inAtkRange;
    public bool prevCalculatePathSucceeded;
    public LockOnTgt lockedOnTgt;
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

public struct Cp_NonUnityCompClassRefs {
    public Cp_ActSts actSts;
    public IFsmSt_Cp st_cur;
    public IFsmSt_Cp st_prev;

    public Cp_NonUnityCompClassRefs(int cpId) {
        actSts = new Cp_ActSts(cpId);
        st_cur = null;
        st_prev = null;
    }
}

/// <summary>
/// Monobehavior (and other Unity Component) references for capsule pawn.
/// </summary>
[Serializable]
public struct Cp_UnityComps {
    public Animator anim;
    public CpAnimEventHandler animEventHandler;
    public CharacterController cc;
    public CpCtrl cpCtrl;
    public CpHitRecieveHandler hitRecieverHandler;
    public NavMeshAgent navMeshAgent;
    public Transform rHand;
    public IHandItem rHandItem;
    public Transform rootTrf;
    public Transform lockOnTrf;
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

public struct HitData {
    public HitEffects atkData;
    public Vector3 hitWldDir;

    public HitData(HitEffects atkData, Vector3 hitWldDir) {
        this.atkData = atkData;
        this.hitWldDir = hitWldDir;
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
