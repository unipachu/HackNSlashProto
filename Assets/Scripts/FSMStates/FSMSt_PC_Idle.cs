using UnityEngine;

public class FsmSt_Pc_Idle : MonoBehaviour, IFsmSt{
    [SerializeField] Pc pc;

    public void Enter(IFsmSt previousState){
        pc.visComponents.anims.Play_Idle();
    }

    public void Exit(){
    }

    public void PhysicsTick(){
    }

    public void Tick(){
        pc.charCtrlMov.UpdateMov(Vector2.zero, Vector3.zero, 0, 0);
        if (pc.inputBuffer.TryConsumeInput("dodge"))
            pc.fsm.SwitchSt(pc.fsmSts.dodge);
        else if (pc.inputBuffer.TryConsumeInput("atk1"))
            pc.fsm.SwitchSt(pc.fsmSts.atk_HorSlash1);
        else if (pc.inputBuffer.TryConsumeInput("atk2"))
            pc.fsm.SwitchSt(pc.fsmSts.atk_Jump);
        else if (pc.inputBuffer.TryConsumeInput("atk3"))
            pc.fsm.SwitchSt(pc.fsmSts.atk_HorFlyingAtk);
        else if (pc.MoveInput != Vector2.zero)
            pc.fsm.SwitchSt(pc.fsmSts.walk);
    }

    public void LateTick() {
    }
}
