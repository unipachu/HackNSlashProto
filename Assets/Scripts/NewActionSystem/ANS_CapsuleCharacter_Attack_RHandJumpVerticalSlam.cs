using UnityEngine;

public class ANS_CapsuleCharacter_Attack_RHandJumpVerticalSlam : ANS_AnimatorState, IHitboxActivatingAnimation, IExitTimeAnimation
{
    ANR_AnimationRange _hitBoxActive;
    // TODO: Rename. Or figure a better way to do this.
    bool _hitBoxActuallyActiveBecauseThisIsSoStupid;

    public ANS_CapsuleCharacter_Attack_RHandJumpVerticalSlam(Animator animator, int animatorLayer) 
        : base(animator, nameof(ANS_CapsuleCharacter_Attack_RHandJumpVerticalSlam), animatorLayer, 122)
    {
        _hitBoxActive = new(
            GeneralUtils.FrameToNormalizedTime(76, LastFrameIndex),
            GeneralUtils.FrameToNormalizedTime(87, LastFrameIndex),
            true,
            true);
    }

    public override void UpdateState(float activeStateClampedNormalizedTime, float previousActiveStateClampedNormalizedTime)
    {
        _hitBoxActive.InvokeActionIfInRange(activeStateClampedNormalizedTime, previousActiveStateClampedNormalizedTime);
        _hitBoxActuallyActiveBecauseThisIsSoStupid = _hitBoxActive.IsInRange(activeStateClampedNormalizedTime, previousActiveStateClampedNormalizedTime);
    }

    public bool IsHitboxActive()
    {
        return _hitBoxActuallyActiveBecauseThisIsSoStupid;
    }

    // TODO: Honestly you could count this only once when instantiating the class.
    public float NormalizedExitTime()
    {
        // TODO: Maybe make the sample rate a field...
        // TODO: You probably want to save this method someplace. NOTE: 1 - because the util method returns remaining time.
        return 1 - GeneralUtils.TimeUntilAnimationEndToNormalizedTime(0.1f, LastFrameIndex, 60);
    }
}
