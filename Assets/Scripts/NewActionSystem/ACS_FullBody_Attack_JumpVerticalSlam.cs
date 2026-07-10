//using UnityEngine;

//public class ACS_FullBody_Attack_JumpVerticalSlam : ACS_FullBody
//{
//    // TODO: This should buffer actions, not just attacks. So this should save the type of action that has been buffered.
//    bool _attackBuffered = false;

//    public ACS_FullBody_Attack_JumpVerticalSlam(PC pawn) : base(pawn)
//    {
//    }

//    public override void BufferAction(ACS_ActionState newAction)
//    {
//        throw new System.NotImplementedException();
//    }

//    public override bool CanBuffer(ACS_ActionState newAction)
//    {
//        return false;
//    }

//    public override bool CanInstantlyTransitionTo(ACS_ActionState newAction)
//    {
//        return true;
//    }

//    public override void EnterState()
//    {
//        //Pawn.CustomAnimator.CharacterVisualsLayer_FullBody.RequestCrossfadeTo(Pawn.CustomAnimator.CharacterVisualsLayer_FullBody.Attack_JumpVerticalSlam);
//        Pawn.VisComponents.anims.Play_Attack_RHandJumpVerticalSlam();
//        Pawn.Equipment.ReadyAttackRight();
//        _attackBuffered = false;

//        Pawn.VisComponents.animEvents.Attack_RHandJumpVerticalSlam_Finished += OnAttackRHandJumpVerticalSlam_Finished;
//        Pawn.VisComponents.animEvents.Attack_RHandJumpVerticalSlam_HitboxActivated += OnAttack_RHandJumpVerticalSlam_HitboxActivated;
//        Pawn.VisComponents.animEvents.Attack_RHandJumpVerticalSlam_HitboxDeactivated += OnAttack_RHandJumpVerticalSlam_HitboxDeactivated;
//        Pawn.VisComponents.animEvents.Attack_RHandJumpVerticalSlam_JumpFinished += OnAttackRHandJumpVerticalSlam_JumpFinished;
//        Pawn.VisComponents.animEvents.Attack_RHandJumpVerticalSlam_JumpStarted += OnAttackRHandJumpVerticalSlam_JumpStarted;




//    }

//    public override void ExitState()
//    {
//        Pawn.Movement.IsAffectedByGravity = true;

//        Pawn.VisComponents.animEvents.Attack_RHandJumpVerticalSlam_Finished -= OnAttackRHandJumpVerticalSlam_Finished;
//        Pawn.VisComponents.animEvents.Attack_RHandJumpVerticalSlam_HitboxActivated -= OnAttack_RHandJumpVerticalSlam_HitboxActivated;
//        Pawn.VisComponents.animEvents.Attack_RHandJumpVerticalSlam_HitboxDeactivated -= OnAttack_RHandJumpVerticalSlam_HitboxDeactivated;
//        Pawn.VisComponents.animEvents.Attack_RHandJumpVerticalSlam_JumpStarted -= OnAttackRHandJumpVerticalSlam_JumpStarted;
//        Pawn.VisComponents.animEvents.Attack_RHandJumpVerticalSlam_JumpFinished -= OnAttackRHandJumpVerticalSlam_JumpFinished;

//    }

//    // TODO: You might want to refactor logic into multiple methods.
//    public override void UpdateState(float deltaTime)
//    {
//        Pawn.Movement.UpdateMovement(Vector3.zero, Pawn.AnimationDeltaMovement);

//        //IHitboxActivatingAnimation hitBox = Pawn.CustomAnimator.CharacterVisualsLayer_FullBody.ActiveState as IHitboxActivatingAnimation;
//        //Debug.Assert(hitBox != null, nameof(ACS_FullBody_Attack_JumpVerticalSlam) + "'s related animation state wasn't a " + nameof(IHitboxActivatingAnimation));
//        //if (hitBox.IsHitboxActive())
//        //{
//        //    Pawn.Equipment.AttackRight(Pawn.ThisObject.transform);
//        //}

//        //// TODO: Attack should only buffer if the attack button was PRESSED DOWN DURING THIS FRAME - now this allows buffering if the button was held down this frame.
//        //if (Pawn.CustomAnimator.CharacterVisualsLayer_FullBody.GetFixedTimeUntilAnimationEnd() <= Pawn.InputBufferTime 
//        //    && Pawn.AttackInput)
//        //{
//        //    _attackBuffered = true;
//        //}

//        //IExitTimeAnimation exitTime = Pawn.CustomAnimator.CharacterVisualsLayer_FullBody.ActiveState as IExitTimeAnimation;
//        //if (exitTime != null
//        //    && Pawn.CustomAnimator.CharacterVisualsLayer_FullBody.GetActiveStateNormalizedTime() >= exitTime.NormalizedExitTime())
//        //{
//        //    if(_attackBuffered)
//        //    {
//        //        Pawn.RequestFullBodyAction(new ACS_FullBody_Attack_JumpVerticalSlam(Pawn));
//        //        return;
//        //    }
//        //    else if(Pawn.MoveInput != Vector2.zero)
//        //    {
//        //        Pawn.RequestFullBodyAction(new ACS_FullBody_Walk(Pawn));
//        //        return;
//        //    }
//        //    else
//        //    {
//        //        Pawn.RequestFullBodyAction(new ACS_FullBody_Idle(Pawn));
//        //        return;
//        //    }
//        //}
//    }

//    private void OnAttackRHandJumpVerticalSlam_Finished()
//    {
//        if (_attackBuffered)
//        {
//            Pawn.RequestFullBodyAction(new ACS_FullBody_Attack_JumpVerticalSlam(Pawn));
//            return;
//        }
//        else if (Pawn.MoveInput != Vector2.zero)
//        {
//            Pawn.RequestFullBodyAction(new ACS_FullBody_Walk(Pawn));
//            return;
//        }
//        else
//        {
//            Pawn.RequestFullBodyAction(new ACS_FullBody_Idle(Pawn));
//            return;
//        }
//    }
    
//    private void OnAttack_RHandJumpVerticalSlam_HitboxActivated()
//    {
//        // TODO
//    }

//    private void OnAttack_RHandJumpVerticalSlam_HitboxDeactivated()
//    {
//        // TODO
//    }

//    private void OnAttackRHandJumpVerticalSlam_JumpFinished()
//    {
//        // TODO: Only do if this is current state. TBH, you should probably figure out a
//        // TODO C: general way to do these events to force events only when the state is active.
//        Pawn.Movement.IsAffectedByGravity = true;
//        Pawn.Movement._verticalVelocity = -10;
//    }

//    private void OnAttackRHandJumpVerticalSlam_JumpStarted()
//    {
//        // TODO: Only do if this is current state. TBH, you should probably figure out a
//        // TODO C: general way to do these events to force events only when the state is active.
//        Pawn.Movement.IsAffectedByGravity = false;
//    }


//}
