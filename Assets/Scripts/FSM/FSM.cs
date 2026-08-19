using System;
using UnityEngine;

/// <summary>
/// Reusable finite state machine.
/// </summary>
// TODO: Check how the FSM in the VRTemplate project is implemented.
public class Fsm : MonoBehaviour {
    public event Action StSwitched;

    public IFsmSt CurSt { get; private set; }
    public bool IsSwitchingSt { get; private set; }
    public IFsmSt PrevSt { get; private set; }

    public void SwitchSt(IFsmSt newSt){
        Debug.Assert(!IsSwitchingSt, "Can't start a new state transition during another state transition!", this);
        //Debug.Assert(CurSt != newSt, "Tried to change to same state we are already in. " +
        //    "This can cause errors related to animation events overlapping during animation transition.", this);
        IsSwitchingSt = true;
        if (CurSt != null)
            CurSt.Exit();
        newSt.Enter(CurSt);
        PrevSt = CurSt;
        CurSt = newSt;
        //Debug.Log("Switched to state: " + newSt.GetType().Name, this);
        IsSwitchingSt = false;
        StSwitched?.Invoke();
    }
}
