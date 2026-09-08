using System;

/// <summary>
/// Node in a combo graph used by characters to do combo moves.
/// </summary>
public interface IComboNode {
    AnimInfo AnimInfo { get; }

    Func<IFsmSt_Cp> GetEnterFunc(int cpId);

    IComboNode GetNextNode(BufferableInput input);
}
