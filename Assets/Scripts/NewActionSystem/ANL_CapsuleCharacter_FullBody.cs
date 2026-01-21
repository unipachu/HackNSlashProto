using UnityEngine;

public class ANL_CapsuleCharacter_FullBody : ANL_AnimatorLayer
{
    // TODO: Perhaps it would be the best to simply create a factory-like system that simply creates the type of animator state that
    // TODO C: is currently needed, and as it is created, we check if the animator actually has it on the specified layer log error if it doesn't.
    // TODO C: Or perhaps even... Do not log anything since animator will log error if no state of that name exist. It will be more error prone,
    // TODO C: but so is the nature of Unity's Animator.
    public ANS_CapsuleCharacter_Attack_RHandJumpVerticalSlam Attack_JumpVerticalSlam { get; }
    //public ANS_CapsuleCharacter_Attack_RHorSwing_0 Attack_RHorSwing_0 { get; }
    //public ANS_CapsuleCharacter_Attack_RHorSwing_1 Attack_RHorSwing_1 { get; }
    public ANS_CapsuleCharacter_Attack_RHandVerticalSlam Attack_RVerSlam { get; }
    public ANS_CapsuleCharacter_Idle Idle { get; }
    //public ANS_CapsuleCharacter_KnockBack_Backward KnockBack_Backward { get; }
    public ANS_CapsuleCharacter_Walk Walk { get; }

    public ANL_CapsuleCharacter_FullBody(int layerIndex, Animator animator) : base(layerIndex, animator, new ANS_CapsuleCharacter_Idle(animator, layerIndex))
    {
        Attack_JumpVerticalSlam = new(animator, layerIndex);
        //Attack_RHorSwing_0 = new(animator, layerIndex);
        //Attack_RHorSwing_1 = new(Animator, layerIndex);
        Attack_RVerSlam = new(Animator, layerIndex);
        Idle = InitialState as ANS_CapsuleCharacter_Idle;
        //KnockBack_Backward = new(animator, layerIndex);
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
