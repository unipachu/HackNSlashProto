using UnityEditor;
using UnityEngine;

/// <summary>
/// Makes a public field visible, but readonly in the inspector.
/// </summary>
//TODO: For some reasons arrays and lists get always shown normally when public, even when this script tells inspector to show them otherwise.
//TODO: You can make sure that if the object reference is a class, the inspector will show the lowest child in the inheritance tree of that class.
[CustomPropertyDrawer(typeof(ShowOnlyAttribute))]
public class ShowOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string valueStr = "";
        if (property.propertyType == SerializedPropertyType.ObjectReference)
        {
            // For object reference properties, check if the value isn't null
            if (property.objectReferenceValue != null)
            {
                EditorGUI.LabelField(position, label.text, property.objectReferenceValue.name);
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "NULL");
            }
        }
        switch (property.propertyType)
        {
            /*
            case SerializedPropertyType.Generic:
                //valueStr = property.name;
                break;
            */
            case SerializedPropertyType.Integer:
                valueStr = property.intValue.ToString();
                break;
            case SerializedPropertyType.Boolean:
                valueStr = property.boolValue.ToString();
                break;
            case SerializedPropertyType.Float:
                valueStr = property.floatValue.ToString("0.00000");
                break;
            case SerializedPropertyType.String:
                valueStr = property.stringValue;
                break;
            case SerializedPropertyType.Color:
                valueStr = property.colorValue.ToString();
                break;

            case SerializedPropertyType.ObjectReference:
                valueStr = "(Rect property type not yet supported, you might want to uncomment the line below)";
                // For object reference properties, check if the value isn't null
                if (property.objectReferenceValue != null)
                {
                    valueStr = property.objectReferenceValue.name;
                }
                else
                {
                    valueStr = "NULL Object Reference";
                }
                break;

            case SerializedPropertyType.LayerMask:
                int layerMaskValue = property.intValue;
                valueStr = "Layers used: ";
                for (int i = 0; i < 32; i++)
                {
                    if (((1 << i) & layerMaskValue) != 0)
                    {
                        valueStr += (LayerMask.LayerToName(i) + " ");
                    }
                }
                break;
            case SerializedPropertyType.Enum:
                valueStr = property.enumDisplayNames[property.enumValueIndex];
                break;
            case SerializedPropertyType.Vector2:
                valueStr = property.vector2Value.ToString();
                break;
            case SerializedPropertyType.Vector3:
                valueStr = property.vector3Value.ToString();
                break;
            case SerializedPropertyType.Vector4:
                valueStr = property.vector4Value.ToString();
                break;
            /*
        case SerializedPropertyType.Rect:
            valueStr = "(Rect property type not yet supported, you might want to uncomment the line below)";
            //property.rectValue.ToString();
            break;
        case SerializedPropertyType.ArraySize:
            valueStr = "(ArraySize property type not yet supported)";
            break;
        case SerializedPropertyType.Character:
            valueStr = "(Character property type not yet supported)";
            break;
        case SerializedPropertyType.AnimationCurve:
            valueStr = "(AnimationCurve property type not yet supported)";
            break;
        case SerializedPropertyType.Bounds:
            valueStr = "(Bounds property type not yet supported)";
            break;
        case SerializedPropertyType.Gradient:
            valueStr = "(Gradient property type not yet supported)";
            break;
            */
            case SerializedPropertyType.Quaternion:
                valueStr = property.quaternionValue.eulerAngles.ToString();
                break;
            /*
        case SerializedPropertyType.ExposedReference:
            valueStr = "(ExposedReference property type not yet supported, you might want to uncomment the line below)";
            //valueStr = property.objectReferenceValue.name;
            break;
        case SerializedPropertyType.FixedBufferSize:
            valueStr = "(FixedBufferSize property type not yet supported, you might want to uncomment the line below)";
            //valueStr = property.fixedBufferSize.ToString();
            break;
        case SerializedPropertyType.Vector2Int:
            valueStr = "(Vector2Int property type not yet supported, you might want to uncomment the line below)";
            //valueStr = property.vector2IntValue.ToString();
            break;
        case SerializedPropertyType.Vector3Int:
            valueStr = "(Vector3Int property type not yet supported, you might want to uncomment the line below)";
            //valueStr = property.vector3IntValue.ToString();
            break;
        case SerializedPropertyType.RectInt:
            valueStr = "(RectInt property type not yet supported, you might want to uncomment the line below)";
            //valueStr = property.rectIntValue.ToString();
            break;
        case SerializedPropertyType.BoundsInt:
            valueStr = "(BoundsInt property type not yet supported, you might want to uncomment the line below)";
            //valueStr = property.boundsIntValue.ToString();
            break;
        case SerializedPropertyType.ManagedReference:
            valueStr = "(ManagedReference property type not yet supported, you might want to uncomment the line below)";
            //valueStr = property.managedReferenceFieldTypename;
            break;
        case SerializedPropertyType.Hash128:
            valueStr = "(Hash128 property type not yet supported, you might want to uncomment the line below)";
            //valueStr = property.hash128Value.ToString();
            break;
            */
            default:
                valueStr = "(property type not supported)";
                break;
        }

        EditorGUI.LabelField(position, label.text, valueStr);
    }
}

