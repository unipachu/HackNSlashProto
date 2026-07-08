/// <summary>
/// Sheet which has a column of unique ids for each row.
/// </summary>
public interface ISheetRowWithId
{
    string Id { get; }
}