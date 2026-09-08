/// <summary>
/// Hand item which allows for combo moves.
/// </summary>
public interface IHandItem_Comboer : IHandItem {
    IComboNode LShldrComboStart { get; }
    IComboNode RShldrComboStart{ get; }
    IComboNode RTrgComboStart{ get; }
}
