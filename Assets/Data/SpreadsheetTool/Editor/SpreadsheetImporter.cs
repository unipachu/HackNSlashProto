#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class SpreadsheetImporter
{
    public static void ImportInto(SpreadsheetContainerBase container)
    {
        if (container == null)
            throw new ArgumentNullException(nameof(container));

        Type containerType = container.GetType();

        FieldInfo[] fields = containerType.GetFields(
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.NonPublic
        );

        foreach (FieldInfo field in fields)
        {
            SpreadsheetPageAttribute pageAttribute =
                field.GetCustomAttribute<SpreadsheetPageAttribute>();

            if (pageAttribute == null)
                continue;

            ImportPage(container, field, pageAttribute.PageName);
        }

        EditorUtility.SetDirty(container);
        AssetDatabase.SaveAssets();

        Debug.Log($"Imported spreadsheet data into {container.name}.");
    }

    private static void ImportPage(
        SpreadsheetContainerBase container,
        FieldInfo listField,
        string pageName)
    {
        Type listType = listField.FieldType;

        if (!listType.IsGenericType || listType.GetGenericTypeDefinition() != typeof(List<>))
        {
            throw new InvalidOperationException(
                $"Field '{listField.Name}' must be a List<T>."
            );
        }

        Type rowType = listType.GetGenericArguments()[0];

        string csv = GoogleSheetCsvDownloader.DownloadCsv(
            container.SpreadsheetId,
            pageName
        );

        List<List<string>> table = CsvParser.Parse(csv);

        if (table.Count == 0)
        {
            Debug.LogWarning($"Sheet '{pageName}' is empty.");
            return;
        }

        List<string> headers = table[0];

        IList list = (IList)Activator.CreateInstance(listType);

        for (int rowIndex = 1; rowIndex < table.Count; rowIndex++)
        {
            List<string> row = table[rowIndex];

            if (IsEmptyRow(row))
                continue;

            object rowObject = Activator.CreateInstance(rowType);

            for (int columnIndex = 0; columnIndex < headers.Count; columnIndex++)
            {
                string header = headers[columnIndex];

                if (string.IsNullOrWhiteSpace(header))
                    continue;

                string value = columnIndex < row.Count ? row[columnIndex] : "";

                FieldInfo rowField = rowType.GetField(
                    header,
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic
                );

                if (rowField == null)
                {
                    Debug.LogWarning(
                        $"Sheet '{pageName}', row type '{rowType.Name}': no field named '{header}'."
                    );

                    continue;
                }

                object convertedValue = ConvertCellValue(value, rowField.FieldType);
                rowField.SetValue(rowObject, convertedValue);
            }

            list.Add(rowObject);
        }

        listField.SetValue(container, list);

        Debug.Log($"Imported {list.Count} row(s) from sheet '{pageName}'.");
    }

    private static bool IsEmptyRow(List<string> row)
    {
        foreach (string cell in row)
        {
            if (!string.IsNullOrWhiteSpace(cell))
                return false;
        }

        return true;
    }

    private static object ConvertCellValue(string value, Type targetType)
    {
        value = value?.Trim() ?? "";

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
            return Enum.Parse(targetType, value, ignoreCase: true);

        throw new NotSupportedException(
            $"Type '{targetType.Name}' is not supported yet."
        );
    }
}
#endif
