/// <summary>
/// Capsule pawn fsm state. Includes ability to handle capsule pawn animation events.
/// </summary>
public interface IFsmSt_Cp : IFsmSt {
    void HandleAnimEvent(CpAnimEventT animEvent);
}
