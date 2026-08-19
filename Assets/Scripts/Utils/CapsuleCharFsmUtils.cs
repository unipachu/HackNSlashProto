/// <summary>
/// Capsule charater's state machine related utility methods (e.g. state change condition checks).
/// </summary>
public static class CapsuleCharFsmUtils{
    /// <summary>
    /// True if switched.
    /// </summary>
    public static bool SwitchToFallingStIfNotGrounded(Pc pc) {
        if (!pc.Data.isGrounded
            && pc.fsm.CurSt != (IFsmSt)pc.fsmSts.falling
            && pc.fsm.CurSt.CanSwitchStTo(pc.fsmSts.falling)
        ) {
            pc.fsm.SwitchSt(pc.fsmSts.falling);
            return true;
        }
        return false;
    }
}
