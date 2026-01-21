using UnityEngine;

public class ANL_CharacterVisuals_FullBody : ANL_AnimatorLayer
{
    // TODO: Perhaps it would be the best to simply create a factory-like system that simply creates the type of animator state that
    // TODO C: is currently needed, and as it is created, we check if the animator actually has it on the specified layer log error if it doesn't.
    // TODO C: Or perhaps even... Do not log anything since animator will log error if no state of that name exist. It will be more error prone,
    // TODO C: but so is the nature of Unity's Animator.
    public ANS_CharacterVisuals_Attack_JumpVerticalSlam Attack_JumpVerticalSlam { get; }
    public ANS_CharacterVisuals_Attack_RHorSwing_0 Attack_RHorSwing_0 { get; }
    public ANS_CharacterVisuals_Attack_RHorSwing_1 Attack_RHorSwing_1 { get; }
    public ANS_CharacterVisuals_Attack_RVerSlam Attack_RVerSlam { get; }
    public ANS_CharacterVisuals_Idle Idle { get; }
    public ANS_CharacterVisuals_KnockBack_Backward KnockBack_Backward { get; }
    public ANS_CharacterVisuals_Walk Walk { get; }

    public ANL_CharacterVisuals_FullBody(int layerIndex, Animator animator) : base(layerIndex, animator, new ANS_CharacterVisuals_Idle(animator, layerIndex))
    {
        Attack_JumpVerticalSlam = new(animator, layerIndex);
        Attack_RHorSwing_0 = new(animator, layerIndex);
        Attack_RHorSwing_1 = new(Animator, layerIndex);
        Attack_RVerSlam = new(Animator, layerIndex);
        Idle = InitialState as ANS_CharacterVisuals_Idle;
        KnockBack_Backward = new(animator, layerIndex);
        Walk = new(animator, layerIndex);
    }

    public override void UpdateLayer()
    {
        ActiveState.UpdateState(
            GetActiveStateClampedNormalizedTime(),
            GetPreviousClampedNormalizedTime());

        base.UpdateLayer();
    }
}
