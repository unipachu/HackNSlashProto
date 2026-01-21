using UnityEngine;

public class ANS_CapsuleCharacter_Idle : ANS_AnimatorState
{
    public ANS_CapsuleCharacter_Idle(Animator animator, int animatorLayer)
        : base(animator, nameof(ANS_CapsuleCharacter_Idle), animatorLayer, 90)
    {
    }

    public override void UpdateState(float activeStateClampedNormalizedTime, float previousActiveStateClampedNormalizedTime)
    {
    }
}
