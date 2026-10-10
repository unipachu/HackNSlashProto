using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HandItemCatalog", menuName = "Scriptable Object Data/HandItemCatalog")]
public class So_HandItemCatalog : ScriptableObject {
    [SerializeField] List<GameObject> items = new();

    public IReadOnlyList<GameObject> Items => items;

#if UNITY_EDITOR
    /// <summary>
    /// Used for catalogue asset generation.
    /// </summary>
    public void SetItems(List<GameObject> newItems) {
        items.Clear();
        items.AddRange(newItems);
    }
#endif
}
