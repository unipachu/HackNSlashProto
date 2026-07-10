using UnityEngine;

public class FSMSt_PC_Idle : MonoBehaviour, IFSMSt
{
    [SerializeField] PC pc;

    public void Enter()
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
        if (pc.AttackInput)
            pc.fSM.SwitchState(pc.fSMStates.atk_Jump);
        else if (pc.MoveInput != Vector2.zero)
            pc.fSM.SwitchState(pc.fSMStates.walk);
    }
}
