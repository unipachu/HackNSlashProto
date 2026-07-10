using UnityEngine;

public class FSMSt_PC_Walk : MonoBehaviour, IFSMSt
{
    [SerializeField] private PC pc;

    public void Enter()
    {
        pc.VisComponents.anims.Play_Walk();
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
        if (pc.AttackInput)
            pc.fSM.SwitchState(pc.fSMStates.atk_Jump);
        else if (pc.MoveInput == Vector2.zero)
            pc.fSM.SwitchState(pc.fSMStates.idle);
    }
}
