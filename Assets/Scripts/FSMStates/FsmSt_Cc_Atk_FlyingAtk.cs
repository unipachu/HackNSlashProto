using System;
using UnityEngine;

// TODO: This should probably be called FlyingSlam or something more descriptive than "Atk".
public class FsmSt_Cc_Atk_FlyingAtk : MonoBehaviour, IFsmSt {
    event Action<CapsuleCharAnimEvent> animEvent;
    [SerializeField] Pc pc;

    AtkPhase attackPhase = AtkPhase.Windup;
    bool impactFinished = false;

    void OnEnable() {
        animEvent += OnAnimEvent;
    }

    void OnDisable() {
        animEvent -= OnAnimEvent;
    }

    public void Enter(IFsmSt previousState) {
        var data = pc.Data;
        data.invul = true;
        data.isAffectedByGravity = false;
        pc.Data = data;
        attackPhase = AtkPhase.Windup;
        impactFinished = false;
        pc.inputBuffer.Clear();
        VisUtils.CrossfadeNInitAnimEventPlr(
            ref pc.animEventPlr,
            pc.capsuleCharAnim,
            CapsuleCharAnimInfo.atk_FlyingAtk_Windup,
            animEvent,
            0.1f
        );
    }

    public void Exit() {
        CapsuleCharData data = pc.Data;
        data.invul = false;
        data.isAffectedByGravity = true;
        pc.Data = data;
        pc.weapon.aoeHitDealer.Deactivate();
    }

    public void PhysicsTick() {
    }

    public void Tick() {
        switch (attackPhase) {
            case AtkPhase.Windup:
                pc.charCtrlMov.UpdateMov(
                    // TODO: Have some interpolation to this so that this smoothly fades to
                    // TODO C: the input hor speed of the impact part of the attack.
                    pc.inputData.mov,
                    pc.AnimationDeltaMovement,
                    2,
                    0
                );
                break;
            case AtkPhase.Impact:
                // TODO: Set values in base data.
                pc.charCtrlMov.UpdateMov(
                    Vector3.zero,
                    pc.AnimationDeltaMovement,
                    0,
                    0
                );
                if (pc.Data.isGrounded && impactFinished) {
                    pc.weapon.aoeHitDealer.Deactivate();
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
                pc.charCtrlMov.UpdateMov(
                    Vector2.zero,
                    Vector3.zero,
                    0,
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

    public bool CanSwitchStTo(IFsmSt newSt) {
        if (newSt == (IFsmSt) pc.fsmSts.falling)
            return false;
        else
            return true;
    }

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
                pc.weapon.aoeHitDealer.atkData = new(1, KnockbackT.Weak, 5);
                pc.weapon.aoeHitDealer.Activate();
                break;
            case CapsuleCharAnimEvent.FlyingAtk_Impact_Finished:
                CapsuleCharData data = pc.Data;
                data.isAffectedByGravity = true;
                impactFinished = true;
                // TODO: Set this in base data.
                data.vel_Ver = -40;
                pc.Data = data;
                break;
            case CapsuleCharAnimEvent.FlyingAtk_Recovery_Finished:
                pc.fsm.SwitchSt(pc.fsmSts.idle);
                break;
        }
    }
}
