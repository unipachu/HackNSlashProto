/// <summary>
/// Finite state machine state.
/// NOTE: Enter state logic needs to be passed as a parameter when state is changed.
/// </summary>
public interface IFsmSt {
    /// <summary>
    /// Can we switch to the specified state type?
    /// </summary>
    bool CanSwitchTo<TState>() where TState : IFsmSt;
    
    void Exit();
    
    void PhysicsTick();
    
    void Tick();

    void LateTick();
}
