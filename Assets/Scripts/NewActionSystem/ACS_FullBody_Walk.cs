using UnityEngine;

public class ACS_FullBody_Walk : ACS_FullBody
{
    public ACS_FullBody_Walk(NewPlayerController playerController) : base(playerController) 
    {
    }

    public override bool CanTransitionTo(ACS_ActionState newAction)
    {
        //switch (newAction)
        //{
        //    case ACS_FullBody_Walk:
        //        return true;
        //    default:
        //        Debug.LogError("Switch defaulted.");
        //        return false;
        //}
        return true;
    }

    public override void EnterState()
    {
        PC.CustomAnimator.CharacterVisualsLayer_FullBody.RequestCrossfadeTo(PC.CustomAnimator.CharacterVisualsLayer_FullBody.Walk);
    }

    public override void ExitState()
    {
    }

    public override void UpdateState(float deltaTime)
    {
        PC.Movement.SolveMovement(PC.MoveInput);
        if (PC.AttackInput)
            PC.RequestFullBodyAction(PC.ACS_FullBody_Attack_JumpVerticalSlam);
        else if (PC.MoveInput == Vector2.zero)
            PC.RequestFullBodyAction(PC.ACS_FullBody_Idle);
    }
}
