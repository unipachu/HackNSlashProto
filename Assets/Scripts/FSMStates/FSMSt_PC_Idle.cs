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
        pc.locomotion.UpdateMov(Vector2.zero, Vector3.zero, 0, 0);
        if (pc.inputBuffer.TryConsumeInput("dodge"))
            pc.fSM.SwitchSt(pc.fSMStates.dodge);
        else if (pc.inputBuffer.TryConsumeInput("atk1"))
            pc.fSM.SwitchSt(pc.fSMStates.atk_HorSlash1);
        else if (pc.inputBuffer.TryConsumeInput("atk2"))
            pc.fSM.SwitchSt(pc.fSMStates.atk_Jump);
        else if (pc.inputBuffer.TryConsumeInput("atk3"))
            pc.fSM.SwitchSt(pc.fSMStates.atk_HorFlyingAtk);
        else if (pc.MoveInput != Vector2.zero)
            pc.fSM.SwitchSt(pc.fSMStates.walk);
    }
}
