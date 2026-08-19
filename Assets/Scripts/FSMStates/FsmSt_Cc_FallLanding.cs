using System;
using UnityEngine;

public class FsmSt_Cc_FallLanding : MonoBehaviour, IFsmSt{
    event Action<CapsuleCharAnimEvent> animEvent;

    [SerializeField] Pc pc;

    bool dodgeAllowed = false;

    void OnEnable() {
        animEvent += OnAnimEvent;
    }

    void OnDisable() {
        animEvent -= OnAnimEvent;
    }

    public void Enter(IFsmSt previousState) {
        dodgeAllowed = false;
        VisUtils.CrossfadeNInitAnimEventPlr(
            ref pc.animEventPlr,
            pc.capsuleCharAnim,
            CapsuleCharAnimInfo.fallLanding,
            animEvent,
            // TODO MINOR: You could make this transition faster if falling from higher/faster,
            // TODO MINOR C: e.g. 0.2 if hitting the ground with slow speed, and 0.1 if hitting
            // TODO MINOR C: the ground while fast falling speed.
            0.2f
        );
    }

    public void Exit() {
    }

    public void PhysicsTick() {
    }

    public void Tick() {
        pc.charCtrlMov.UpdateMov(
            Vector2.zero,
            Vector3.zero,
            0,
            0
        );
        if (CapsuleCharFsmUtils.SwitchToFallingStIfNotGrounded(pc))
            return;
        if (dodgeAllowed) {
            if (pc.inputBuffer.TryConsumeInput(BufferableInput.Dodge)) {
                pc.fsm.SwitchSt(pc.fsmSts.dodge);
                return;
            }
        }
    }

    public void LateTick() {
        pc.animEventPlr.Tick();
    }

    public bool CanSwitchStTo(IFsmSt newSt) => true;

    // ----------------------
    // Animation Event
    // ----------------------

    void OnAnimEvent(CapsuleCharAnimEvent id) {
        switch (id) {
            // TODO: There could be a dodge window from the beginning of the state to
            // TODO C: few frames into it instead of this.
            case CapsuleCharAnimEvent.FallLanding_CanSwitchSt:
                dodgeAllowed = true;
                return;
            case CapsuleCharAnimEvent.FallLanding_Finished:
                pc.fsm.SwitchSt(pc.fsmSts.idle);
                return;
        }
    }
}
