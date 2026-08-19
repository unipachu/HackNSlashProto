using UnityEngine;

public class FsmSt_Cc_Falling : MonoBehaviour, IFsmSt{
    [SerializeField] Pc pc;

    float fallingStartHgt;

    public void Enter(IFsmSt previousState) {
        fallingStartHgt = pc.transform.position.y;
        VisUtils.CrossfadeAnim(pc.capsuleCharAnim, CapsuleCharAnimInfo.falling, 4);
    }

    public void Exit() {
    }

    public void PhysicsTick() {
    }

    public void Tick() {
        pc.charCtrlMov.UpdateMov(
            Vector2.zero,
            Vector3.zero,
            // TODO MINOR: You could use st_Falling_MaxLinSpd in here + hor input to
            // TODO MINOR: allow for slight air control.
            0,
            0,
            pc.Data.st_Falling_LinAcc
        );
        if (pc.Data.isGrounded){
            float fallDist = fallingStartHgt - pc.transform.position.y;
            // TODO: Make scriptable object field. This decides if the player will go to
            // TODO C: landing animation or straight to idle.
            if(fallDist > 2) {
                pc.fsm.SwitchSt(pc.fsmSts.fallLanding);
                return;
            }
            pc.fsm.SwitchSt(pc.fsmSts.idle);
            return;
        }
        if(pc.Data.curStDur > 20) {
            // TODO: Character stuck falling. Kill/reset character (maybe have a unique
            // TODO C: death state for when character dies like this where the player doesn't
            // TODO C: lose their souls).
        }
    }

    public void LateTick() {
    }

    public bool CanSwitchStTo(IFsmSt newSt) => true;
}
