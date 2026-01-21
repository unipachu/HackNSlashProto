using UnityEngine;

public class ANS_CharacterVisuals_Attack_RVerSlam : ANS_AnimatorState
{
    public readonly ANR_AnimationRange HitBoxActive;

    public ANS_CharacterVisuals_Attack_RVerSlam(Animator animator, int animatorLayer)
        : base(animator, nameof(ANS_CharacterVisuals_Attack_RVerSlam), animatorLayer, 106)
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
