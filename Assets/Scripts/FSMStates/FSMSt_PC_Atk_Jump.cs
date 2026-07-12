using UnityEngine;

public class FSMSt_PC_Atk_Jump : MonoBehaviour, IFSMSt
{
    [SerializeField] PC pc;

    void OnEnable()
    {
        pc.visComponents.animEvents.Atk_JumpVerSlam_Finished += OnAttackRHandJumpVerticalSlam_Finished;
        pc.visComponents.animEvents.Atk_JumpVerSlam_HitboxActivated += OnAttack_RHandJumpVerticalSlam_HitboxActivated;
        pc.visComponents.animEvents.Atk_JumpVerSlam_HitboxDeactivated += OnAttack_RHandJumpVerticalSlam_HitboxDeactivated;
        pc.visComponents.animEvents.Atk_JumpVerSlam_JumpFinished += OnAttackRHandJumpVerticalSlam_JumpFinished;
        pc.visComponents.animEvents.Atk_JumpVerSlam_JumpStarted += OnAttackRHandJumpVerticalSlam_JumpStarted;
    }

    void OnDisable()
    {
        pc.visComponents.animEvents.Atk_JumpVerSlam_Finished -= OnAttackRHandJumpVerticalSlam_Finished;
        pc.visComponents.animEvents.Atk_JumpVerSlam_HitboxActivated -= OnAttack_RHandJumpVerticalSlam_HitboxActivated;
        pc.visComponents.animEvents.Atk_JumpVerSlam_HitboxDeactivated -= OnAttack_RHandJumpVerticalSlam_HitboxDeactivated;
        pc.visComponents.animEvents.Atk_JumpVerSlam_JumpStarted -= OnAttackRHandJumpVerticalSlam_JumpStarted;
        pc.visComponents.animEvents.Atk_JumpVerSlam_JumpFinished -= OnAttackRHandJumpVerticalSlam_JumpFinished;
    }

    public void Enter(IFSMSt previousState)
    {
        pc.visComponents.anims.Play_Attack_RHandJumpVerticalSlam();
    }

    public void Exit()
    {
        pc.locomotion.IsAffectedByGravity = true;
    }

    public void PhysicsTick()
    {
    }

    public void Tick()
    {
        pc.locomotion.UpdateMovement(Vector3.zero, pc.AnimationDeltaMovement, 0, 0);
    }

    // -------------------------
    // Anim Event Callbacks
    // -------------------------

    void OnAttackRHandJumpVerticalSlam_Finished()
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

    void OnAttack_RHandJumpVerticalSlam_HitboxActivated()
    {
        if (pc.fSM.CurrentState != (IFSMSt)this) return;

        // TODO
    }

    void OnAttack_RHandJumpVerticalSlam_HitboxDeactivated()
    {
        if (pc.fSM.CurrentState != (IFSMSt)this) return;

        // TODO
    }

    void OnAttackRHandJumpVerticalSlam_JumpFinished()
    {
        if (pc.fSM.CurrentState != (IFSMSt)this) return;

        // TODO: Only do if this is current state. TBH, you should probably figure out a
        // TODO C: general way to do these events to force events only when the state is active.
        pc.locomotion.IsAffectedByGravity = true;
        pc.locomotion._verticalVelocity = -pc.baseData.St_AtkJump_DownSpeedAfterJumpFinished;
    }

    void OnAttackRHandJumpVerticalSlam_JumpStarted()
    {
        if (pc.fSM.CurrentState != (IFSMSt)this) return;

        // TODO: Only do if this is current state. TBH, you should probably figure out a
        // TODO C: general way to do these events to force events only when the state is active.
        pc.locomotion.IsAffectedByGravity = false;
    }
}
