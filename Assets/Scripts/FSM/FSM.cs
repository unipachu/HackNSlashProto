using System;
using UnityEngine;

// TODO: Ask chatgpt if this summary is correct.
/// <summary>
/// Reusable finite state machine. Allows states to be initialized with different data with a delegate enter
/// state method.
/// </summary>
// TODO: You will likely need separate references to states since we want to keep them generic here (but not
// TODO C: elsewhere), but for the other fields of this class: make them into a struct, move them to CpMgr,
// TODO C: and make this class static.
public static class Fsm {
    /// <summary>
    /// Takes in a "EnterState" method which returns the state to be entered. Func supports parameters through
    /// lambda expression: () => someState.Enter(SomeType someDataRequiredByState) <br/>
    /// NOTE: This will force state transition. Before calling this you should probably check 
    /// <see cref="IFsmSt.CanSwitchTo{TState}"/>. (4.9.2026)
    /// </summary>
    public static void SwitchSt<TState>(
        Func<TState> enterSt,
        ref TState curSt,
        ref TState prevSt,
        ref bool isSwitchingSt,
        bool logMsg,
        Action<TState> stSwitched = null
    ) where TState : IFsmSt {
#if UNITY_EDITOR
        Debug.Assert(
            !isSwitchingSt,
            $"Tried to switch state to {typeof(TState).Name} while already in state transition!"
        );
#endif
        isSwitchingSt = true;
        if (curSt != null)
            curSt.Exit();
        prevSt = curSt;
        // NOTE: during enter state logic PrevSt and CurSt point to the same state! (6.9.2026)
        curSt = enterSt();
#if UNITY_EDITOR
        if (logMsg)
            Debug.Log("Switched to state: " + curSt.GetType().Name);
#endif
        isSwitchingSt = false;
        stSwitched?.Invoke(curSt);
    }

    /// <summary>
    /// Only switched state if current state allows it.
    /// </summary>
    public static bool TrySwitchState<TState>(
        Func<TState> enterSt,
        ref TState curSt,
        ref TState prevSt,
        ref bool isSwitchingSt,
        bool logMsg,
        Action<TState> stSwitched = null
    ) where TState : IFsmSt{
        if (curSt != null && !curSt.CanSwitchTo<TState>())
            return false;
        SwitchSt(enterSt, ref curSt, ref prevSt, ref isSwitchingSt, logMsg, stSwitched);
        return true;
    }
}
