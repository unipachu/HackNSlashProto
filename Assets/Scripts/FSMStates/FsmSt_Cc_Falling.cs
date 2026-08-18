using UnityEngine;

public class FsmSt_Cc_Falling : MonoBehaviour, IFsmSt{
    [SerializeField] Pc pc;

    float timer;

    public void Enter(IFsmSt previousState) {
        timer = 0;
        VisUtils.CrossfadeAnim(pc.capsuleCharAnim, CapsuleCharAnimInfo.falling, 4);
    }

    public void Exit() {
    }

    public void PhysicsTick() {
    }

    public void Tick() {
        timer += Time.deltaTime;

        pc.charCtrlMov.UpdateMov(
            Vector2.zero,
            Vector3.zero,
            // TODO: Is this actually max hor speed?
            pc.Data.st_Falling_MaxLinSpd,
            0,
            pc.Data.st_Falling_LinAcc
        );
        if (pc.Data.isGrounded){
            // TODO: Make scriptable object field.
            if(timer > 0.7f) {
                pc.fsm.SwitchSt(pc.fsmSts.fallLanding);
                return;
            }
            pc.fsm.SwitchSt(pc.fsmSts.idle);
        }
    }

    public void LateTick() {
    }

    public bool CanSwitchStTo(IFsmSt newSt) => true;
}
