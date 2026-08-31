// TODO: Check if some ints can be converted to short or byte.
using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Grouped Animator state info used by <see cref="AnimEventPlr"/>.<br/>
/// NOTE: <see cref=""/> 
/// </summary>
public struct AnimInfo {
    public int shortNameHash;
    public int animLayer;
    /// <summary>
    /// Does the animation loop?
    /// </summary>
    public bool looping;
    public int lastFrame;
    // TODO MINOR: This struct cannot be used in native arrays because of this managed array. This could
    // TODO MINOR C: be fixed by either making this array a fixed list - meaning more memory overhead. Or
    // TODO MINOR C: possibly better option: have all anim event data in a list and in here have an index
    // TODO MINOR C: to the first and the last events. But both of these options seem more complicated than
    // TODO MINOR C: this and I'm already running out of time...
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
        // TODO: However we could make an Assert etc to make sure they are at least in ascending order.
        sortedAnimEvents = new AnimEvent[sortedEvents.Length];
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

public struct Cp_BaseData {
    public NativeArray<CpActSt> actSt;
    public NativeArray<AtkPhase> actStSt_AtkPhase;
    public NativeArray<bool> actStSt_BufferedInputStSwitchAllowed;
    public NativeArray<bool> actStSt_ComboAllowed;
    public NativeArray<bool> actStSt_DodgeAllowed;
    public NativeArray<float> actStSt_FallingStartHgt;
    public NativeArray<bool> actStSt_ImpactFinished;
    public NativeArray<bool> actStSt_ImpactInputRotAllowed;
    public NativeArray<float> actStSt_RecoveryMotInterpTimer;
    public NativeArray<float3> animDPos;
    public NativeArray<quaternion> animDRot;
    public NativeArray<float> curStDur;
    public NativeArray<bool> enableDebugMsgs;
    public NativeArray<HandEquippableT> equip_RHandEquippable;
    public NativeArray<float> gravitationalAcc;
    public NativeArray<bool> groundCastHitSomething;
    public NativeArray<float3> groundCastNrm;
    public NativeArray<float> groundSnapVerDownSpd;
    // TODO: make these int.
    public NativeArray<float> hp_Cur;
    public NativeArray<float> hp_Max;
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
    public NativeArray<bool> isSwitchingActSt;
    public NativeArray<float3> lastCcVel;
    public NativeArray<float> lastKnockbackStr;
    public NativeArray<float3> lastRecievedHitDir;
    public NativeArray<float> maxFallSpd;
    public NativeArray<float2> mov_horMov;
    public NativeArray<float3> mov_animRootMot;
    public NativeArray<float> mov_maxLinSpd;
    public NativeArray<float> mov_yawSpd;
    // TODO MINOR: Should this be called hor acc instead?
    public NativeArray<float> mov_linAcc;
    // To keep track of which indices are actually used for entitites.
    public NativeArray<bool> occupied; // <- This is important!
    public NativeArray<CpActSt> prevSt;
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
    // TODO: Maybe you don't need these since you have the mov_ arrays?
    public NativeArray<float2> vel_Hor;
    public NativeArray<float> vel_Ver;
    public NativeArray<float> vel_Yaw;

    public static Cp_BaseData Create(int capacity) {
        return new Cp_BaseData {
            actSt = StructUtils.Alloc<CpActSt>(capacity),
            actStSt_AtkPhase = StructUtils.Alloc<AtkPhase>(capacity),
            actStSt_BufferedInputStSwitchAllowed = StructUtils.Alloc<bool>(capacity),
            actStSt_ComboAllowed = StructUtils.Alloc<bool>(capacity),
            actStSt_DodgeAllowed = StructUtils.Alloc<bool>(capacity),
            actStSt_FallingStartHgt = StructUtils.Alloc<float>(capacity),
            actStSt_ImpactFinished = StructUtils.Alloc<bool>(capacity),
            actStSt_ImpactInputRotAllowed = StructUtils.Alloc<bool>(capacity),
            actStSt_RecoveryMotInterpTimer = StructUtils.Alloc<float>(capacity),
            animDPos = StructUtils.Alloc<float3>(capacity),
            animDRot = StructUtils.Alloc<quaternion>(capacity),
            curStDur = StructUtils.Alloc<float>(capacity),
            enableDebugMsgs = StructUtils.Alloc<bool>(capacity),
            equip_RHandEquippable = StructUtils.Alloc<HandEquippableT>(capacity),
            gravitationalAcc = StructUtils.Alloc<float>(capacity),
            groundCastHitSomething = StructUtils.Alloc<bool>(capacity),
            groundCastNrm = StructUtils.Alloc<float3>(capacity),
            groundSnapVerDownSpd = StructUtils.Alloc<float>(capacity),
            hp_Cur = StructUtils.Alloc<float>(capacity),
            hp_Max = StructUtils.Alloc<float>(capacity),
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
            isSwitchingActSt = StructUtils.Alloc<bool>(capacity),
            lastCcVel = StructUtils.Alloc<float3>(capacity),
            lastKnockbackStr = StructUtils.Alloc<float>(capacity),
            lastRecievedHitDir = StructUtils.Alloc<float3>(capacity),
            maxFallSpd = StructUtils.Alloc<float>(capacity),
            mov_horMov = StructUtils.Alloc<float2>(capacity),
            mov_animRootMot = StructUtils.Alloc<float3>(capacity),
            mov_maxLinSpd = StructUtils.Alloc<float>(capacity),
            mov_yawSpd = StructUtils.Alloc<float>(capacity),
            mov_linAcc = StructUtils.Alloc<float>(capacity),
            occupied = StructUtils.Alloc<bool>(capacity),
            prevSt = StructUtils.Alloc<CpActSt>(capacity),
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
            vel_Yaw = StructUtils.Alloc<float>(capacity)
        };
    }

    public void Dispose() {
        actSt.Dispose();
        actStSt_AtkPhase.Dispose();
        actStSt_BufferedInputStSwitchAllowed.Dispose();
        actStSt_ComboAllowed.Dispose();
        actStSt_DodgeAllowed.Dispose();
        actStSt_FallingStartHgt.Dispose();
        actStSt_ImpactFinished.Dispose();
        actStSt_ImpactInputRotAllowed.Dispose();
        actStSt_RecoveryMotInterpTimer.Dispose();
        animDPos.Dispose();
        animDRot.Dispose();
        curStDur.Dispose();
        enableDebugMsgs.Dispose();
        equip_RHandEquippable.Dispose();
        gravitationalAcc.Dispose();
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
        isSwitchingActSt.Dispose();
        lastCcVel.Dispose();
        lastKnockbackStr.Dispose();
        lastRecievedHitDir.Dispose();
        maxFallSpd.Dispose();
        mov_horMov.Dispose();
        mov_animRootMot.Dispose();
        mov_maxLinSpd.Dispose();
        mov_yawSpd.Dispose();
        mov_linAcc.Dispose();
        occupied.Dispose();
        prevSt.Dispose();
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
        vel_Yaw.Dispose();
    }
}

public struct Cp_BrainData {
    public NativeArray<float3> agentDesiredVel;
    public NativeArray<float> aggroRange;
    public NativeArray<float> atkRange;
    public NativeArray<float> distToTgt;
    public NativeArray<bool> hasTgt;
    public NativeArray<bool> inAggroRange;
    public NativeArray<bool> inAtkRange;
    public NativeArray<bool> prevCalculatePathSucceeded;
    public NativeArray<float3> tgtPos;

    public static Cp_BrainData Create(int capacity) {
        return new Cp_BrainData {
            agentDesiredVel = StructUtils.Alloc<float3>(capacity),
            aggroRange = StructUtils.Alloc<float>(capacity),
            atkRange = StructUtils.Alloc<float>(capacity),
            distToTgt = StructUtils.Alloc<float>(capacity),
            hasTgt = StructUtils.Alloc<bool>(capacity),
            inAggroRange = StructUtils.Alloc<bool>(capacity),
            inAtkRange = StructUtils.Alloc<bool>(capacity),
            prevCalculatePathSucceeded = StructUtils.Alloc<bool>(capacity),
            tgtPos = StructUtils.Alloc<float3>(capacity)
        };
    }

    public void Dispose() {
        agentDesiredVel.Dispose();
        aggroRange.Dispose();
        atkRange.Dispose();
        distToTgt.Dispose();
        hasTgt.Dispose();
        inAggroRange.Dispose();
        inAtkRange.Dispose();
        prevCalculatePathSucceeded.Dispose();
        tgtPos.Dispose();
    }
}

[Serializable]
public struct Cp_UnityComps {
    public Transform trf;
    public CpCtrl cpCtrl;
    public Animator anim;
    public CpHitRecieveHandler hitRecieverHandler;
    public CharacterController cc;
    public Transform tgt;
    public NavMeshAgent navMeshAgent;
    public Transform rHand;
    public CpAnimEventHandler animEventHandler;
    [HideInInspector] public HandEquippable rHandEquippable;
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

public struct HandEquippableData {
    public HandEquippableT type;
    public int lightAtkActId;
    public int heavyAtkActId;
    public int ultAtkActId;
}
