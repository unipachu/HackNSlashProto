using UnityEngine;

public class FSMSt_PC_Walk : MonoBehaviour, IFSMSt
{
    [SerializeField] private PC pc;

    public void Enter(IFSMSt previousState)
    {
        switch(previousState)
        {
            case FSMSt_PC_Atk_HorSlash1:
                // We use a little more time to fade to the walk animation.
                pc.VisComponents.anims.Play_Walk(1);
                break;
            case FSMSt_PC_Atk_HorSlash2:
                // We use a little more time to fade to the walk animation.
                pc.VisComponents.anims.Play_Walk(1);
                break;
            default:
                pc.VisComponents.anims.Play_Walk();
                break;
        }
    }

    public void Exit()
    {
    }

    public void PhysicsTick()
    {
    }

    public void Tick()
    {
        pc.Movement.UpdateMovement(pc.MoveInput, Vector3.zero, pc.baseData.St_Walk_MaxLinSpd, pc.baseData.St_Walk_LinAcc, pc.baseData.St_Walk_MaxAngSpd);
        
        if (pc.inputBuffer.TryConsumeInput("dodge"))
            pc.fSM.SwitchState(pc.fSMStates.dodge);
        else if (pc.inputBuffer.TryConsumeInput("atk1"))
            pc.fSM.SwitchState(pc.fSMStates.atk_HorSlash1);
        else if (pc.inputBuffer.TryConsumeInput("atk2"))
            pc.fSM.SwitchState(pc.fSMStates.atk_Jump);
        else if (pc.MoveInput == Vector2.zero)
            pc.fSM.SwitchState(pc.fSMStates.idle);
    }
}
