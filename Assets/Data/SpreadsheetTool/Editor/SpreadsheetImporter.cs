#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
// Needed for CultureInfo.InvariantCulture.
using System.Globalization;
// Needed for finding fields so that we don't need separate code for each spreadsheet.
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Copies data from Google Sheets into a scriptable object.
/// </summary>
public static class SpreadsheetImporter
{
    /// <summary>
    /// Imports Google Sheets spreadsheet sheets into a <see cref="SheetContainerBase"/>.
    /// </summary>
    public static void ImportInto(SheetContainerBase container)
    {
        // Record object to support "undo", in case you want to undo the import.
        Undo.RecordObject(container, "Import Spreadsheet Data");

        if (container == null)
            throw new ArgumentNullException(nameof(container));

        Type containerType = container.GetType();

        // Reflection is used to get container instance fields (public, protected and private) regardless of container type.
        FieldInfo[] fields = containerType.GetFields(
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.NonPublic
        );

        int importedPageCount = 0;

        foreach (FieldInfo field in fields)
        {
            // Only use fields with a page attribute.
            SheetAttribute pageAttribute =
                field.GetCustomAttribute<SheetAttribute>();

            if (pageAttribute == null)
                continue;

            ImportSheet(container, field, pageAttribute.PageName);
            importedPageCount++;
        }

        if (importedPageCount == 0)
        {
            Debug.LogWarning(
                $"No fields with [SpreadsheetPageAttribute] were found on {container.GetType().Name}."
            );
        }

        // Now that the row data has been imported to a list, we want to rebuild our dictionary lookup table so that we can instantly use the dictionary to try and get row values from the list.
        if (container is SheetContainerBase lookupContainer)
        {
            lookupContainer.RebuildLookups();
        }

        // Marks scriptable object as having unsaved changes, since Unity doesn't automatically notice that assets have been changed by reflection, e.g.:
        // listField.SetValue(container, list);
        // When scriptable object is set dirty, Unity seems to automatically save it.
        EditorUtility.SetDirty(container);

        // Below are two lines that can be used to manually save dirty assets.
        // Scriptable objects seem to save changes automatically as part of the Editor workflow so the lines below is not needed.
        //AssetDatabase.SaveAssets();
        //AssetDatabase.SaveAssetIfDirty(container);

        Debug.Log($"Imported spreadsheet data into {container.name}.");
    }

    /// <summary>
    /// Downloads csv file, parses it, creates row objects and writes the completed list into the container's corresponding field.
    /// </summary>
    /// <param name="container">Container we are importing the sheet into.</param>
    /// <param name="listField">Reflection object representing container field (with <see cref="SheetAttribute"/>) that should recieve the imported data.</param>
    /// <param name="sheetName">Name of the sheet in the spreadsheet with the data to be imported to the container.</param>
    private static void ImportSheet(
        SheetContainerBase container,
        FieldInfo listField,
        string sheetName)
    {
        // Type of the field representing a table row needs to be List.
        Type listType = listField.FieldType;
        if (!listType.IsGenericType || listType.GetGenericTypeDefinition() != typeof(List<>))
        {
            throw new InvalidOperationException(
                $"Field '{listField.Name}' must be a List<T>."
            );
        }

        // List item type i.e. row type.
        Type rowType = listType.GetGenericArguments()[0];

        string csv = GoogleSheetCsvDownloader.DownloadCsv(
            container.SpreadsheetId,
            sheetName
        );

        List<List<string>> table = CsvParser.Parse(csv);

        if (table.Count == 0)
        {
            Debug.LogWarning($"Sheet '{sheetName}' is empty.");
            return;
        }

        List<string> headers = table[0];
        Dictionary<string, int> headerToIndex = BuildHeaderLookup(headers);

        // Create list of the same type as the field in the container we are importing to.
        // This represents list of all table rows.
        IList list = (IList)Activator.CreateInstance(listType);

        // Outer loop goes through each row (below the header row).
        for (int rowIndex = 1; rowIndex < table.Count; rowIndex++)
        {
            List<string> row = table[rowIndex];

            if (IsEmptyRow(row))
                continue;

            object rowObject = Activator.CreateInstance(rowType);

            FieldInfo[] rowFields = rowType.GetFields(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic
            );

            // Inner loop goes through each cell in a row
            foreach (FieldInfo rowField in rowFields)
            {
                string columnName = GetColumnName(rowField);

                bool hasColumn = headerToIndex.TryGetValue(columnName, out int columnIndex);

                bool isRequired =
                    rowField.GetCustomAttribute<SheetRequiredAttribute>() != null;

                if (!hasColumn)
                {
                    if (isRequired)
                    {
                        throw new InvalidOperationException(
                            $"Sheet '{sheetName}', row {rowIndex + 1}: required column '{columnName}' was not found."
                        );
                    }

                    continue;
                }

                // In case Google Sheets doesn't export rows with trailing commans up to the final relevant column. TODO: Check if this is needed.
                string cellValue = columnIndex < row.Count ? row[columnIndex] : "";

                // Does the cell actually contain data.
                if (isRequired && string.IsNullOrWhiteSpace(cellValue))
                {
                    throw new InvalidOperationException(
                        $"Sheet '{sheetName}', row {rowIndex + 1}, column '{columnName}': value is required."
                    );
                }

                object convertedValue = ConvertCellValue(
                    cellValue,
                    rowField.FieldType,
                    rowField,
                    sheetName,
                    rowIndex + 1,
                    columnName
                );

                rowField.SetValue(rowObject, convertedValue);
            }

            list.Add(rowObject);
        }

        listField.SetValue(container, list);

        Debug.Log($"Imported {list.Count} row(s) from sheet '{sheetName}'.");
    }

    /// <summary>
    /// Creates dictionary where keys are the table's header row's column names, and values are their indices.
    /// </summary>
    private static Dictionary<string, int> BuildHeaderLookup(List<string> headers)
    {
        Dictionary<string, int> lookup = new(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < headers.Count; i++)
        {
            string header = headers[i]?.Trim();

            if (string.IsNullOrWhiteSpace(header))
                continue;

            if (lookup.ContainsKey(header))
            {
                Debug.LogWarning(
                    $"Duplicate spreadsheet header '{header}' found. The first column will be used."
                );

                continue;
            }

            lookup.Add(header, i);
        }

        return lookup;
    }

    /// <summary>
    /// Column name is either the name of the field, or the string denoted by <see cref="SheetColumnAttribute"/> if such exists.
    /// </summary>
    /// <param name="field">Field representing table column.</param>
    private static string GetColumnName(FieldInfo field)
    {
        SheetColumnAttribute columnAttribute =
            field.GetCustomAttribute<SheetColumnAttribute>();

        if (columnAttribute != null && !string.IsNullOrWhiteSpace(columnAttribute.ColumnName))
            return columnAttribute.ColumnName;

        return field.Name;
    }

    /// <returns>True if all cells are null or white space.</returns>
    private static bool IsEmptyRow(List<string> row)
    {
        foreach (string cell in row)
        {
            if (!string.IsNullOrWhiteSpace(cell))
                return false;
        }

        return true;
    }

    // Converts sheet cell value to a field.
    private static object ConvertCellValue(
        string value,
        Type targetType,
        FieldInfo targetField,
        string sheetName,
        int rowNumber,
        string columnName)
    {
        value = value?.Trim() ?? "";

        bool isAssetPath =
            targetField.GetCustomAttribute<SheetAssetPathAttribute>() != null;

        if (isAssetPath)
        {
            return ConvertAssetPath(
                value,
                targetType,
                sheetName,
                rowNumber,
                columnName
            );
        }

        if (targetType == typeof(string))
            return value;

        if (targetType == typeof(int))
            return string.IsNullOrEmpty(value)
                ? 0
                : int.Parse(value, CultureInfo.InvariantCulture);

        if (targetType == typeof(float))
            return string.IsNullOrEmpty(value)
                ? 0f
                : float.Parse(value, CultureInfo.InvariantCulture);

        if (targetType == typeof(double))
            return string.IsNullOrEmpty(value)
                ? 0d
                : double.Parse(value, CultureInfo.InvariantCulture);

        if (targetType == typeof(bool))
        {
            if (string.IsNullOrEmpty(value))
                return false;

            return value.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                   value.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                   value == "1";
        }

        if (targetType.IsEnum)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Activator.CreateInstance(targetType);

            return Enum.Parse(targetType, value, ignoreCase: true);
        }

        throw new NotSupportedException(
            $"Sheet '{sheetName}', row {rowNumber}, column '{columnName}': type '{targetType.Name}' is not supported."
        );
    }

    /// <summary>
    /// When converting cell value, in the case the cell is an asset path, we use this function
    /// </summary>
    private static object ConvertAssetPath(
        string value,
        Type targetType,
        string sheetName,
        int rowNumber,
        string columnName)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (!typeof(UnityEngine.Object).IsAssignableFrom(targetType))
        {
            throw new InvalidOperationException(
                $"Sheet '{sheetName}', row {rowNumber}, column '{columnName}': [SpreadsheetAssetPath] can only be used on UnityEngine.Object fields."
            );
        }

        UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(value, targetType);

        if (asset == null)
        {
            Debug.LogWarning(
                $"Sheet '{sheetName}', row {rowNumber}, column '{columnName}': could not load asset at path '{value}'."
            );
        }

        return asset;
    }
}
#endif