/// <summary>
/// Finite state machine state.
/// </summary>
public interface IFsmSt{
    void Enter(IFsmSt prevSt);
    
    void Exit();
    
    void PhysicsTick();
    
    void Tick();

    void LateTick();

    bool CanSwitchStTo(IFsmSt newSt);
}
