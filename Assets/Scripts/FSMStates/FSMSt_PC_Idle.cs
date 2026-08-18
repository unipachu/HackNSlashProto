using UnityEngine;

public class FsmSt_Pc_Idle : MonoBehaviour, IFsmSt{
    [SerializeField] Pc pc;

    public void Enter(IFsmSt previousState){
        VisUtils.CrossfadeAnim(pc.capsuleCharAnim, CapsuleCharAnimInfo.idle, 0.1f);
    }

    public void Exit(){
    }

    public void PhysicsTick(){
    }

    public void Tick(){
        pc.charCtrlMov.UpdateMov(Vector2.zero, Vector3.zero, 0, 0);
        if (pc.inputBuffer.TryConsumeInput(BufferableInput.Dodge))
            pc.fsm.SwitchSt(pc.fsmSts.dodge);
        else if (pc.inputBuffer.TryConsumeInput(BufferableInput.Atk_Light))
            pc.fsm.SwitchSt(pc.fsmSts.atk_HorSlash1);
        else if (pc.inputBuffer.TryConsumeInput(BufferableInput.Atk_Heavy))
            pc.fsm.SwitchSt(pc.fsmSts.atk_Jump);
        else if (pc.inputBuffer.TryConsumeInput(BufferableInput.Atk_Ult))
            pc.fsm.SwitchSt(pc.fsmSts.atk_FlyingAtk);
        else if (pc.Input_Mov != Vector2.zero)
            pc.fsm.SwitchSt(pc.fsmSts.walk);
    }

    public void LateTick() {
    }

    public bool CanSwitchStTo(IFsmSt newSt) => true;
}
