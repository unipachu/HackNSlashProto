// Portions of this file are based on NorskaLib.Spreadsheets.
// Original project: https://github.com/NorskaGames/NorskaLib.Spreadsheets
// Original copyright: Copyright (c) 2022 NorskaGames.
// Licensed under the MIT License

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

// NOTE: VISUAL ONLY ATTRIBUTE!
[CustomPropertyDrawer(typeof(SheetInspectorOnlyAttribute))]
public class SheetInspectorOnlyDrawer : PropertyDrawer {
    private static GUIStyle labelStyle;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        label.text += " (Inspector Only)";
        if (labelStyle == null) {
            labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.normal.textColor = new Color(1f, 0.65f, 0.2f);
        }
        int controlId = GUIUtility.GetControlID(FocusType.Passive);
        Rect fieldPosition = EditorGUI.PrefixLabel(position, controlId, label, labelStyle);
        EditorGUI.PropertyField(fieldPosition, property, GUIContent.none, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
#endif