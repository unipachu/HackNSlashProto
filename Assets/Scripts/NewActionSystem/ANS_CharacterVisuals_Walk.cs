using UnityEngine;

public class ANS_CharacterVisuals_Walk : ANS_AnimatorState
{
    public ANS_CharacterVisuals_Walk(Animator animator, int animatorLayer)
        : base(animator, nameof(ANS_CharacterVisuals_Walk), animatorLayer, 20)
    {
    }

    public override void UpdateState(float activeStateClampedNormalizedTime, float previousActiveStateClampedNormalizedTime)
    {
    }
}
