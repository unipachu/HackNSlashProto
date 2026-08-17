using System;
using UnityEngine;

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
        params (int frame, CapsuleCharAnimEvent id)[] sortedEvents
    ) {
        this.shortNameHash = shortNameHash;
        this.animLayer = animLayer;
        this.looping = looping;
        this.lastFrame = lastFrame;
        // NOTE: We cannot automaticize sorting since some events might happen on the same frame and yet
        // their order matters.
        // TODO: However we could make an Assert etc to make sure they are at least in ascending order.
        sortedAnimEvents = new AnimEvent[sortedEvents.Length];
        for (int i = 0; i < sortedEvents.Length; i++)
            sortedAnimEvents[i] = new AnimEvent(sortedEvents[i].frame, lastFrame, sortedEvents[i].id);
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
    public CapsuleCharAnimEvent id;

    /// <param name="frame">
    /// Frame of the animation event.<br/>
    /// NOTE: Unity's first animation frame has an index of 0.
    /// </param>
    /// <param name="lastFrame">
    /// Index of the last frame of the animation. In the Animation window, this is the frame
    /// on the timeline where the animation bar changes to dark grey.
    /// </param>
    /// <param name="id">Unique name for the action event, used to check against a switch case.</param>
    public AnimEvent(int frame, int lastFrame, CapsuleCharAnimEvent id) {
        nrmT = frame / (float)lastFrame;
        this.id = id;
    }
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

/// <summary>
/// Used for SoA type of handling of capsule character data.
/// </summary>
public struct CapsuleCharacterData {
    public float gravitationalAcc;
    public float inputBufferDuration;
    public float maxHp;
    public float curHp;
    public bool invul;
    public Vector3 lastRecievedHitDir;
    public float lastKnockbackStr;
    public float st_AtkJump_DownSpeedAfterJumpFinished;
    public float st_AtkHorSlash_RecoveryMotionInterpDur;
    public float st_AtkHorSlash_Impact_AngSpd;
    public float st_Dodge_YawAngSpd;
    public float st_Walk_LinAcc;
    public float st_Walk_MaxLinSpd;
    public float st_Walk_MaxAngSpd;
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