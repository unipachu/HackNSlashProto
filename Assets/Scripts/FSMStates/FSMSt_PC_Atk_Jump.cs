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
        pc.locomotion.IsAffectedByGravity = true;
    }

    public void PhysicsTick(){
    }

    public void Tick(){
        pc.locomotion.UpdateMov(Vector3.zero, pc.AnimationDeltaMovement, 0, 0);
    }

    // -------------------------
    // Anim Event Callbacks
    // -------------------------

    void OnAttackRHandJumpVerticalSlam_Finished(){
        if(pc.fSM.CurSt != (IFsmSt)this)
            return;
        if (pc.MoveInput != Vector2.zero){
            pc.fSM.SwitchSt(pc.fSMStates.walk);
            return;
        }
        else{
            pc.fSM.SwitchSt(pc.fSMStates.idle);
            return;
        }
    }

    void OnAttack_RHandJumpVerticalSlam_HitboxActivated(){
        if (pc.fSM.CurSt != (IFsmSt)this)
            return;
        // TODO
    }

    void OnAttack_RHandJumpVerticalSlam_HitboxDeactivated(){
        if (pc.fSM.CurSt != (IFsmSt)this)
            return;
        // TODO
    }

    void OnAttackRHandJumpVerticalSlam_JumpFinished(){
        if (pc.fSM.CurSt != (IFsmSt)this)
            return;
        // TODO: Only do if this is current state. TBH, you should probably figure out a
        // TODO C: general way to do these events to force events only when the state is active.
        pc.locomotion.IsAffectedByGravity = true;
        pc.locomotion.verVel = -pc.Data.st_AtkJump_DownSpeedAfterJumpFinished;
    }

    void OnAttackRHandJumpVerticalSlam_JumpStarted(){
        if (pc.fSM.CurSt != (IFsmSt)this)
            return;
        // TODO: Only do if this is current state. TBH, you should probably figure out a
        // TODO C: general way to do these events to force events only when the state is active.
        pc.locomotion.IsAffectedByGravity = false;
    }
}
