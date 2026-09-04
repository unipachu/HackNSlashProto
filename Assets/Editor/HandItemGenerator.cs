#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class HandItemGenerator {
    const string EnumPath = "Assets/Scripts/Generated/HandItemT.cs";
    const string CatalogPath = "Assets/SoData/HandItems/HandItemCatalog.asset";

    [MenuItem("Tools/Hand Items/Regenerate")]
    public static void Generate() {
        List<So_HandItem> items = FindItems();
        if (!Validate(items))
            return;
        items.Sort((a, b) =>
            string.Compare(
                a.Id,
                b.Id,
                StringComparison.Ordinal
            )
        );
        GenerateEnum(items);
        GenerateCatalog(items);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log(
            $"Generated HandItemT and HandItemCatalog " +
            $"with {items.Count} hand items."
        );
    }

    static List<So_HandItem> FindItems() {
        List<So_HandItem> items = new();
        string[] guids = AssetDatabase.FindAssets("t:ScriptableObject");
        foreach (string guid in guids) {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            So_HandItem item = AssetDatabase.LoadAssetAtPath<So_HandItem>(path);
            if (item != null)
                items.Add(item);
        }
        return items;
    }

    static bool Validate(List<So_HandItem> items) {
        HashSet<string> ids = new();
        foreach (So_HandItem item in items) {
            if (string.IsNullOrWhiteSpace(item.Id)) {
                Debug.LogError(
                    $"Hand item '{item.name}' has an empty ID.",
                    item
                );
                return false;
            }
            if (!IsValidIdentifier(item.Id)) {
                Debug.LogError(
                    $"Hand item '{item.name}' has invalid ID " +
                    $"'{item.Id}'. IDs must be valid C# identifiers.",
                    item
                );
                return false;
            }
            if (!ids.Add(item.Id)) {
                Debug.LogError(
                    $"Duplicate HandItem ID: '{item.Id}'."
                );
                return false;
            }
        }
        return true;
    }

    static bool IsValidIdentifier(string id) {
        if (string.IsNullOrEmpty(id))
            return false;
        if (!char.IsLetter(id[0]) && id[0] != '_')
            return false;
        for (int i = 1; i < id.Length; i++) {
            if (!char.IsLetterOrDigit(id[i]) && id[i] != '_')
                return false;
        }
        if (IsCSharpKeyword(id))
            return false;
        return true;
    }

    static bool IsCSharpKeyword(string value) {
        switch (value) {
            case "abstract":
            case "as":
            case "base":
            case "bool":
            case "break":
            case "byte":
            case "case":
            case "catch":
            case "char":
            case "checked":
            case "class":
            case "const":
            case "continue":
            case "decimal":
            case "default":
            case "delegate":
            case "do":
            case "double":
            case "else":
            case "enum":
            case "event":
            case "explicit":
            case "extern":
            case "false":
            case "finally":
            case "fixed":
            case "float":
            case "for":
            case "foreach":
            case "goto":
            case "if":
            case "implicit":
            case "in":
            case "int":
            case "interface":
            case "internal":
            case "is":
            case "lock":
            case "long":
            case "namespace":
            case "new":
            case "null":
            case "object":
            case "operator":
            case "out":
            case "override":
            case "params":
            case "private":
            case "protected":
            case "public":
            case "readonly":
            case "ref":
            case "return":
            case "sbyte":
            case "sealed":
            case "short":
            case "sizeof":
            case "stackalloc":
            case "static":
            case "string":
            case "struct":
            case "switch":
            case "this":
            case "throw":
            case "true":
            case "try":
            case "typeof":
            case "uint":
            case "ulong":
            case "unchecked":
            case "unsafe":
            case "ushort":
            case "using":
            case "virtual":
            case "void":
            case "volatile":
            case "while":
            case "add":
            case "alias":
            case "ascending":
            case "async":
            case "await":
            case "by":
            case "descending":
            case "dynamic":
            case "equals":
            case "from":
            case "get":
            case "global":
            case "group":
            case "into":
            case "join":
            case "let":
            case "nameof":
            case "on":
            case "orderby":
            case "partial":
            case "remove":
            case "select":
            case "set":
            case "unmanaged":
            case "value":
            case "var":
            case "when":
            case "where":
            case "yield":
                return true;
            default:
                return false;
        };
    }

    static void GenerateEnum(List<So_HandItem> items) {
        string directory = Path.GetDirectoryName(EnumPath);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        StringBuilder sb = new();
        sb.AppendLine("// AUTO-GENERATED. DO NOT EDIT.");
        sb.AppendLine();
        sb.AppendLine("public enum HandItemT : byte {");
        for (int i = 0; i < items.Count; i++) {
            sb.Append("    ");
            sb.Append(items[i].Id);
            sb.Append(" = ");
            sb.Append(i);
            sb.AppendLine(",");
        }
        sb.AppendLine("}");
        File.WriteAllText(EnumPath, sb.ToString());
    }

    static void GenerateCatalog(List<So_HandItem> items) {
        HandItemCatalog catalog =
            AssetDatabase.LoadAssetAtPath<HandItemCatalog>(
                CatalogPath
            );
        if (catalog == null) {
            string directory =
                Path.GetDirectoryName(CatalogPath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            catalog = ScriptableObject.CreateInstance<HandItemCatalog>();
            AssetDatabase.CreateAsset(
                catalog,
                CatalogPath
            );
        }
        catalog.SetItems(items);
        EditorUtility.SetDirty(catalog);
    }
}
#endif
