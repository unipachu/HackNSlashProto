using UnityEngine;

/// <summary>
/// Reusable finite state machine.
/// </summary>
// TODO: Check how the FSM in the VRTemplate project is implemented.
public class Fsm : MonoBehaviour {
    public bool IsSwitchingSt { get; private set; }
    public IFsmSt CurSt { get; private set; }

    public void SwitchSt(IFsmSt newSt){
        Debug.AssertFormat(!IsSwitchingSt, "Already switching state!", this);
        Debug.AssertFormat(CurSt != newSt, "Tried to change to same state we are already in. " +
            "This can cause errors related to animation events overlapping during animation transition.", this);
        IsSwitchingSt = true;
        if (CurSt != null)
            CurSt.Exit();
        newSt.Enter(CurSt);
        CurSt = newSt;
        Debug.Log("Switched to state: " + newSt.GetType().Name, this);
        IsSwitchingSt = false;
    }
}
