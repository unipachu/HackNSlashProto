using UnityEngine;

public class ACS_FullBody_Attack_JumpVerticalSlam : ACS_FullBody
{
    // TODO: This should buffer actions, not just attacks. So this should save the type of action that has been buffered.
    bool _attackBuffered = false;

    public ACS_FullBody_Attack_JumpVerticalSlam(NewPlayerController playerController) : base(playerController)
    {
    }

    public override bool CanTransitionTo(ACS_ActionState newAction)
    {
        return true;
    }

    public override void EnterState()
    {
        PC.CustomAnimator.CharacterVisualsLayer_FullBody.RequestCrossfadeTo(PC.CustomAnimator.CharacterVisualsLayer_FullBody.Attack_JumpVerticalSlam);
        PC.Equipment.ReadyAttackRight();
        _attackBuffered = false;
    }

    public override void ExitState()
    {
    }

    // TODO: You might want to refactor logic into multiple methods.
    public override void UpdateState(float deltaTime)
    {
        PC.Movement.MoveCharacterController(PC.AnimationDeltaXZMovement);

        IHitboxActivatingAnimation hitBox = PC.CustomAnimator.CharacterVisualsLayer_FullBody.ActiveState as IHitboxActivatingAnimation;
        Debug.Assert(hitBox != null, nameof(ACS_FullBody_Attack_JumpVerticalSlam) + "'s related animation state wasn't a " + nameof(IHitboxActivatingAnimation));
        if (hitBox.IsHitboxActive())
        {
            PC.Equipment.AttackRight(PC.transform);
        }

        // TODO: Attack should only buffer if the attack button was PRESSED DOWN DURING THIS FRAME - now this allows buffering if the button was held down this frame.
        if (PC.CustomAnimator.CharacterVisualsLayer_FullBody.GetFixedTimeUntilAnimationEnd() <= PC.InputBufferTime 
            && PC.AttackInput)
        {
            _attackBuffered = true;
        }

        IExitTimeAnimation exitTime = PC.CustomAnimator.CharacterVisualsLayer_FullBody.ActiveState as IExitTimeAnimation;
        if (exitTime != null
            && PC.CustomAnimator.CharacterVisualsLayer_FullBody.GetActiveStateNormalizedTime() >= exitTime.NormalizedExitTime())
        {
            if(_attackBuffered)
            {
                PC.RequestFullBodyAction(PC.ACS_FullBody_Attack_JumpVerticalSlam);
                return;
            }
            else if(PC.MoveInput != Vector2.zero)
            {
                PC.RequestFullBodyAction(PC.ACS_FullBody_Walk);
                return;
            }
            else
            {
                PC.RequestFullBodyAction(PC.ACS_FullBody_Idle);
                return;
            }
        }
    }
}
