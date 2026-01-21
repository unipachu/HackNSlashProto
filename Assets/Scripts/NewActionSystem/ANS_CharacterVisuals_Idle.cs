using UnityEngine;

public class ANS_CharacterVisuals_Idle : ANS_AnimatorState
{
    public ANS_CharacterVisuals_Idle(Animator animator, int animatorLayer)
        : base(animator, nameof(ANS_CharacterVisuals_Attack_JumpVerticalSlam), animatorLayer, 90)
    {
    }

    public override void UpdateState(float activeStateClampedNormalizedTime, float previousActiveStateClampedNormalizedTime)
    {
    }
}
