// Add all public enums here.

public enum AtkPhase{
    Windup,
    Impact,
    Recovery
}

/// <summary>
/// Animation events used by the capsule character Animator.
/// </summary>
public enum CapsuleCharAnimEvent {
    FlyingAtk_Windup_Finished,
    FlyingAtk_Impact_HitDealerActivated,
    FlyingAtk_Impact_Finished,
    FlyingAtk_Recovery_Finished,
    HorSlash1_Windup_Finished,
    HorSlash1_Impact_RotationAllowed,
    HorSlash1_Impact_RotationDisallowed,
    HorSlash1_Impact_HitDealerActivated,
    HorSlash1_Impact_HitDealerDeactivated,
    HorSlash1_Impact_ComboAllowed,
    HorSlash1_Impact_ComboDisallowed,
    HorSlash1_Impact_Finished,
    HorSlash1_Recovery_DodgeAllowed,
    HorSlash1_Recovery_Finished,
    HorSlash2_Impact_ComboAllowed,
    HorSlash2_Impact_ComboDisallowed,
    HorSlash2_Impact_Finished,
    HorSlash2_Impact_HitDealerActivated,
    HorSlash2_Impact_HitDealerDeactivated,
    HorSlash2_Impact_RotationAllowed,
    HorSlash2_Impact_RotationDisallowed,
    HorSlash2_Recovery_DodgeAllowed,
    HorSlash2_Recovery_Finished,
    HorSlash3_Impact_ComboAllowed,
    HorSlash3_Impact_ComboDisallowed,
    HorSlash3_Impact_Finished,
    HorSlash3_Impact_HitDealerActivated,
    HorSlash3_Impact_HitDealerDeactivated,
    HorSlash3_Impact_RotationAllowed,
    HorSlash3_Impact_RotationDisallowed,
    JumpVerSlam_Finished,
    JumpVerSlam_HitboxActivated,
    JumpVerSlam_HitboxDeactivated,
    JumpVerSlam_JumpFinished,
    JumpVerSlam_JumpStarted,
    Dodge_YawAllowed,
    Dodge_InvulEnd,
    Dodge_BufferedInputStSwitchAllowed,
    Dodge_Finished
}

public enum Directions2DVertical{
    Left,
    Right,
    Down,
    Up,
}

public enum Directions2DHorizontal{
    Left,
    Right,
    Backward,
    Forward,
}

public enum Directions3D{
    Left,
    Right,
    Down,
    Up,
    Backward,
    Forward,
}

public enum Faces{
    Left,
    Right,
    Bottom,
    Top,
    Near,
    Far,
}

public enum LocomotionType {
    VelocityByDirectionalInput,
    DirectMotion,
}

public enum Side{
    Left,
    Right,
}
