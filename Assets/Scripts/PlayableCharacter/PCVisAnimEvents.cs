using System;
using UnityEngine;

// TODO: Since in most animations the events have similar functionality, maybe you could simplify? Now you make state checks in the
// TODO C: callback functions inside the state classes, maybe move that someplace else?

/// <summary>
/// All playable character's animation events are in here.
/// NOTE: Atm all events must be unique and named after their animation!
/// </summary>
public class PCVisAnimEvents : MonoBehaviour
{
    public event Action Atk_HorSlash1_Impact_ComboAllowed;
    public event Action Atk_HorSlash1_Impact_ComboDisallowed;
    public event Action Atk_HorSlash1_Impact_Finished;
    public event Action Atk_HorSlash1_Impact_HitDealerActivated;
    public event Action Atk_HorSlash1_Impact_HitDealerDeactivated;
    public event Action Atk_HorSlash1_Impact_RotationAllowed;
    public event Action Atk_HorSlash1_Impact_RotationDisallowed;

    public event Action Atk_HorSlash1_Recovery_Finished;
    public event Action Atk_HorSlash1_Recovery_DodgeAllowed;

    public event Action Atk_HorSlash1_Windup_Finished;

    public event Action Atk_HorSlash2_Impact_ComboAllowed;
    public event Action Atk_HorSlash2_Impact_ComboDisallowed;
    public event Action Atk_HorSlash2_Impact_Finished;
    public event Action Atk_HorSlash2_Impact_HitDealerActivated;
    public event Action Atk_HorSlash2_Impact_HitDealerDeactivated;
    public event Action Atk_HorSlash2_Impact_RotationAllowed;
    public event Action Atk_HorSlash2_Impact_RotationDisallowed;

    public event Action Atk_HorSlash2_Recovery_Finished;
    public event Action Atk_HorSlash2_Recovery_DodgeAllowed;

    public event Action Atk_HorSlash3_Impact_ComboAllowed;
    public event Action Atk_HorSlash3_Impact_ComboDisallowed;
    public event Action Atk_HorSlash3_Impact_Finished;
    public event Action Atk_HorSlash3_Impact_HitDealerActivated;
    public event Action Atk_HorSlash3_Impact_HitDealerDeactivated;
    public event Action Atk_HorSlash3_Impact_RotationAllowed;
    public event Action Atk_HorSlash3_Impact_RotationDisallowed;

    public event Action Atk_JumpVerSlam_Finished;
    public event Action Atk_JumpVerSlam_HitboxActivated;
    public event Action Atk_JumpVerSlam_HitboxDeactivated;
    public event Action Atk_JumpVerSlam_JumpFinished;
    public event Action Atk_JumpVerSlam_JumpStarted;

    public event Action Dodge_BufferedInputStateSwitchAllowed;
    public event Action Dodge_Finished;
    public event Action Dodge_InvulnerabilityEnd;
    public event Action Dodge_YawAllowed;


    // -----------------------------------------
    // Atk_HorSlash1_Impact
    // -----------------------------------------

    public void E_Atk_HorSlash1_Impact_ComboAllowed()
    {
        Atk_HorSlash1_Impact_ComboAllowed?.Invoke();
    }

    public void E_Atk_HorSlash1_Impact_ComboDisallowed()
    {
        Atk_HorSlash1_Impact_ComboDisallowed?.Invoke();
    }

    public void E_Atk_HorSlash1_Impact_Finished()
    {
        Atk_HorSlash1_Impact_Finished?.Invoke();
    }

    public void E_Atk_HorSlash1_Impact_HitDealerActivated()
    {
        Atk_HorSlash1_Impact_HitDealerActivated?.Invoke();
    }

    public void E_Atk_HorSlash1_Impact_HitDealerDeactivated()
    {
        Atk_HorSlash1_Impact_HitDealerDeactivated?.Invoke();
    }

    public void E_Atk_HorSlash1_Impact_RotationAllowed()
    {
        Atk_HorSlash1_Impact_RotationAllowed?.Invoke();
    }

    public void E_Atk_HorSlash1_Impact_RotationDisallowed()
    {
        Atk_HorSlash1_Impact_RotationDisallowed?.Invoke();
    }

    // -----------------------------------------
    // Atk_HorSlash1_Recovery
    // -----------------------------------------

    public void E_Atk_HorSlash1_Recovery_Finished()
    {
        Atk_HorSlash1_Recovery_Finished?.Invoke();
    }

    public void E_Atk_HorSlash1_Recovery_DodgeAllowed()
    {
        Atk_HorSlash1_Recovery_DodgeAllowed?.Invoke();
    }

    // -----------------------------------------
    // Atk_HorSlash1_Windup
    // -----------------------------------------

    public void E_Atk_HorSlash1_Windup_Finished()
    {
        Atk_HorSlash1_Windup_Finished?.Invoke();
    }

    // -----------------------------------------
    // Atk_HorSlash2_Impact
    // -----------------------------------------

    public void E_Atk_HorSlash2_Impact_ComboAllowed()
    {
        Atk_HorSlash2_Impact_ComboAllowed?.Invoke();
    }

    public void E_Atk_HorSlash2_Impact_ComboDisallowed()
    {
        Atk_HorSlash2_Impact_ComboDisallowed?.Invoke();
    }

    public void E_Atk_HorSlash2_Impact_Finished()
    {
        Atk_HorSlash2_Impact_Finished?.Invoke();
    }

    public void E_Atk_HorSlash2_Impact_HitDealerActivated()
    {
        Atk_HorSlash2_Impact_HitDealerActivated?.Invoke();
    }

    public void E_Atk_HorSlash2_Impact_HitDealerDeactivated()
    {
        Atk_HorSlash2_Impact_HitDealerDeactivated?.Invoke();
    }

    public void E_Atk_HorSlash2_Impact_RotationAllowed()
    {
        Atk_HorSlash2_Impact_RotationAllowed?.Invoke();
    }

    public void E_Atk_HorSlash2_Impact_RotationDisallowed()
    {
        Atk_HorSlash2_Impact_RotationDisallowed?.Invoke();
    }

    // -----------------------------------------
    // Atk_HorSlash2_Recovery
    // -----------------------------------------

    public void E_Atk_HorSlash2_Recovery_Finished()
    {
        Atk_HorSlash2_Recovery_Finished?.Invoke();
    }

    public void E_Atk_HorSlash2_Recovery_DodgeAllowed()
    {
        Atk_HorSlash2_Recovery_DodgeAllowed?.Invoke();
    }

    // -----------------------------------------
    // Atk_HorSlash3_Impact
    // -----------------------------------------

    public void E_Atk_HorSlash3_Impact_ComboAllowed()
    {
        Atk_HorSlash3_Impact_ComboAllowed?.Invoke();
    }

    public void E_Atk_HorSlash3_Impact_ComboDisallowed()
    {
        Atk_HorSlash3_Impact_ComboDisallowed?.Invoke();
    }

    public void E_Atk_HorSlash3_Impact_Finished()
    {
        Atk_HorSlash3_Impact_Finished?.Invoke();
    }

    public void E_Atk_HorSlash3_Impact_HitDealerActivated()
    {
        Atk_HorSlash3_Impact_HitDealerActivated?.Invoke();
    }

    public void E_Atk_HorSlash3_Impact_HitDealerDeactivated()
    {
        Atk_HorSlash3_Impact_HitDealerDeactivated?.Invoke();
    }

    public void E_Atk_HorSlash3_Impact_RotationAllowed()
    {
        Atk_HorSlash3_Impact_RotationAllowed?.Invoke();
    }
    
    public void E_Atk_HorSlash3_Impact_RotationDisallowed()
    {
        Atk_HorSlash3_Impact_RotationDisallowed?.Invoke();
    }

    // -----------------------------------------
    // Atk_JumpVerSlam
    // -----------------------------------------

    public void E_Attack_RHandJumpVerticalSlam_Finished()
    {
        Atk_JumpVerSlam_Finished?.Invoke();
    }

    public void E_Attack_RHandJumpVerticalSlam_HitboxActivated()
    {
        Atk_JumpVerSlam_HitboxActivated?.Invoke();
    }

    public void E_Attack_RHandJumpVerticalSlam_HitboxDeactivated()
    {
        Atk_JumpVerSlam_HitboxDeactivated?.Invoke();
    }

    public void E_Attack_RHandJumpVerticalSlam_JumpFinished()
    {
        Atk_JumpVerSlam_JumpFinished?.Invoke();
    }

    public void E_Attack_RHandJumpVerticalSlam_JumpStarted()
    {
        Atk_JumpVerSlam_JumpStarted?.Invoke();
    }

    // -----------------------------------------
    // Dodge
    // -----------------------------------------

    public void E_Dodge_BufferedInputStateSwitchAllowed()
    {
        Dodge_BufferedInputStateSwitchAllowed?.Invoke();
    }

    public void E_Dodge_Finished()
    {
        Dodge_Finished?.Invoke();
    }

    public void E_Dodge_InvulnerabilityEnd()
    {
        Dodge_InvulnerabilityEnd?.Invoke();
    }

    public void E_Dodge_YawAllowed()
    {
        Dodge_YawAllowed?.Invoke();
    }
}
