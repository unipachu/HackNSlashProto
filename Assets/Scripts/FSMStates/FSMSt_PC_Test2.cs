using UnityEngine;

public class FSMSt_PC_Test2 : MonoBehaviour, IFSMSt
{
    [SerializeField] private PC pc;

    public void Enter(IFSMSt previousState)
    {
        pc.VisComponents.anims.Play_Test2();

    }

    public void Exit()
    {
    }

    public void PhysicsTick()
    {
    }

    public void Tick()
    {
        if (pc.Atk1Input)
            pc.fSM.SwitchState(pc.fSMStates.test1);
    }
}
