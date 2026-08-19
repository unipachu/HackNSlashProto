using System;
using UnityEngine;

public class FsmSt_Cc_Knockback_Weak : MonoBehaviour, IFsmSt{
    event Action<CapsuleCharAnimEvent> animEvent;

    [SerializeField] Pc pc;

    void OnEnable() {
        animEvent += OnAnimEvent;
    }

    void OnDisable() {
        animEvent -= OnAnimEvent;
    }

    public void Enter(IFsmSt previousState) {
        Vector3 viewVec = new Vector3(
            pc.Data.lastRecievedHitDir.x,
            0,
            pc.Data.lastRecievedHitDir.z
        );
        // If you, for some reason, set the hit direction to Vector3.zero.
        if (viewVec.sqrMagnitude < 0.0001f)
            viewVec = Vector3.down;
        else
            viewVec.Normalize();
        if (Vector3.Dot(pc.Data.lastRecievedHitDir, transform.forward) > 0) {
            //pc.transform.rotation = Quaternion.LookRotation(
            //    new Vector3(
            //        pc.Data.lastRecievedHitDir.x,
            //        0,
            //        pc.Data.lastRecievedHitDir.z
            //    ).normalized,
            //    Vector3.up
            //);
            VisUtils.CrossfadeNInitAnimEventPlr(
                ref pc.animEventPlr,
                pc.capsuleCharAnim,
                CapsuleCharAnimInfo.knockback_Weak_Fwd,
                animEvent,
                0.1f
            );
        }
        else {
            //pc.transform.rotation = Quaternion.LookRotation(
            //    new Vector3(
            //        -pc.Data.lastRecievedHitDir.x,
            //        0,
            //        -pc.Data.lastRecievedHitDir.z
            //    ).normalized,
            //    Vector3.up
            //);
            VisUtils.CrossfadeNInitAnimEventPlr(
                ref pc.animEventPlr,
                pc.capsuleCharAnim,
                CapsuleCharAnimInfo.knockback_Weak_Bwd,
                animEvent,
                0.1f
            );

        }
    }

    public void Exit() {
    }

    public void PhysicsTick() {
    }

    public void Tick() {
        if (CapsuleCharFsmUtils.SwitchToFallingStIfNotGrounded(pc))
            return;
        pc.charCtrlMov.UpdateMov(Vector3.zero, pc.AnimationDeltaMovement * pc.Data.lastKnockbackStr, 0, 0);
    }

    public void LateTick() {
        pc.animEventPlr.Tick();
    }

    public bool CanSwitchStTo(IFsmSt newSt) {
        // TODO: To avoid stun locking, after some amount of consequtive knockbacks,
        // TODO C: allow canceling knockback state.
        if(newSt == (IFsmSt)pc.fsmSts.knockback_Weak)
            return true;
        // TODO: Allow switch to death state.
        return false;
    }

    // ----------------------
    // Animation event
    // ----------------------

    private void OnAnimEvent(CapsuleCharAnimEvent id) {
        switch (id) {
            case CapsuleCharAnimEvent.Knockback_Weak_Bwd_Finished:
                if (pc.inputData.mov != Vector2.zero)
                    pc.fsm.SwitchSt(pc.fsmSts.walk);
                else
                    pc.fsm.SwitchSt(pc.fsmSts.idle);
                break;
            case CapsuleCharAnimEvent.Knockback_Weak_Fwd_Finished:
                if (pc.inputData.mov != Vector2.zero)
                    pc.fsm.SwitchSt(pc.fsmSts.walk);
                else
                    pc.fsm.SwitchSt(pc.fsmSts.idle);
                break;
        }
    }
}
