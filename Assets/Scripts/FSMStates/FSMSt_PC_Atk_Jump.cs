using System;
using UnityEngine;

public class FsmSt_Pc_Atk_Jump : MonoBehaviour, IFsmSt{
    event Action<CapsuleCharAnimEvent> animEvent;
    
    [SerializeField] Pc pc;

    void OnEnable(){
        animEvent += OnAnimEvent;
    }

    void OnDisable(){
        animEvent -= OnAnimEvent;
    }

    public void Enter(IFsmSt previousState){
        VisUtils.CrossfadeNInitAnimEventPlr(
            ref pc.animEventPlr,
            pc.capsuleCharAnim,
            CapsuleCharAnimInfo.atk_JumpVerSlam,
            animEvent
        );
    }

    public void Exit(){
        pc.charCtrlMov.IsAffectedByGravity = true;
        pc.weapon.hitDealer.Deactivate();
    }

    public void PhysicsTick(){
    }

    public void Tick(){
        pc.charCtrlMov.UpdateMov(Vector3.zero, pc.AnimationDeltaMovement, 0, 0);
    }

    public void LateTick() {
        pc.animEventPlr.Tick();
    }

    public bool CanSwitchStTo(IFsmSt newSt) => true;


    // ----------------------
    // Animation Event
    // ----------------------

    private void OnAnimEvent(CapsuleCharAnimEvent id) {
        switch (id) {
            case CapsuleCharAnimEvent.JumpVerSlam_JumpStarted:
                pc.charCtrlMov.IsAffectedByGravity = false;
                break;
            case CapsuleCharAnimEvent.JumpVerSlam_HitboxActivated:
                pc.weapon.hitDealer.Activate();
                break;
            case CapsuleCharAnimEvent.JumpVerSlam_JumpFinished:
                pc.charCtrlMov.IsAffectedByGravity = true;
                pc.charCtrlMov.verVel = -pc.Data.st_AtkJump_DownSpeedAfterJumpFinished;
                break;
            case CapsuleCharAnimEvent.JumpVerSlam_HitboxDeactivated:
                pc.weapon.hitDealer.Deactivate();
                break;
            case CapsuleCharAnimEvent.JumpVerSlam_Finished:
                if (pc.MoveInput != Vector2.zero)
                    pc.fsm.SwitchSt(pc.fsmSts.walk);
                else
                    pc.fsm.SwitchSt(pc.fsmSts.idle);
                break;
        }
    }
}
