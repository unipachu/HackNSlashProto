using System;

/// <summary>
/// Marks columns cells as required, i.e. if cell is null or white space, it will throw an error.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class SheetColumnRequiredAttribute : Attribute
{
}
