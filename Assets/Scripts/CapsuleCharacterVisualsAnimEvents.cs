using System;
using UnityEngine;

/// <summary>
/// All capsule character animation events in here
/// NOTE: Áll events must be unique and named after their animation!
/// </summary>
public class CapsuleCharacterVisualsAnimEvents : MonoBehaviour
{
    public event Action AttackRHandJumpVerticalSlam_JumpStarted;
    public event Action AttackRHandJumpVerticalSlam_JumpFinished;

    public void Attack_RHandJumpVerticalSlam_JumpStarted()
    {
        AttackRHandJumpVerticalSlam_JumpStarted?.Invoke();
    }

    public void Attack_RHandJumpVerticalSlam_JumpFinished()
    {
        AttackRHandJumpVerticalSlam_JumpFinished?.Invoke();
    }
}
