using System;

/// <summary>
/// Node in a combo graph used by characters to do combo moves.<br/>
/// Combo node is a coupling of an action state, an animation and (if they implement
/// <see cref="IComboNodeTransitionsHolder"/>) information about possible transitions to next combo node.
/// Combo nodes allow modular reuse of animations and action states and with those, creation of
/// weapon-specific combo move sequences (see <see cref="ComboGraphFactory"/>).
/// </summary>
public interface IComboNode {
    AnimInfo AnimInfo { get; }

    Func<IFsmSt_Cp> GetEnterFunc(int cpI);

    IComboNode GetNextNode(BufferableInput input);
}
