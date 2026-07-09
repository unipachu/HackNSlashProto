/// <summary>
/// Sheet which has a column of unique ids for each row.
/// </summary>
public interface ISheetRowWithId
{
    /// <summary>
    /// Row cell which is unique to each row of the sheet.
    /// </summary>
    string Id { get; }
}