using UnityEngine;

public class FSMSt_PC_Idle : MonoBehaviour, IFSMSt
{
    [SerializeField] PC pc;

    public void Enter(IFSMSt previousState)
    {
        pc.VisComponents.anims.Play_Idle();
    }

    public void Exit()
    {
    }

    public void PhysicsTick()
    {
    }

    public void Tick()
    {
        pc.Movement.UpdateMovement(Vector2.zero, Vector3.zero, 0, 0);
        
        if (pc.inputBuffer.TryConsumeInput("dodge"))
            pc.fSM.SwitchState(pc.fSMStates.dodge);
        else if (pc.inputBuffer.TryConsumeInput("atk1"))
            pc.fSM.SwitchState(pc.fSMStates.atk_HorSlash1);
        else if (pc.inputBuffer.TryConsumeInput("atk2"))
            pc.fSM.SwitchState(pc.fSMStates.atk_Jump);
        else if (pc.MoveInput != Vector2.zero)
            pc.fSM.SwitchState(pc.fSMStates.walk);
    }
}
