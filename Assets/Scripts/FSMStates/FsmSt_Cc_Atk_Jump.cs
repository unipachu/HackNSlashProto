using System;
using UnityEngine;

public class FsmSt_Cc_Atk_Jump : MonoBehaviour, IFsmSt{
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
        CapsuleCharData data = pc.Data;
        data.isAffectedByGravity = true;
        pc.Data = data;
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

    public bool CanSwitchStTo(IFsmSt newSt) {
        if (newSt == (IFsmSt)pc.fsmSts.falling)
            return false;
        else
            return true;
    }


    // ----------------------
    // Animation Event
    // ----------------------

    private void OnAnimEvent(CapsuleCharAnimEvent id) {
        CapsuleCharData data = pc.Data;
        switch (id) {
            case CapsuleCharAnimEvent.JumpVerSlam_JumpStarted:
                data.isAffectedByGravity = false;
                pc.Data = data;
                break;
            case CapsuleCharAnimEvent.JumpVerSlam_HitboxActivated:
                pc.weapon.hitDealer.Activate();
                break;
            case CapsuleCharAnimEvent.JumpVerSlam_JumpFinished:
                data.isAffectedByGravity = true;
                data.vel_Ver = -pc.Data.st_AtkJump_DownSpeedAfterJumpFinished;
                pc.Data = data;
                break;
            case CapsuleCharAnimEvent.JumpVerSlam_HitboxDeactivated:
                pc.weapon.hitDealer.Deactivate();
                break;
            case CapsuleCharAnimEvent.JumpVerSlam_Finished:
                if (pc.inputData.mov != Vector2.zero)
                    pc.fsm.SwitchSt(pc.fsmSts.walk);
                else
                    pc.fsm.SwitchSt(pc.fsmSts.idle);
                break;
        }
    }
}
