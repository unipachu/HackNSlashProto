// Add all public enums here.

public enum AtkPhase : byte {
    Windup,
    Impact,
    Recovery
}

public enum BufferableInput : byte {
    None,
    RShldr,
    RTrg,
    LShldr,
    BtnE
}

// TODO: remove.
///// <summary>
///// Capsule pawn action state. These pair animations and animation events with gameplay logic.
///// </summary>
//public enum CpActSt : byte {
//    Atk_FlyingAtk,
//    // TODO: start indexing from 0, ugh.
//    Atk_HorSlash1,
//    Atk_HorSlash2,
//    Atk_HorSlash3,
//    Atk_Jump,
//    Atk_ShootHomingProj,
//    Dodge,
//    Falling,
//    FallLanding,
//    Idle,
//    Knockback_Weak,
//    Walk
//}

/// <summary>
/// Animation events used by the capsule pawn Animator.
/// </summary>
public enum CpAnimEventT {
    // NOTE: This doesn't mean that the character is grounded, only that the animation reached the point
    // NOTE C: where the vertical movement is no more controlled by the animation. (6.9.2026)
    AirtimeEnded,
    AirtimeStarted,
    BufferedInputStSwitchAllowed,
    ComboAllowed,
    ComboDisallowed,
    DodgeAllowed,
    Finished,
    HitDealerActivated,
    HitDealerDeactivated,
    InvulEnd,
    YawDisallowed,
    YawAllowed,
}

public enum BtNodeT : byte {
    Cmd_Atk1,
    Cmd_Idle,
    Cmd_MovToTgt,
    Cond_InAggroRange,
    Cond_InAtkRange,
    Selector,
    Sequence,
}

public enum BtResult : byte {
    Success,
    Failure,
    Running,
}

public enum ComboNodeT : byte {
    BasicImpact,
    BasicRecovery,
    BasicWindup,
}

public enum Directions2DVertical : byte {
    Left,
    Right,
    Down,
    Up,
}

public enum Directions2DHorizontal : byte {
    Left,
    Right,
    Backward,
    Forward,
}

public enum Directions3D : byte {
    Left,
    Right,
    Down,
    Up,
    Backward,
    Forward,
}

public enum BtNodeMethodCall : byte {
    None,
    Cond_IsInAggroRange,
    Cond_IsInAtkRange,
    Cmd_Atk,
}

public enum Faces : byte {
    Left,
    Right,
    Bottom,
    Top,
    Near,
    Far,
}

/// <summary>
/// Represent the type of data a hand item uses (and what handle and database it uses).
/// </summary>
public enum HandItemDataT : byte {
    MeleeWeapon,
    Gun,
}

public enum KnockbackT : byte {
    None,
    Weak,
    Strong
}

public enum LocomotionType : byte {
    VelocityByDirectionalInput,
    DirectMotion,
}

public enum Side : byte {
    Left,
    Right,
}
