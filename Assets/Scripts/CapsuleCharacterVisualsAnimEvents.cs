using System;
using UnityEngine;

/// <summary>
/// All capsule character animation events in here
/// NOTE: All events must be unique and named after their animation!
/// </summary>
public class CapsuleCharacterVisualsAnimEvents : MonoBehaviour
{
    public event Action Attack_RHandJumpVerticalSlam_Finished;
    public event Action Attack_RHandJumpVerticalSlam_HitboxActivated;
    public event Action Attack_RHandJumpVerticalSlam_HitboxDeactivated;
    public event Action Attack_RHandJumpVerticalSlam_JumpFinished;
    public event Action Attack_RHandJumpVerticalSlam_JumpStarted;

    // -----------------------------------------
    // Attack_RHandJumpVerticalSlam
    // -----------------------------------------

    public void E_Attack_RHandJumpVerticalSlam_Finished()
    {
        Attack_RHandJumpVerticalSlam_Finished?.Invoke();
    }

    public void E_Attack_RHandJumpVerticalSlam_HitboxActivated()
    {
        Attack_RHandJumpVerticalSlam_HitboxActivated?.Invoke();
    }

    public void E_Attack_RHandJumpVerticalSlam_HitboxDeactivated()
    {
        Attack_RHandJumpVerticalSlam_HitboxDeactivated?.Invoke();
    }

    public void E_Attack_RHandJumpVerticalSlam_JumpFinished()
    {
        Attack_RHandJumpVerticalSlam_JumpFinished?.Invoke();
    }

    public void E_Attack_RHandJumpVerticalSlam_JumpStarted()
    {
        Attack_RHandJumpVerticalSlam_JumpStarted?.Invoke();
    }


}
