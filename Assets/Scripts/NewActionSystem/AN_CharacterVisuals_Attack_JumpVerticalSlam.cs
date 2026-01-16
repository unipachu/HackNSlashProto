using UnityEngine;

public class AN_CharacterVisuals_Attack_JumpVerticalSlam : AN_NewAnimationData
{
    public readonly AR_NewAnimationRangeData HitBoxActive;

    public AN_CharacterVisuals_Attack_JumpVerticalSlam(Animator animator, int animatorLayer) 
        : base(animator, nameof(AN_CharacterVisuals_Attack_JumpVerticalSlam), animatorLayer, 122)
    {
        HitBoxActive = new(
        GeneralUtils.FrameToNormalizedTime(76, LastFrameIndex),
        GeneralUtils.FrameToNormalizedTime(87, LastFrameIndex),
        true);
    }
}
