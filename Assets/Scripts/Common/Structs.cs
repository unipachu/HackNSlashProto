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
    /// Last frame's normalized time from the animator.
    /// NOTE: This will go over 1.
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
    /// 
    /// </summary>
    public bool fireEventsBeforeStartOffset;
    /// <summary>
    /// Is this the first tick of the animation?
    /// </summary>
    public bool firstTick;
}

[Serializable]
public struct AtkData {
    public int dmg;
    public KnockbackT knockbackT;
    /// <summary>
    /// 1 equals knocback movement of 1 unit.
    /// </summary>
    public float knockbackStr;

    public AtkData(int dmg, KnockbackT knockbackT, float knockbackStr) {
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
    public CpSt_Atk_BasicActive atk_BasicImpact;
    public CpSt_Atk_BasicWindup atk_BasicWindup;
    public Cp_Atk_BasicRecovery atk_BasicRecovery;
    public CpSt_Atk_FlyingAtk atk_FlyingAtk;
    public CpSt_Atk_Jump atk_Jump;
    public CpSt_Atk_ShootHomingProj atk_ShootHomingProj;
    public CpSt_Dodge dodge;
    public CpSt_Falling falling;
    public CpSt_FallLanding fallLanding;
    public CpSt_Idle idle;
    public CpSt_Knockback_Weak knockback;
    public CpSt_Walk walk;

    public Cp_ActSts(int cpId) {
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

public struct Cp_SoaData {
    public NativeArray<AtkPhase> actStSt_AtkPhase;
    public NativeArray<bool> actStSt_BufferedInputStSwitchAllowed;
    public NativeArray<bool> actStSt_ComboAllowed;
    public NativeArray<bool> actStSt_DodgeAllowed;
    public NativeArray<float> actStSt_FallingStartHgt;
    public NativeArray<bool> actStSt_ImpactFinished;
    public NativeArray<bool> actStSt_InputRotAllowed;
    public NativeArray<float> actStSt_RecoveryMotInterpTimer;
    public NativeArray<float3> animDPos;
    public NativeArray<quaternion> animDRot;
    public NativeArray<float> curStDur;
    public NativeArray<bool> groundCastHitSomething;
    public NativeArray<float3> groundCastNrm;
    public NativeArray<float> groundSnapVerDownSpd;
    public NativeArray<int> hp_Cur;
    public NativeArray<int> hp_Max;
    public NativeArray<float2> input_mov;
    /// <summary>
    /// Last nonzero movement input (in world space).
    /// </summary>
    public NativeArray<float2> input_mov_LastNonZero;
    /// <summary>
    /// Movement input during last state switch (in world space).
    /// </summary>
    public NativeArray<float2> input_mov_WhenLastSwitchedSt;
    public NativeArray<bool> input_atk_Light;
    public NativeArray<bool> input_atk_Heavy;
    public NativeArray<bool> input_atk_Ult;
    public NativeArray<bool> input_dodge;
    public NativeArray<BufferableInput> inputBuffer_BufferedInput;
    public NativeArray<float> inputBuffer_RemainingTime;
    public NativeArray<bool> invul;
    public NativeArray<bool> isAffectedByGravity;
    public NativeArray<bool> isGrounded;
    public NativeArray<float3> lastCcVel;
    public NativeArray<float> lastKnockbackStr;
    public NativeArray<float3> lastRecievedHitDir;
    public NativeArray<float2> movInput_tgtHorDir;
    public NativeArray<float3> mov_animRootMot;
    public NativeArray<float> movInput_tgtHorSpd;
    public NativeArray<float> movInput_yawSpd;
    public NativeArray<float> movInput_horAcc;
    // To keep track of which indices are actually used for entitites.
    public NativeArray<bool> occupied; // <- This is important!
    public NativeArray<float> st_AtkHorSlash_Impact_AngSpd;
    public NativeArray<float> st_AtkHorSlash_Windup_MaxAngSpd;
    public NativeArray<float> st_AtkJump_DownSpeedAfterJumpFinished;
    public NativeArray<float> st_Dodge_YawSpd;
    public NativeArray<float> st_Falling_LandingStFallDistThreshold;
    public NativeArray<float> st_Falling_LinAcc;
    public NativeArray<float> st_Falling_MaxLinSpd;
    public NativeArray<float> st_Walk_LinAcc;
    public NativeArray<float> st_Walk_MaxLinSpd;
    public NativeArray<float> st_Walk_YawSpd;
    public NativeArray<float3> trf_lossyScl;
    public NativeArray<float3> trf_pos;
    public NativeArray<quaternion> trf_rot;
    /// <summary>
    /// Current horisontal (XZ) velocity.
    /// </summary>
    public NativeArray<float2> vel_Hor;
    /// <summary>
    /// Current vertical (Y) velocity.
    /// </summary>
    public NativeArray<float> vel_Ver;

    public static Cp_SoaData Create(int capacity) {
        return new Cp_SoaData {
            actStSt_AtkPhase = StructUtils.Alloc<AtkPhase>(capacity),
            actStSt_BufferedInputStSwitchAllowed = StructUtils.Alloc<bool>(capacity),
            actStSt_ComboAllowed = StructUtils.Alloc<bool>(capacity),
            actStSt_DodgeAllowed = StructUtils.Alloc<bool>(capacity),
            actStSt_FallingStartHgt = StructUtils.Alloc<float>(capacity),
            actStSt_ImpactFinished = StructUtils.Alloc<bool>(capacity),
            actStSt_InputRotAllowed = StructUtils.Alloc<bool>(capacity),
            actStSt_RecoveryMotInterpTimer = StructUtils.Alloc<float>(capacity),
            animDPos = StructUtils.Alloc<float3>(capacity),
            animDRot = StructUtils.Alloc<quaternion>(capacity),
            curStDur = StructUtils.Alloc<float>(capacity),
            groundCastHitSomething = StructUtils.Alloc<bool>(capacity),
            groundCastNrm = StructUtils.Alloc<float3>(capacity),
            groundSnapVerDownSpd = StructUtils.Alloc<float>(capacity),
            hp_Cur = StructUtils.Alloc<int>(capacity),
            hp_Max = StructUtils.Alloc<int>(capacity),
            input_mov = StructUtils.Alloc<float2>(capacity),
            input_mov_LastNonZero = StructUtils.Alloc<float2>(capacity),
            input_mov_WhenLastSwitchedSt = StructUtils.Alloc<float2>(capacity),
            input_atk_Light = StructUtils.Alloc<bool>(capacity),
            input_atk_Heavy = StructUtils.Alloc<bool>(capacity),
            input_atk_Ult = StructUtils.Alloc<bool>(capacity),
            input_dodge = StructUtils.Alloc<bool>(capacity),
            inputBuffer_BufferedInput = StructUtils.Alloc<BufferableInput>(capacity),
            inputBuffer_RemainingTime = StructUtils.Alloc<float>(capacity),
            invul = StructUtils.Alloc<bool>(capacity),
            isAffectedByGravity = StructUtils.Alloc<bool>(capacity),
            isGrounded = StructUtils.Alloc<bool>(capacity),
            lastCcVel = StructUtils.Alloc<float3>(capacity),
            lastKnockbackStr = StructUtils.Alloc<float>(capacity),
            lastRecievedHitDir = StructUtils.Alloc<float3>(capacity),
            movInput_tgtHorDir = StructUtils.Alloc<float2>(capacity),
            mov_animRootMot = StructUtils.Alloc<float3>(capacity),
            movInput_tgtHorSpd = StructUtils.Alloc<float>(capacity),
            movInput_yawSpd = StructUtils.Alloc<float>(capacity),
            movInput_horAcc = StructUtils.Alloc<float>(capacity),
            occupied = StructUtils.Alloc<bool>(capacity),
            st_AtkHorSlash_Impact_AngSpd = StructUtils.Alloc<float>(capacity),
            st_AtkHorSlash_Windup_MaxAngSpd = StructUtils.Alloc<float>(capacity),
            st_AtkJump_DownSpeedAfterJumpFinished = StructUtils.Alloc<float>(capacity),
            st_Dodge_YawSpd = StructUtils.Alloc<float>(capacity),
            st_Falling_LandingStFallDistThreshold = StructUtils.Alloc<float>(capacity),
            st_Falling_LinAcc = StructUtils.Alloc<float>(capacity),
            st_Falling_MaxLinSpd = StructUtils.Alloc<float>(capacity),
            st_Walk_LinAcc = StructUtils.Alloc<float>(capacity),
            st_Walk_MaxLinSpd = StructUtils.Alloc<float>(capacity),
            st_Walk_YawSpd = StructUtils.Alloc<float>(capacity),
            trf_pos = StructUtils.Alloc<float3>(capacity),
            trf_rot = StructUtils.Alloc<quaternion>(capacity),
            trf_lossyScl = StructUtils.Alloc<float3>(capacity),
            vel_Hor = StructUtils.Alloc<float2>(capacity),
            vel_Ver = StructUtils.Alloc<float>(capacity),
        };
    }

    public void Dispose() {
        actStSt_AtkPhase.Dispose();
        actStSt_BufferedInputStSwitchAllowed.Dispose();
        actStSt_ComboAllowed.Dispose();
        actStSt_DodgeAllowed.Dispose();
        actStSt_FallingStartHgt.Dispose();
        actStSt_ImpactFinished.Dispose();
        actStSt_InputRotAllowed.Dispose();
        actStSt_RecoveryMotInterpTimer.Dispose();
        animDPos.Dispose();
        animDRot.Dispose();
        curStDur.Dispose();
        groundCastHitSomething.Dispose();
        groundCastNrm.Dispose();
        groundSnapVerDownSpd.Dispose();
        hp_Cur.Dispose();
        hp_Max.Dispose();
        input_mov.Dispose();
        input_mov_LastNonZero.Dispose();
        input_mov_WhenLastSwitchedSt.Dispose();
        input_atk_Light.Dispose();
        input_atk_Heavy.Dispose();
        input_atk_Ult.Dispose();
        input_dodge.Dispose();
        inputBuffer_BufferedInput.Dispose();
        inputBuffer_RemainingTime.Dispose();
        invul.Dispose();
        isAffectedByGravity.Dispose();
        isGrounded.Dispose();
        lastCcVel.Dispose();
        lastKnockbackStr.Dispose();
        lastRecievedHitDir.Dispose();
        movInput_tgtHorDir.Dispose();
        mov_animRootMot.Dispose();
        movInput_tgtHorSpd.Dispose();
        movInput_yawSpd.Dispose();
        movInput_horAcc.Dispose();
        occupied.Dispose();
        st_AtkHorSlash_Impact_AngSpd.Dispose();
        st_AtkHorSlash_Windup_MaxAngSpd.Dispose();
        st_AtkJump_DownSpeedAfterJumpFinished.Dispose();
        st_Dodge_YawSpd.Dispose();
        st_Falling_LandingStFallDistThreshold.Dispose();
        st_Falling_LinAcc.Dispose();
        st_Falling_MaxLinSpd.Dispose();
        st_Walk_LinAcc.Dispose();
        st_Walk_MaxLinSpd.Dispose();
        st_Walk_YawSpd.Dispose();
        trf_lossyScl.Dispose();
        trf_pos.Dispose();
        trf_rot.Dispose();
        vel_Hor.Dispose();
        vel_Ver.Dispose();
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
/// Per capsule pawn data.
/// </summary>
public struct Cp_AosData {
    public bool isSwitchingSt;
    public bool enableDebugMsgs;
}

// TODO: Enemy brain should be its own entity! Also a AoS data layout works better for heavily branching
// TODO C: and reference-dependent behavior tree!
public struct Cp_BrainData {
    public float3 agentDesiredVel;
    public float aggroRange;
    public float atkRange;
    public float distToTgt;
    public bool hasTgt;
    public bool inAggroRange;
    public bool inAtkRange;
    public bool prevCalculatePathSucceeded;
    public float3 tgtPos;
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
    public Transform tgt;
    public Transform trf;
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
    public AtkData atkData;
    public Vector3 hitWldDir;

    public HitData(AtkData atkData, Vector3 hitWldDir) {
        this.atkData = atkData;
        this.hitWldDir = hitWldDir;
    }
}

public struct HitResult {
    // TODO:
    public bool wasInvul;
    public bool wasBlocked;

    public HitResult(bool wasInvul, bool wasBlocked) {
        this.wasInvul = wasInvul;
        this.wasBlocked = wasBlocked;
    }
}

public struct HomingProjMovData {
    public float spd;
    public float maxLifetime;
    public float homingStr;

    public HomingProjMovData(float spd, float maxLifetime, float homingStr) {
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
