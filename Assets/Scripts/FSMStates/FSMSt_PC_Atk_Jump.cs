using UnityEngine;

public class FSMSt_PC_Atk_Jump : MonoBehaviour, IFSMSt
{
    [SerializeField] PC pc;

    private void OnEnable()
    {
        pc.VisComponents.animEvents.Attack_RHandJumpVerticalSlam_Finished += OnAttackRHandJumpVerticalSlam_Finished;
        pc.VisComponents.animEvents.Attack_RHandJumpVerticalSlam_HitboxActivated += OnAttack_RHandJumpVerticalSlam_HitboxActivated;
        pc.VisComponents.animEvents.Attack_RHandJumpVerticalSlam_HitboxDeactivated += OnAttack_RHandJumpVerticalSlam_HitboxDeactivated;
        pc.VisComponents.animEvents.Attack_RHandJumpVerticalSlam_JumpFinished += OnAttackRHandJumpVerticalSlam_JumpFinished;
        pc.VisComponents.animEvents.Attack_RHandJumpVerticalSlam_JumpStarted += OnAttackRHandJumpVerticalSlam_JumpStarted;
    }

    private void OnDisable()
    {
        pc.VisComponents.animEvents.Attack_RHandJumpVerticalSlam_Finished -= OnAttackRHandJumpVerticalSlam_Finished;
        pc.VisComponents.animEvents.Attack_RHandJumpVerticalSlam_HitboxActivated -= OnAttack_RHandJumpVerticalSlam_HitboxActivated;
        pc.VisComponents.animEvents.Attack_RHandJumpVerticalSlam_HitboxDeactivated -= OnAttack_RHandJumpVerticalSlam_HitboxDeactivated;
        pc.VisComponents.animEvents.Attack_RHandJumpVerticalSlam_JumpStarted -= OnAttackRHandJumpVerticalSlam_JumpStarted;
        pc.VisComponents.animEvents.Attack_RHandJumpVerticalSlam_JumpFinished -= OnAttackRHandJumpVerticalSlam_JumpFinished;
    }

    public void Enter()
    {
        pc.VisComponents.anims.Play_Attack_RHandJumpVerticalSlam();
    }

    public void Exit()
    {
        pc.Movement.IsAffectedByGravity = true;
    }

    public void PhysicsTick()
    {
    }

    public void Tick()
    {
        pc.Movement.UpdateMovement(Vector3.zero, pc.AnimationDeltaMovement, 0, 0);
    }

    private void OnAttackRHandJumpVerticalSlam_Finished()
    {
        if(pc.fSM.CurrentState != (IFSMSt)this) return;

        if (pc.MoveInput != Vector2.zero)
        {
            pc.fSM.SwitchState(pc.fSMStates.walk);
            return;
        }
        else
        {
            pc.fSM.SwitchState(pc.fSMStates.idle);
            return;
        }
    }

    private void OnAttack_RHandJumpVerticalSlam_HitboxActivated()
    {
        if (pc.fSM.CurrentState != (IFSMSt)this) return;

        // TODO
    }

    private void OnAttack_RHandJumpVerticalSlam_HitboxDeactivated()
    {
        if (pc.fSM.CurrentState != (IFSMSt)this) return;

        // TODO
    }

    private void OnAttackRHandJumpVerticalSlam_JumpFinished()
    {
        if (pc.fSM.CurrentState != (IFSMSt)this) return;

        // TODO: Only do if this is current state. TBH, you should probably figure out a
        // TODO C: general way to do these events to force events only when the state is active.
        pc.Movement.IsAffectedByGravity = true;
        pc.Movement._verticalVelocity = -pc.baseData.St_AtkJump_DownSpeedAfterJumpFinished;
    }

    private void OnAttackRHandJumpVerticalSlam_JumpStarted()
    {
        if (pc.fSM.CurrentState != (IFSMSt)this) return;

        // TODO: Only do if this is current state. TBH, you should probably figure out a
        // TODO C: general way to do these events to force events only when the state is active.
        pc.Movement.IsAffectedByGravity = false;
    }
}
