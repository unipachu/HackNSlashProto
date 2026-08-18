using System;
using UnityEngine;

// TODO: This should probably be called FlyingSlam or something more descriptive than "Atk".
public class FsmSt_Pc_Atk_FlyingAtk : MonoBehaviour, IFsmSt{
    event Action<CapsuleCharAnimEvent> animEvent;
    [SerializeField] Pc pc;

    AtkPhase attackPhase = AtkPhase.Windup;
    bool impactFinished = false;

    void OnEnable() {
        animEvent += OnAnimEvent;
    }

    void OnDisable(){
        animEvent -= OnAnimEvent;
    }

    public void Enter(IFsmSt previousState){
        var data = pc.Data;
        data.invul = true;
        pc.Data = data;
        attackPhase = AtkPhase.Windup;
        impactFinished = false;
        pc.charCtrlMov.IsAffectedByGravity = false;
        pc.inputBuffer.Clear();
        VisUtils.CrossfadeNInitAnimEventPlr(
            ref pc.animEventPlr,
            pc.capsuleCharAnim,
            CapsuleCharAnimInfo.atk_FlyingAtk_Windup,
            animEvent,
            0.1f
        );
    }

    public void Exit(){
        var data = pc.Data;
        data.invul = false;
        pc.Data = data;
        pc.charCtrlMov.IsAffectedByGravity = true;
        pc.weapon.hitDealer.Deactivate();
    }

    public void PhysicsTick(){
    }

    public void Tick(){
        switch (attackPhase){
            case AtkPhase.Windup:
                pc.charCtrlMov.UpdateMov(
                    pc.Input_Mov,
                    pc.AnimationDeltaMovement,
                    2,
                    0
                );
                break;
            case AtkPhase.Impact:
                // TODO: Set values in base data.
                pc.charCtrlMov.UpdateMov(Vector3.zero, pc.AnimationDeltaMovement, 0, 0);
                if(pc.charCtrlMov.IsGrounded() && impactFinished){
                    pc.weapon.hitDealer.Deactivate();
                    attackPhase = AtkPhase.Recovery;
                    VisUtils.CrossfadeNInitAnimEventPlr(
                        ref pc.animEventPlr,
                        pc.capsuleCharAnim,
                        CapsuleCharAnimInfo.atk_FlyingAtk_Recovery,
                        animEvent
                    );
                }
                break;
            case AtkPhase.Recovery:
                // TODO: Set values in base data.
                pc.charCtrlMov.UpdateMov(
                    pc.Input_Mov,
                    Vector3.zero,
                    pc.Data.st_Walk_MaxLinSpd,
                    0,
                    0
                );
                break;
            default:
                Debug.LogError("Switch defaulted.", this);
                break;
        }
    }

    public void LateTick() {
        pc.animEventPlr.Tick();
    }

    public bool CanSwitchStTo(IFsmSt newSt) => true;

    // -------------------------
    // Anim Event
    // -------------------------

    private void OnAnimEvent(CapsuleCharAnimEvent id) {
        switch (id) {
            case CapsuleCharAnimEvent.FlyingAtk_Windup_Finished:
                VisUtils.CrossfadeNInitAnimEventPlr(
                    ref pc.animEventPlr,
                    pc.capsuleCharAnim,
                    CapsuleCharAnimInfo.atk_FlyingAtk_Impact,
                    animEvent
                );
                attackPhase = AtkPhase.Impact;
                break;
            case CapsuleCharAnimEvent.FlyingAtk_Impact_HitDealerActivated:
                pc.weapon.hitDealer.Activate();
                break;
            case CapsuleCharAnimEvent.FlyingAtk_Impact_Finished:
                pc.charCtrlMov.IsAffectedByGravity = true;
                impactFinished = true;
                // TODO: Set this in base data.
                pc.charCtrlMov.verVel = -40;
                break;
            case CapsuleCharAnimEvent.FlyingAtk_Recovery_Finished:
                pc.fsm.SwitchSt(pc.fsmSts.idle);
                break;
        }
    }
}
