using UnityEngine;

public class ANS_CapsuleCharacter_KnockBack_Backward : ANS_AnimatorState
{
    public ANS_CapsuleCharacter_KnockBack_Backward(Animator animator, int animatorLayer)
        : base(animator, nameof(ANS_CapsuleCharacter_KnockBack_Backward), animatorLayer, 30)
    {
    }

    public override void UpdateState(float activeStateClampedNormalizedTime, float previousActiveStateClampedNormalizedTime)
    {
    }
}
