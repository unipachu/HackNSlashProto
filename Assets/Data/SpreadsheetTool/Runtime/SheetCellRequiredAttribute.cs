using System;

/// <summary>
/// Marks a cell as being required, i.e. if cell is null or white space, it will throw an error.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class SheetCellRequiredAttribute : Attribute
{
}
