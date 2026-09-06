using UnityEngine;

/// <summary>
/// Hand item which allows for combo moves.
/// </summary>
// TODO: Rename to IHandItem_ComboUser
public interface IComboUser : IHandItem {
    IComboNode LShldrComboStart { get; }
    IComboNode RShldrComboStart{ get; }
    IComboNode RTrgComboStart{ get; }
}
