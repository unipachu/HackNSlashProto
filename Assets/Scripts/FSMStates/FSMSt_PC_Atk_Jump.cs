using UnityEngine;

public class FsmSt_Pc_Atk_Jump : MonoBehaviour, IFsmSt{
    [SerializeField] Pc pc;

    void OnEnable(){
        pc.visComponents.animEvents.Atk_JumpVerSlam_Finished += OnAttackRHandJumpVerticalSlam_Finished;
        pc.visComponents.animEvents.Atk_JumpVerSlam_HitboxActivated += OnAttack_RHandJumpVerticalSlam_HitboxActivated;
        pc.visComponents.animEvents.Atk_JumpVerSlam_HitboxDeactivated += OnAttack_RHandJumpVerticalSlam_HitboxDeactivated;
        pc.visComponents.animEvents.Atk_JumpVerSlam_JumpFinished += OnAttackRHandJumpVerticalSlam_JumpFinished;
        pc.visComponents.animEvents.Atk_JumpVerSlam_JumpStarted += OnAttackRHandJumpVerticalSlam_JumpStarted;
    }

    void OnDisable(){
        pc.visComponents.animEvents.Atk_JumpVerSlam_Finished -= OnAttackRHandJumpVerticalSlam_Finished;
        pc.visComponents.animEvents.Atk_JumpVerSlam_HitboxActivated -= OnAttack_RHandJumpVerticalSlam_HitboxActivated;
        pc.visComponents.animEvents.Atk_JumpVerSlam_HitboxDeactivated -= OnAttack_RHandJumpVerticalSlam_HitboxDeactivated;
        pc.visComponents.animEvents.Atk_JumpVerSlam_JumpStarted -= OnAttackRHandJumpVerticalSlam_JumpStarted;
        pc.visComponents.animEvents.Atk_JumpVerSlam_JumpFinished -= OnAttackRHandJumpVerticalSlam_JumpFinished;
    }

    public void Enter(IFsmSt previousState){
        pc.visComponents.anims.Play_Atk_RHandJumpVerSlam();
    }

    public void Exit(){
        pc.charCtrlMov.IsAffectedByGravity = true;
    }

    public void PhysicsTick(){
    }

    public void Tick(){
        pc.charCtrlMov.UpdateMov(Vector3.zero, pc.AnimationDeltaMovement, 0, 0);
    }

    public void LateTick() {
    }

    // -------------------------
    // Anim Event Callbacks
    // -------------------------

    void OnAttackRHandJumpVerticalSlam_Finished(){
        if(pc.fsm.CurSt != (IFsmSt)this)
            return;
        if (pc.MoveInput != Vector2.zero){
            pc.fsm.SwitchSt(pc.fsmSts.walk);
            return;
        }
        else{
            pc.fsm.SwitchSt(pc.fsmSts.idle);
            return;
        }
    }

    void OnAttack_RHandJumpVerticalSlam_HitboxActivated(){
        if (pc.fsm.CurSt != (IFsmSt)this)
            return;
        // TODO
    }

    void OnAttack_RHandJumpVerticalSlam_HitboxDeactivated(){
        if (pc.fsm.CurSt != (IFsmSt)this)
            return;
        // TODO
    }

    void OnAttackRHandJumpVerticalSlam_JumpFinished(){
        if (pc.fsm.CurSt != (IFsmSt)this)
            return;
        // TODO: Only do if this is current state. TBH, you should probably figure out a
        // TODO C: general way to do these events to force events only when the state is active.
        pc.charCtrlMov.IsAffectedByGravity = true;
        pc.charCtrlMov.verVel = -pc.Data.st_AtkJump_DownSpeedAfterJumpFinished;
    }

    void OnAttackRHandJumpVerticalSlam_JumpStarted(){
        if (pc.fsm.CurSt != (IFsmSt)this)
            return;
        // TODO: Only do if this is current state. TBH, you should probably figure out a
        // TODO C: general way to do these events to force events only when the state is active.
        pc.charCtrlMov.IsAffectedByGravity = false;
    }
}
