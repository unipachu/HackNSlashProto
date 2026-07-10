using UnityEngine;

/// <summary>
/// Playable capsule character FSM.
/// </summary>
public class FSM : MonoBehaviour
{
    public bool IsSwitchingState { get; private set; }
    public IFSMSt CurrentState { get; private set; }

    public void SwitchState(IFSMSt newSt)
    {
        Debug.AssertFormat(!IsSwitchingState, "Already switching state!", this);
        Debug.AssertFormat(CurrentState != newSt, "Tried to change to same state we are already in.", this);

        IsSwitchingState = true;
        if (CurrentState != null)
        {
            CurrentState.Exit();
        }
        newSt.Enter();
        CurrentState = newSt;


        IsSwitchingState = false;
    }

}
