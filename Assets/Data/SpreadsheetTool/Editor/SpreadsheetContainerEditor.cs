#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom inspector view for a SpreadsheetContainerBase. Includes a button for importing <br/>
/// NOTE: This is also used for child classes of <see cref="SpreadsheetContainerBase"/>
/// </summary>
[CustomEditor(typeof(SpreadsheetContainerBase), true)]
public class SpreadsheetContainerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // This draws all serialize fields like Unity normally would.
        DrawDefaultInspector();

        // Cosmetic spacing before the buttons.
        EditorGUILayout.Space(12);

        // Target is the object being inspected, but saved as an Object, so we need to cast it to SpreadsheetContainerBase.
        SpreadsheetContainerBase container = (SpreadsheetContainerBase)target;

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
    }
}
#endif