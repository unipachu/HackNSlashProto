using UnityEngine;

public class ANS_CapsuleCharacter_Attack_RHorSwing_0 : ANS_AnimatorState
{
    public readonly ANR_AnimationRange HitBoxActive;

    public ANS_CapsuleCharacter_Attack_RHorSwing_0(Animator animator, int animatorLayer)
        : base(animator, nameof(ANS_CapsuleCharacter_Attack_RHorSwing_0), animatorLayer, 40)
    {
        HitBoxActive = new(
            GeneralUtils.FrameToNormalizedTime(9, LastFrameIndex),
            GeneralUtils.FrameToNormalizedTime(21, LastFrameIndex),
            true,
            true);
    }

    public override void UpdateState(float activeStateClampedNormalizedTime, float previousActiveStateClampedNormalizedTime)
    {
        HitBoxActive.InvokeActionIfInRange(activeStateClampedNormalizedTime, previousActiveStateClampedNormalizedTime);
    }
}
