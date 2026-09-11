/// <summary>
/// Finite state machine state.
/// NOTE: Enter state logic needs to be passed as a parameter when state is changed.
/// </summary>
public interface IFsmSt {
    /// <summary>
    /// A state can change to any state from within its own logic, but if you want to switch state from outside
    /// the state logic, then call this first.
    /// </summary>
    bool CanSwitchTo<TState>() where TState : IFsmSt;
    
    void Exit();
    
    void LateTick();
    
    void PhysicsTick();
    
    void Tick();
}
