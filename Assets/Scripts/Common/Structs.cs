using System;

/// <summary>
/// Grouped Animator state info used by <see cref="AnimEventPlr"/>
/// </summary>
public struct AnimInfo {
    public int shortNameHash;
    public int animLayer;
    /// <summary>
    /// Does the animation loop?
    /// </summary>
    public bool looping;
    public int lastFrame;

    public AnimInfo(
        int shortNameHash,
        int animLayer,
        bool looping,
        int lastFrame
    ) {
        this.shortNameHash = shortNameHash;
        this.animLayer = animLayer;
        this.looping = looping;
        this.lastFrame = lastFrame;
    }
}

/// <summary>
/// Animation event decoupled from the Animator.
/// </summary>
[Serializable]
public struct ActAnimEvent {
    /// <summary>
    /// Normalized time of the event during one animation loop.
    /// </summary>
    public float nrmT;
    /// <summary>
    /// Unique name for the action event, used to check against a switch case.
    /// </summary>
    public string id;

    /// <param name="frame">
    /// Frame of the animation event.<br/>
    /// NOTE: Unity's first animation frame has an index of 0.
    /// </param>
    /// <param name="lastFrame">
    /// Index of the last frame of the animation. In the Animation window, this is the frame
    /// on the timeline where the animation bar changes to dark grey.
    /// </param>
    /// <param name="id">Unique name for the action event, used to check against a switch case.</param>
    public ActAnimEvent(int frame, int lastFrame, string id) {
        nrmT = frame / (float)lastFrame;
        this.id = id;
    }
}

/// <summary>
/// Used for SoA type of handling of capsule character data.
/// </summary>
public struct CapsuleCharacterData {
    public float gravitationalAcc;
    public float inputBufferDuration;
    public float st_AtkJump_DownSpeedAfterJumpFinished;
    public float st_AtkHorSlash_RecoveryMotionInterpDur;
    public float st_AtkHorSlash_Impact_AngSpd;
    public float st_Dodge_YawAngSpd;
    public float st_Walk_LinAcc;
    public float st_Walk_MaxLinSpd;
    public float st_Walk_MaxAngSpd;
}