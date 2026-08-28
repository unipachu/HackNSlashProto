using System;
using UnityEngine;

/// <summary>
/// Reusable finite state machine.
/// </summary>
public class Fsm : MonoBehaviour {
    public event Action StSwitched;

    public bool logMsg;

    public IFsmSt CurSt { get; private set; }
    public bool IsSwitchingSt { get; private set; }
    public IFsmSt PrevSt { get; private set; }

    public void SwitchSt(IFsmSt newSt){
#if UNITY_EDITOR
        Debug.Assert(!IsSwitchingSt, "Can't start a new state transition during another state transition!", this);
        if (newSt == CurSt)
            Debug.LogWarning("Tried to change to the same state the FSM was already in. " 
                + "If this was intended, ignore this.", this);
#endif
        //    "This can cause errors related to animation events overlapping during animation transition.", this);
        IsSwitchingSt = true;
        if (CurSt != null)
            CurSt.Exit();
        newSt.Enter(CurSt);
        PrevSt = CurSt;
        CurSt = newSt;
#if UNITY_EDITOR
        if (logMsg)
            Debug.Log("Switched to state: " + newSt.GetType().Name, this);
#endif
        IsSwitchingSt = false;
        StSwitched?.Invoke();
    }
}
