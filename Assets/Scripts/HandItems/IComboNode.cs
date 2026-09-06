using System;

public interface IComboNode {
    AnimInfo GetAnimInfo();

    Func<IFsmSt_Cp> GetEnterFunc(int cpId);

    IComboNode GetNextNode(BufferableInput input);

    /// <summary>
    /// If -1, then doesn't have node.
    /// </summary>
    int NextNodeI(BufferableInput input);
}
