
using UnityEngine;

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

    public static void SwitchToLightAtkSt(Pc cc) {
        switch (cc.Data.equip_RHandEquippable) {
            case HandEquippableT.Empty:
                cc.fsm.SwitchSt(cc.fsmSts.atk_HorSlash1);
                break;
            case HandEquippableT.Sword:
                cc.fsm.SwitchSt(cc.fsmSts.atk_HorSlash1);
                break;
            case HandEquippableT.Hammer:
                cc.fsm.SwitchSt(cc.fsmSts.atk_HorSlash1);
                break;
            case HandEquippableT.Pistol:
                cc.fsm.SwitchSt(cc.fsmSts.atk_ShootHomingProj);
                break;
            default:
                Debug.LogError($"Switch defaulted with {cc.Data.equip_RHandEquippable}");
                break;
        }
    }
}
