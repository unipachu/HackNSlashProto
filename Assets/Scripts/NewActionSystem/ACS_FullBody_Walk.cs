using UnityEngine;

public class ACS_FullBody_Walk : ACS_FullBody
{
    public ACS_FullBody_Walk(IPawn pawn) : base(pawn) 
    {
    }

    public override void BufferAction(ACS_ActionState newAction)
    {
        throw new System.NotImplementedException();
    }

    public override bool CanBuffer(ACS_ActionState newAction)
    {
        return false;
    }

    public override bool CanInstantlyTransitionTo(ACS_ActionState newAction)
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
        Pawn.CustomAnimator.CharacterVisualsLayer_FullBody.RequestCrossfadeTo(Pawn.CustomAnimator.CharacterVisualsLayer_FullBody.Walk);
    }

    public override void ExitState()
    {
    }

    public override void UpdateState(float deltaTime)
    {
        Pawn.Movement.UpdateMovement(LocomotionType.VelocityByDirectionalInput, Pawn.MoveInput);
        if (Pawn.AttackInput)
            Pawn.RequestFullBodyAction(new ACS_FullBody_Attack_JumpVerticalSlam(Pawn));
        else if (Pawn.MoveInput == Vector2.zero)
            Pawn.RequestFullBodyAction(new ACS_FullBody_Idle(Pawn));
    }
}
