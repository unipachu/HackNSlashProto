/// <summary>
/// Finite state machine state.
/// </summary>
public interface IFSMSt
{
    void Enter();
    void Exit();
    void PhysicsTick();
    void Tick();
}
