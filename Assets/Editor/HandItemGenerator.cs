#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Creates item catalogue and enum based on <see cref="IHandItem"/> prefabs
/// in <see cref="handItemPrefabsPath"/>.
/// </summary>
public static class HandItemGenerator {
    const string enumPath = "Assets/Scripts/Generated/HandItemT.cs";
    const string catalogPath = "Assets/SoData/Generated/HandItemCatalog.asset";
    const string handItemPrefabsPath = "Assets/Prefabs/HandItems";
    const string enumName = "HandItemT";

    [MenuItem("Tools/Hand Items/Generate catalogue and enum (NOTE: Saves all unsaved assets!)")]
    public static void Generate() {
        List<GameObject> items = FindHandItemPrefabs();
        if (!EditorUtils.Validate(items))
            return;
        // We want the items in alphabetical order.
        items.Sort((a, b) =>
            string.Compare(
                a.name,
                b.name,
                StringComparison.Ordinal
            )
        );
        EditorUtils.GenerateEnum(items, enumName, enumPath);
        EditorUtils.GenerateCatalog(items, catalogPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log(
            $"Generated HandItemT and HandItemCatalog " +
            $"with {items.Count} hand items."
        );
    }

    /// <summary>
    /// Finds hand item prefabs in <see cref="handItemPrefabsPath"/>.
    /// </summary>
    static List<GameObject> FindHandItemPrefabs() {
        List<GameObject> items = new();
        string[] guids = AssetDatabase.FindAssets("t:GameObject", new[] { handItemPrefabsPath });
        foreach (string guid in guids) {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject item = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (item == null) {
                Debug.LogError("Loaded Item was null! I'm not sure how this is possible.");
                continue;
            }
            if(item.GetComponent<IHandItem>() == null) {
                Debug.LogError($"Prefab {item.name} in {handItemPrefabsPath} didn't implement IHandItem!");
                continue;
            }
            items.Add(item);
        }
        return items;
    }
}
#endif
