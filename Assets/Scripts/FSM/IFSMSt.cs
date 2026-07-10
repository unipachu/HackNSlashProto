/// <summary>
/// Finite state machine state.
/// </summary>
public interface IFSMSt
{
    void Enter(IFSMSt previousState);
    void Exit();
    void PhysicsTick();
    void Tick();
}
