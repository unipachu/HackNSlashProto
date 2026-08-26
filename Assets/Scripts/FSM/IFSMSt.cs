/// <summary>
/// Finite state machine state.
/// </summary>
public interface IFsmSt{
    void Enter(IFsmSt prevSt);
    
    void Exit();
    
    void PhysicsTick();
    
    void Tick();

    // TODO: Probably should use this much either. You can mutate the data in Tick and
    // TODO C: use Late Tick only for the animation events.
    void LateTick();

    // TODO: You probably should not try to change state from outside states to avoid
    // TODO C: problems in logic order.
    bool CanSwitchStTo(IFsmSt newSt);
}
