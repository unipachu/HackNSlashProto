#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpreadsheetContainerBase), true)]
public sealed class SpreadsheetContainerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(12);

        SpreadsheetContainerBase container = (SpreadsheetContainerBase)target;

        using (new EditorGUI.DisabledScope(string.IsNullOrWhiteSpace(container.SpreadsheetId)))
        {
            if (GUILayout.Button("Import From Google Sheet"))
            {
                try
                {
                    SpreadsheetImporter.ImportInto(container);
                }
                catch (System.Exception exception)
                {
                    Debug.LogException(exception);
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
