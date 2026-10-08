/// <summary>
/// Hand item which allows for combo moves.
/// </summary>
public interface IHandItem_Comboer : IHandItem {
    IComboNode_CpHumanoid LShldrComboStart { get; }
    IComboNode_CpHumanoid RShldrComboStart{ get; }
    IComboNode_CpHumanoid RTrgComboStart{ get; }
}
