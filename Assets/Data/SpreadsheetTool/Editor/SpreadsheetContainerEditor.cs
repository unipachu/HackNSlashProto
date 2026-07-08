#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom inspector view for a SpreadsheetContainerBase. Includes a button for importing <br/>
/// NOTE: This is also used for child classes of <see cref="SheetContainerBase"/>
/// </summary>
[CustomEditor(typeof(SheetContainerBase), true)]
// This is sealed for fun mostly.
public sealed class SpreadsheetContainerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // This draws all serialize fields like Unity normally would.
        DrawDefaultInspector();

        // Cosmetic spacing before the buttons.
        EditorGUILayout.Space(12);

        // Target is the object being inspected, but saved as an Object, so we need to cast it to SpreadsheetContainerBase.
        SheetContainerBase container = (SheetContainerBase)target;

        // If the string is empty, we grey out the button.
        using (new EditorGUI.DisabledScope(string.IsNullOrWhiteSpace(container.SpreadsheetId)))
        {
            // This both draws the button and checks if it was clicked.
            if (GUILayout.Button("Import From Google Sheet"))
            {
                try
                {
                    SpreadsheetImporter.ImportInto(container);
                }
                catch (System.Exception exception)
                {
                    Debug.LogException(exception);
                    // Displays a pop up if importing fails.
                    EditorUtility.DisplayDialog(
                        "Spreadsheet Import Failed",
                        exception.Message,
                        "OK"
                    );
                }
            }
        }

        // TODO: JSON exporter is useless for this project. Remove code.
        //if (GUILayout.Button("Export To JSON"))
        //{
        //    try
        //    {
        //        SpreadsheetJsonExporter.ExportToJson(container);
        //    }
        //    catch (System.Exception exception)
        //    {
        //        Debug.LogException(exception);
        //        EditorUtility.DisplayDialog(
        //            "JSON Export Failed",
        //            exception.Message,
        //            "OK"
        //        );
        //    }
        //}
    }
}
#endif