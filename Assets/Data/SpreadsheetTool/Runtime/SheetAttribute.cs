using System;

/// <summary>
/// Used to link a List of ISpreadsheetRowWithId to a spreadsheet's sheet based on the sheet name.
/// </summary>
/// 

[AttributeUsage(AttributeTargets.Field)]
public sealed class SheetAttribute : Attribute
{
    public string PageName { get; }

    public SheetAttribute(string pageName)
    {
        PageName = pageName;
    }
}