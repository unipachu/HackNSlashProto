using System;
using UnityEngine;

/// <summary>
/// Reusable finite state machine.
/// </summary>
public static class Fsm {
    /// <summary>
    /// Takes in a <paramref name="enterSt"/> which is supposed to initialize and return the state to be
    /// entered. <see cref="Func{TResult}"/> supports parameters through lambda expression:<br/>
    /// () => someState.Enter(SomeType someDataRequiredByState)<br/>
    /// NOTE: Calling this will force state transition. Before calling this you should probably check 
    /// <see cref="IFsmSt.CanSwitchTo{TState}"/>, or call <see cref="TrySwitchState{TState}"/>
    /// instead. (8.9.2026)
    /// </summary>
    public static void SwitchSt<TState>(
        Func<TState> enterSt,
        ref TState curSt,
        ref TState prevSt,
        ref bool isSwitchingSt,
        bool logMsg = false,
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
        bool logMsg = false,
        Action<TState> stSwitched = null
    ) where TState : IFsmSt{
        if (curSt != null && !curSt.CanSwitchTo<TState>())
            return false;
        SwitchSt(enterSt, ref curSt, ref prevSt, ref isSwitchingSt, logMsg, stSwitched);
        return true;
    }
}
