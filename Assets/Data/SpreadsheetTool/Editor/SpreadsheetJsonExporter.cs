#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

// TODO: You probably won't need this so delete the class.
public static class SpreadsheetJsonExporter
{
    public static void ExportToJson(SheetContainerBase container)
    {
        if (container == null)
            return;

        string path = EditorUtility.SaveFilePanel(
            "Export Spreadsheet Database To JSON",
            Application.dataPath,
            container.name + ".json",
            "json"
        );

        if (string.IsNullOrWhiteSpace(path))
            return;

        string json = JsonUtility.ToJson(container, prettyPrint: true);

        File.WriteAllText(path, json);

        Debug.Log($"Exported spreadsheet database JSON to: {path}");

        AssetDatabase.Refresh();
    }
}
#endif
