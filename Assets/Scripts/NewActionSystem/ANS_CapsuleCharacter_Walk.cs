using UnityEngine;

public class ANS_CapsuleCharacter_Walk : ANS_AnimatorState
{
    public ANS_CapsuleCharacter_Walk(Animator animator, int animatorLayer)
        : base(animator, nameof(ANS_CapsuleCharacter_Walk), animatorLayer, 20)
    {
    }

    public override void UpdateState(float activeStateClampedNormalizedTime, float previousActiveStateClampedNormalizedTime)
    {
    }
}
