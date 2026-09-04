/// <summary>
/// Polymorfism for hand items handles.
/// </summary>
public interface IHandItemHandle {
    int Id { get; }
    HandItemData HandItemData { get; }
    HandItemDataT HandItemDataT { get; }
}
