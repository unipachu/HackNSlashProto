using UnityEngine;

public class FSMSt_PC_Test1 : MonoBehaviour, IFSMSt
{
    [SerializeField] private PC pc;

    public void Enter(IFSMSt previousState)
    {
        pc.VisComponents.anims.Play_Test1();
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
            pc.fSM.SwitchState(pc.fSMStates.test2);
    }
}
