using UnityEngine;

public class ACS_FullBody_Idle : ACS_FullBody
{
    public ACS_FullBody_Idle(NewPlayerController pC) : base(pC)
    {
    }

    public override bool CanTransitionTo(ACS_ActionState newAction)
    {
        return true;
    }

    public override void EnterState()
    {
        PC.CustomAnimator.CharacterVisualsLayer_FullBody.RequestCrossfadeTo(PC.CustomAnimator.CharacterVisualsLayer_FullBody.Idle);
    }

    public override void ExitState()
    {
    }

    public override void UpdateState(float deltaTime)
    {
        PC.Movement.SolveMovement(Vector2.zero);
        if (PC.AttackInput)
            PC.RequestFullBodyAction(PC.ACS_FullBody_Attack_JumpVerticalSlam);
        else if (PC.MoveInput != Vector2.zero)
            PC.RequestFullBodyAction(PC.ACS_FullBody_Walk);
    }
}
